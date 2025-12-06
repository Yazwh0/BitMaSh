using BitMagic.Common.Address;
using BitMagic.X16Emulator;

namespace BitMaSh
{
    internal static class EmulatorWork
    {
        public static Emulator? Emulator { get; set; }
        private static bool _reverse = false;

        public static Emulator.EmulatorResult Emulate()
        {
            if (Emulator == null)
                throw new Exception("Emulator not set.");

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            Emulator.Pc = (ushort)((Emulator.RomBank[0x3ffd] << 8) + Emulator.RomBank[0x3ffc]);

            KeyboardIo.AttachKeyboard(Emulator);
            var display = new Display(Emulator);
            display.Start();

            Emulator.EmulatorResult ret;
            Emulator.FrameControl = FrameControl.Synced;

            while (true)
            {
                ret = Emulator.Emulate();

                break;
            }

            KeyboardIo.Stop();
            display.Stop();
            Console.CursorVisible = true;
            return ret;
        }

        // for reference only, old version with CHROUT and PLOT handling.
        public static Emulator.EmulatorResult Emulate_old()
        {
            if (Emulator == null)
                throw new Exception("Emulator not set.");

            Console.Clear();
            //Console.SetWindowSize(80, 60);
            //Console.SetBufferSize(80, 60);
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;

            Emulator.Pc = (ushort)((Emulator.RomBank[0x3ffd] << 8) + Emulator.RomBank[0x3ffc]);

            // set breakpoints
            var (primaryAddress, secondAddress) = AddressFunctions.GetMemoryLocations(0, 0xffd2); // CHROUT
            Emulator.Breakpoints[primaryAddress] = 0x02;
            Emulator.Breakpoints[secondAddress] = 0x02;

            (primaryAddress, secondAddress) = AddressFunctions.GetMemoryLocations(0, 0xfff0); // PLOT
            Emulator.Breakpoints[primaryAddress] = 0x02;
            Emulator.Breakpoints[secondAddress] = 0x02;

            KeyboardIo.AttachKeyboard(Emulator);

            Emulator.EmulatorResult ret;
            while (true)
            {
                ret = Emulator.Emulate();

                if (ret == Emulator.EmulatorResult.Breakpoint)
                {
                    switch (Emulator.Pc)
                    {
                        case 0xfff0:
                            // move cursor
                            if (Emulator.Carry)
                            {
                                Console.SetCursorPosition(Console.CursorLeft + Emulator.Y, Console.CursorTop + Emulator.X);
                                break;
                            }
                            Console.SetCursorPosition(Emulator.Y, Emulator.X);

                            break;
                        case 0xffd2:
                            switch (Emulator.A)
                            { // https://www1.cx16.dk/cx16-petscii/
                                // COLOURS
                                case 0x05:
                                    Console.ForegroundColor = ConsoleColor.White;
                                    break;
                                case 0x1c:
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case 0x1e: // Green
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                    break;
                                case 0x1f: // Blue
                                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                                    break;
                                case 0x81:
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    break;
                                case 0x90:
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    break;
                                case 0x95: // brown
                                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                                    break;
                                case 0x96: // pink
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    break;
                                case 0x97: // dark gray
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    break;
                                case 0x98: // gray
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    break;
                                case 0x99: // light green
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    break;
                                case 0x9a: // light blue
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    break;
                                case 0x9b: // light gray
                                    Console.ForegroundColor = ConsoleColor.Gray;
                                    break;
                                case 0x9c: // purple
                                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                    break;
                                case 0x9e: // yellow
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    break;
                                case 0x9f: // cyan
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    break;

                                case 0x01:
                                    var temp = Console.ForegroundColor;
                                    Console.ForegroundColor = Console.BackgroundColor;
                                    Console.BackgroundColor = temp;
                                    break;

                                // CONTROL
                                case 0x93:
                                    Console.Clear();
                                    break;
                                case 0x85: // disable ISO
                                    break;

                                // CURSOR
                                case 0x1d: // cursor right
                                    Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
                                    break;
                                case 0x9d: // cursor left
                                    Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                                    break;

                                case 0x12: // reverse on
                                    _reverse = true;
                                    break;
                                case 0x92: // reverse off
                                    _reverse = false;
                                    break;


                                case 0x0d:
                                    Console.WriteLine();
                                    break;

                                default:
                                    var foreground = Console.ForegroundColor;
                                    var background = Console.BackgroundColor;
                                    if (_reverse)
                                    {
                                        Console.BackgroundColor = foreground;
                                        Console.ForegroundColor = background;
                                    }
                                    Console.Write((char)Emulator.A);
                                    if (_reverse)
                                    {
                                        Console.BackgroundColor = background;
                                        Console.ForegroundColor = foreground;
                                    }
                                    break;
                            }
                            break;
                    }

                    continue;
                }

                break;
            }

            KeyboardIo.Stop();
            Console.CursorVisible = true;
            return ret;
        }
    }
}
