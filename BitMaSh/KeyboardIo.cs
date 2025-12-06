using BitMagic.DiscUtils.Streams;
using BitMagic.X16Emulator;
using Silk.NET.Input;

namespace BitMaSh;

internal static class KeyboardIo
{
    private static Task? KeyboardRead { get; set; } = null;
    private static bool _stopRequested = false;
    private static SmcBuffer? _smcBuffer { get;set; } = null;

    public static void AttachKeyboard(Emulator emulator)
    {
        //var stdin = Console.OpenStandardInput();

        _smcBuffer = new SmcBuffer(emulator);
        KeyboardRead = new Task(ProcessKeyboard);

        KeyboardRead.Start();
    }

    public static void Stop()
    {
        _stopRequested = true;
        _smcBuffer = null;
    }

    private static void ProcessKeyboard()
    {
        while(!_stopRequested)
        {
            if (Console.KeyAvailable)
            {
                var c = Console.ReadKey(intercept: true);

                //c.Modifiers
                if (_smcBuffer != null)
                {
                    if (c.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        _smcBuffer.KeyDown(Key.ShiftLeft);

                    _smcBuffer.KeyDown(Convert(c));
                    _smcBuffer.KeyUp(Convert(c));

                    if (c.Modifiers.HasFlag(ConsoleModifiers.Shift))
                        _smcBuffer.KeyUp(Key.ShiftLeft);
                }
            }
            else
                Thread.Sleep(10);
        }
    }

    private static Key Convert(ConsoleKeyInfo consoleKeyInfo)
    {
        switch (consoleKeyInfo.Key)
        {
            // Letters
            case ConsoleKey.A: return Key.A;
            case ConsoleKey.B: return Key.B;
            case ConsoleKey.C: return Key.C;
            case ConsoleKey.D: return Key.D;
            case ConsoleKey.E: return Key.E;
            case ConsoleKey.F: return Key.F;
            case ConsoleKey.G: return Key.G;
            case ConsoleKey.H: return Key.H;
            case ConsoleKey.I: return Key.I;
            case ConsoleKey.J: return Key.J;
            case ConsoleKey.K: return Key.K;
            case ConsoleKey.L: return Key.L;
            case ConsoleKey.M: return Key.M;
            case ConsoleKey.N: return Key.N;
            case ConsoleKey.O: return Key.O;
            case ConsoleKey.P: return Key.P;
            case ConsoleKey.Q: return Key.Q;
            case ConsoleKey.R: return Key.R;
            case ConsoleKey.S: return Key.S;
            case ConsoleKey.T: return Key.T;
            case ConsoleKey.U: return Key.U;
            case ConsoleKey.V: return Key.V;
            case ConsoleKey.W: return Key.W;
            case ConsoleKey.X: return Key.X;
            case ConsoleKey.Y: return Key.Y;
            case ConsoleKey.Z: return Key.Z;

            // Digits (top row)
            case ConsoleKey.D0: return Key.Number0;
            case ConsoleKey.D1: return Key.Number1;
            case ConsoleKey.D2: return Key.Number2;
            case ConsoleKey.D3: return Key.Number3;
            case ConsoleKey.D4: return Key.Number4;
            case ConsoleKey.D5: return Key.Number5;
            case ConsoleKey.D6: return Key.Number6;
            case ConsoleKey.D7: return Key.Number7;
            case ConsoleKey.D8: return Key.Number8;
            case ConsoleKey.D9: return Key.Number9;

            // Numpad
            case ConsoleKey.NumPad0: return Key.Keypad0;
            case ConsoleKey.NumPad1: return Key.Keypad1;
            case ConsoleKey.NumPad2: return Key.Keypad2;
            case ConsoleKey.NumPad3: return Key.Keypad3;
            case ConsoleKey.NumPad4: return Key.Keypad4;
            case ConsoleKey.NumPad5: return Key.Keypad5;
            case ConsoleKey.NumPad6: return Key.Keypad6;
            case ConsoleKey.NumPad7: return Key.Keypad7;
            case ConsoleKey.NumPad8: return Key.Keypad8;
            case ConsoleKey.NumPad9: return Key.Keypad9;
            case ConsoleKey.Decimal: return Key.KeypadDecimal;
            case ConsoleKey.Add: return Key.KeypadAdd;
            case ConsoleKey.Subtract: return Key.KeypadSubtract;
            case ConsoleKey.Multiply: return Key.KeypadMultiply;
            case ConsoleKey.Divide: return Key.KeypadDivide;

            // Function keys
            case ConsoleKey.F1: return Key.F1;
            case ConsoleKey.F2: return Key.F2;
            case ConsoleKey.F3: return Key.F3;
            case ConsoleKey.F4: return Key.F4;
            case ConsoleKey.F5: return Key.F5;
            case ConsoleKey.F6: return Key.F6;
            case ConsoleKey.F7: return Key.F7;
            case ConsoleKey.F8: return Key.F8;
            case ConsoleKey.F9: return Key.F9;
            case ConsoleKey.F10: return Key.F10;
            case ConsoleKey.F11: return Key.F11;
            case ConsoleKey.F12: return Key.F12;

            // Arrows
            case ConsoleKey.LeftArrow: return Key.Left;
            case ConsoleKey.RightArrow: return Key.Right;
            case ConsoleKey.UpArrow: return Key.Up;
            case ConsoleKey.DownArrow: return Key.Down;

            // Control keys
            case ConsoleKey.Enter: return Key.Enter;
            case ConsoleKey.Spacebar: return Key.Space;
            case ConsoleKey.Tab: return Key.Tab;
            case ConsoleKey.Backspace: return Key.Backspace;
            case ConsoleKey.Escape: return Key.Escape;
            case ConsoleKey.Delete: return Key.Delete;
            case ConsoleKey.Insert: return Key.Insert;
            case ConsoleKey.Home: return Key.Home;
            case ConsoleKey.End: return Key.End;
            case ConsoleKey.PageUp: return Key.PageUp;
            case ConsoleKey.PageDown: return Key.PageDown;

            // Modifiers
            //case ConsoleKey.LeftShift: return Key.ShiftLeft;
            //case ConsoleKey.RightShift: return Key.ShiftRight;
            //case ConsoleKey.LeftControl: return Key.ControlLeft;
            //case ConsoleKey.RightControl: return Key.ControlRight;
            //case ConsoleKey.LeftAlt: return Key.AltLeft;
            //case ConsoleKey.RightAlt: return Key.AltRight;

            // Symbols
            case ConsoleKey.OemPlus: return Key.Equal;
            case ConsoleKey.OemMinus: return Key.Minus;
            case ConsoleKey.OemComma: return Key.Comma;
            case ConsoleKey.OemPeriod: return Key.Period;
            case ConsoleKey.Oem1: return Key.Semicolon;
            case ConsoleKey.Oem2: return Key.Slash;
            case ConsoleKey.Oem3: return Key.Apostrophe;
            //case ConsoleKey.Oem4: return Key.BracketLeft;
            //case ConsoleKey.Oem5: return Key.Backslash;
            //case ConsoleKey.Oem6: return Key.BracketRight;
            //case ConsoleKey.Oem7: return Key.Apostrophe;

            default: return Key.Unknown;
        }
    }

}
