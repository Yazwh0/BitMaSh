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
            var screenWidth = Math.Min(128, Console.WindowWidth);
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
            0x00 => '@',
            0x01 => 'A',
            0x02 => 'B',
            0x03 => 'C',
            0x04 => 'D',
            0x05 => 'E',
            0x06 => 'F',
            0x07 => 'G',
            0x08 => 'H',
            0x09 => 'I',
            0x0A => 'J',
            0x0B => 'K',
            0x0C => 'L',
            0x0D => 'M',
            0x0E => 'N',
            0x0F => 'O',
            0x10 => 'P',
            0x11 => 'Q',
            0x12 => 'R',
            0x13 => 'S',
            0x14 => 'T',
            0x15 => 'U',
            0x16 => 'V',
            0x17 => 'W',
            0x18 => 'X',
            0x19 => 'Y',
            0x1A => 'Z',
            0x1B => '[',
            0x1C => '£',
            0x1D => ']',
            0x1E => '\u2191',   // ↑
            0x1F => '\u2190',   // ←
            0x20 => ' ',
            0x21 => '!',
            0x22 => '"',
            0x23 => '#',
            0x24 => '$',
            0x25 => '%',
            0x26 => '&',
            0x27 => '\'',
            0x28 => '(',
            0x29 => ')',
            0x2A => '*',
            0x2B => '+',
            0x2C => ',',
            0x2D => '-',
            0x2E => '.',
            0x2F => '/',
            0x30 => '0',
            0x31 => '1',
            0x32 => '2',
            0x33 => '3',
            0x34 => '4',
            0x35 => '5',
            0x36 => '6',
            0x37 => '7',
            0x38 => '8',
            0x39 => '9',
            0x3A => ':',
            0x3B => ';',
            0x3C => '<',
            0x3D => '=',
            0x3E => '>',
            0x3F => '?',
            0x40 => '\u2501',   // ━
            0x41 => '\u2660',   // ♠
            0x42 => '\u2503',   // ┃
            0x43 => '\u2501',   // ━
            0x44 => ' ',
            0x45 => ' ',
            0x46 => ' ',
            0x47 => ' ',
            0x48 => ' ',
            0x49 => '\u256E',   // ╮
            0x4A => '\u2570',   // ╰
            0x4B => '\u256F',   // ╯
            0x4C => ' ',
            0x4D => '\u2572',   // ╲
            0x4E => '\u2571',   // ╱
            0x4F => ' ',

            0x50 => ' ',
            0x51 => '\u2022',
            0x52 => ' ',
            0x53 => '\u2665',
            0x54 => ' ',
            0x55 => '\u256d',
            0x56 => '\u2573',
            0x57 => '\u25cb',
            0x58 => '\u2663',
            0x59 => ' ',
            0x5A => '\u2666',
            0x5B => '\u253c',
            0x5C => ' ',
            0x5D => '\u007c',
            0x5E => '\u03c0',
            0x5F => '\u25e5',

            0x60 => ' ',
            0x61 => '\u258c',
            0x62 => '\u2584',
            0x63 => '\u2594',
            0x64 => '\u2581',
            0x65 => '\u258e',
            0x66 => '\u2592',
            0x67 => '\u2595', // broken?
           // 0x67 => '\ue0a7',
            0x68 => ' ',
            0x69 => '\u25e4',
            0x6A => '\u258a',
            0x6B => '\u2523',
            0x6C => '\u2597',
            0x6D => '\u2517',
            0x6E => '\u2513',
            0x6F => '\u2582',

            0x70 => '\u250f',
            0x71 => '\u253b',
            0x72 => '\u2533',
            0x73 => '\u252b',
            0x74 => '\u258e',
            0x75 => '\u258d',
            0x76 => '\u258b', // needs invert
            0x77 => '\u2594',
            0x78 => '\u2585',
            0x79 => '\u2585',
            0x7A => ' ',
            0x7B => '\u2596',
            0x7C => '\u259d',
            0x7D => '\u251b',
            0x7E => '\u2598',
            0x7F => '\u259a',

            _ => ' ',
        };

        if (tileIndex == 0x76)
            inverse = !inverse;
        if (tileIndex == 0x6a)
            inverse = !inverse;
        //if (tileIndex == 0x67 && !inverse)
        //{
        //    inverse = true;
        //}
        //else if (tileIndex == 0x67 && inverse)
        //{
        //    toReturn = '\ue2a7';
        //    inverse = false;
        //}

        if (tileIndex == 0x67)
        {
            var a = 0;
        }

        return (toReturn, inverse);
    }
}
