using BitMagic.Common;
using BitMagic.X16Emulator;
using BitMagic.X16Emulator.Display;
using CommandLine;

namespace BitMaSh;

static class Program
{
    private const string RomEnvironmentVariable = "BITMAGIC_ROM";

    static async Task<int> Main(string[] args)
    {
        //Console.ReadKey();
        Console.WriteLine("BitMaSh");

        // Commandline
        ParserResult<CommandLineOptions>? argumentsResult;
        try
        {
            argumentsResult = Parser.Default.ParseArguments<CommandLineOptions>(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error processing arguments:");
            Console.WriteLine(ex.Message);
            return 1;
        }

        var options = argumentsResult.Value;


        // setup terminal
        //if (Console.BufferWidth < 80)
        //    Console.BufferWidth = 80;

        var emulator = new Emulator();
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var rom = options.RomFilename;

        if (rom == null || !File.Exists(rom))
        {
            rom = "rom.bin";
        }

        if (!File.Exists(rom))
        {
            var env = Environment.GetEnvironmentVariable(RomEnvironmentVariable);
            if (!string.IsNullOrWhiteSpace(env))
            {
                rom = env;

                if (!File.Exists(rom))
                {
                    rom = @$"{env}\rom.bin";
                }
            }
        }

        if (File.Exists(rom))
        {
            Console.WriteLine($"Loading '{rom}'.");
            var romData = await File.ReadAllBytesAsync(rom);
            for (var i = 0; i < romData.Length; i++)
            {
                emulator.RomBank[i] = romData[i];
            }
        }
        else
        {
            Console.WriteLine($"ROM '{rom}' not found.");
            return 2;
        }

        if (!string.IsNullOrWhiteSpace(options.Cartridge))
        {
            Console.Write($"Loading Cartridge '{options.Cartridge}'... ");
            var result = emulator.LoadCartridge(options.Cartridge);
            if (result.Result == CartridgeHelperExtension.LoadCartridgeResultCode.Ok)
                Console.WriteLine("Done.");
            else
            {
                Console.WriteLine("Error.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Result switch
                {
                    CartridgeHelperExtension.LoadCartridgeResultCode.FileNotFound => "*** File not found.",
                    CartridgeHelperExtension.LoadCartridgeResultCode.FileTooBig => "*** File too big.",
                    _ => "*** Unknown error."
                });
                Console.ResetColor();
            }
        }


        SdCard sdCard = string.IsNullOrEmpty(options.SdCardFileName) ? new SdCard(options.SdCardSize, new NullLogger()) : new SdCard(options.SdCardFileName, new NullLogger());

        emulator.LoadSdCard(sdCard);

        // set syncing options
        if (options.SdCardFullSync)
        {
            options.SdCardSyncFrom = true;
            options.SdCardSyncTo = true;
        }

        // create the sdcard
        if (!string.IsNullOrWhiteSpace(options.SdCardFolder))
        {
            emulator.SdCard!.SetHomeDirectory(options.SdCardFolder, options.SdCardSyncTo, false);
        }

        // add files after directories.
        if (options.SdCardFiles != null)
        {
            foreach (var file in options.SdCardFiles)
            {
                sdCard.AddFiles(file, file, false);
            }
        }

        if (options.SdCardUpdate)
        {
            if (!string.IsNullOrEmpty(options.SdCardFileName))
            {
                options.SdCardWrite = options.SdCardFileName;
                options.SdCardOverrwrite = true;
            }
            else
            {
                Console.WriteLine("Cannot set `update source SD Card` when the source SD Card is not set. Use with --sdcard.");
            }
        }

        Thread? syncThread = null;
        if (options.SdCardSyncFrom)
        {
            syncThread = emulator.SdCard!.StartX16Watcher();
            syncThread.Start();
        }

        emulator.Control = Control.Run;
        emulator.FrameControl = FrameControl.Run;
        emulator.Stepping = false;

        var audio = new EmulatorAudio(emulator);
        audio.StartPlayback();

        EmulatorWork.Emulator = emulator;

        var returnCode = EmulatorWork.Emulate();

        audio.StopPlayback();

        if (syncThread != null)
        {
            emulator.SdCard!.StopX16Watcher();
            syncThread.Join();
        }

        // once emulation is over write sdcard if requested
        if (!string.IsNullOrWhiteSpace(options.SdCardWrite))
            emulator.SdCard!.Save(options.SdCardWrite, options.SdCardOverrwrite);

        Console.WriteLine($"Emulator finished with return '{returnCode}'.");
        return 0;
    }
}

public class NullLogger : IEmulatorLogger
{
    public void Log(string message)
    {
    }

    public void LogError(string message)
    {
    }

    public void LogError(string message, ISourceFile source, int lineNumber)
    {
    }

    public void LogLine(string message)
    {
    }
}