using CommandLine;

namespace BitMaSh;

internal class CommandLineOptions
{
    [Option('r', "rom", Required = false, HelpText = "rom file to load, will look for rom.bin using locally or falling back to the BITMAGIC_ROM environment variable.")]
    public string RomFilename { get; set; } = "rom.bin";

    [Option('s', "sdcard", Required = false, HelpText = "SD Card to attach. Can be a .zip or .gz file, in the form 'name.xxx.zip', where xxx is either BIN or VHD.")]
    public string? SdCardFileName { get; set; }

    [Option("sdcard-size", Required = false, HelpText = "SD Card size in mb if the card is being created by the emulator.")]
    public ulong SdCardSize { get; set; } = 16;

    [Option('d', "sdcard-folder", Required = false, HelpText = "Set the home folder for the SD Card.")]
    public string? SdCardFolder { get; set; }

    [Option("sdcard-synctox16", Required = false, HelpText = "Sync any changes to the home directory to SD Card. Root directory only.")]
    public bool SdCardSyncTo { get; set; } = false;

    [Option("sdcard-syncfromx16", Required = false, HelpText = "Sync any changes to SD Card to the home directory. Root directory only.")]
    public bool SdCardSyncFrom { get; set; } = false;

    [Option('y', "sdcard-sync", Required = false, HelpText = "Sync any changes to SD Card to the home directory, or vice versa. Root directory only. Same as setting --sdcard-synctox16 --sdcard-syncfromx16.")]
    public bool SdCardFullSync { get; set; } = false;

    [Option('f', "sdcard-file", Required = false, HelpText = "File to add to the SD Card root directory. Can add multiple files and use wildcards.")]
    public IEnumerable<string>? SdCardFiles { get; set; }

    [Option("sdcard-write", Required = false, HelpText = "SD Card file to write at the end of emulation. Can be a .zip or .gz file, in the form 'name.xxx.zip', where xxx is either BIN or VHD.")]
    public string? SdCardWrite { get; set; }

    [Option("sdcard-overwrite", Required = false, HelpText = "When writing the SD Card file, it can overwrite.")]
    public bool SdCardOverrwrite { get; set; } = false;


    [Option('u', "sdcard-update", Required = false, HelpText = "Sets 'sdcard-write' to the 'sdcard' parameter and enables overwrite.")]
    public bool SdCardUpdate { get; set; } = false;

    [Option("cart", Required = false, HelpText = "Cartridge file to load as a standard binary file. Can be a .zip or .gz file, in the form 'name.cart.zip'.")]
    public string Cartridge { get; set; } = "";

}
