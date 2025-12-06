using BitMagic.X16Emulator;
using ANSIConsole;

namespace BitMaSh;

internal class Display(Emulator emulator)
{
    private Task? DisplayUpdater { get; set; } = null;
    private bool _stopRequested = false;

    public void Start()
    {
        DisplayUpdater = new Task(ProcessDisplay);
        DisplayUpdater.Start();
    }

    public void Stop()
    {
        _stopRequested = true;
    }

    private void ProcessDisplay()
    {
        var vram = emulator.Vera.Vram;
        var ram = emulator.Memory;
        var colours = emulator.Palette;

        var prevWidth = 0;
        var prevHeight = 0;

        int[] screenData = [];

        while (!_stopRequested)
        {
            // get tilemap
            var tilemapAddress = (int)emulator.Vera.Layer1_MapAddress;

            // map is 128 x 64, need display settings
            var screenWidth = Math.Min(80, Console.WindowWidth);
            var screenHeight = Math.Min(60, Console.WindowHeight);

            ram[0x386] = (byte)screenWidth;
            ram[0x387] = (byte)screenHeight;

            if (prevHeight != screenHeight || prevWidth != screenWidth)
            {
                prevHeight = screenHeight;
                prevWidth = screenWidth;

                screenData = new int[screenWidth * screenHeight * 2];
            }

            for (var y = 0; y < screenHeight; y++)
            {
                for(var x = 0; x < screenWidth; x++)
                {
                    var address = tilemapAddress + y * 128 * 2 + x * 2;
                    if (address < 0x20000)
                    {
                        // expecting 1bpp tile mode
                        var tileIndex = vram[address]; // character index
                        var tileColour = vram[address + 1]; // colour 2 nibbles: bbbbffff

                        var foregroundColorIndex = tileColour & 0x0F;
                        var backgroundColorIndex = (tileColour >> 4) & 0x0F;

                        var foreground = colours[foregroundColorIndex];
                        var background = colours[backgroundColorIndex];

                        var idx = y * screenWidth * 2 + x * 2;

                        if (screenData[idx] == tileIndex && screenData[idx + 1] == tileColour)
                            continue;

                        screenData[idx] = tileIndex;
                        screenData[idx + 1] = tileColour;

                        Console.SetCursorPosition(x, y);
                        var character = GetTileCharacter(tileIndex);
                        if (character.inverse)
                            Console.Write(character.character.ToString()
                                .Color(background.R, background.G, background.B)
                                .Background(foreground.R, foreground.B, foreground.G));
                        else
                            Console.Write(character.character.ToString()
                            .Color(foreground.R, foreground.G, foreground.B)
                            .Background(background.R, background.B, background.G));
                    }
                }
            }

            Task.Delay(100);
        }
    }

    private (char character, bool inverse) GetTileCharacter(byte tileIndex)
    {
        var inverse = (tileIndex & 0x80) != 0;
        tileIndex = (byte)(tileIndex & 0x7f);

        var toReturn = tileIndex switch
        {
            0 => '@',
            1 => 'A',
            2 => 'B',
            3 => 'C',
            4 => 'D',
            5 => 'E',
            6 => 'F',
            7 => 'G',
            8 => 'H',
            9 => 'I',
            10 => 'J',
            11 => 'K',
            12 => 'L',
            13 => 'M',
            14 => 'N',
            15 => 'O',
            16 => 'P',
            17 => 'Q',
            18 => 'R',
            19 => 'S',
            20 => 'T',
            21 => 'U',
            22 => 'V',
            23 => 'W',
            24 => 'X',
            25 => 'Y',
            26 => 'Z',
            27 => '[',
            28 => '£',
            29 => ']',
            30 => '^',
            31 => '_',
            32 => ' ',
            33 => '!',
            34 => '"',
            35 => '#',
            36 => '$',
            37 => '%',
            38 => '&',
            39 => '\'',
            40 => '(',
            41 => ')',
            42 => '*',
            43 => '+',
            44 => ',',
            45 => '-',
            46 => '.',
            47 => '/',
            48 => '0',
            49 => '1',
            50 => '2',
            51 => '3',
            52 => '4',
            53 => '5',
            54 => '6',
            55 => '7',
            56 => '8',
            57 => '9',
            58 => ':',
            59 => ';',
            60 => '<',
            61 => '=',
            62 => '>',
            63 => '?',
            _ => ' ',
        };

        return (toReturn, inverse);
    }
}
