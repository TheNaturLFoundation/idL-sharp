using System;
using System.Windows.Input;

namespace IDL_for_NaturL
{
    public static class KeyUtil
    {
        public static char KeyToChar(Key key)
        {

            if (Keyboard.IsKeyDown(Key.LeftAlt) ||
                Keyboard.IsKeyDown(Key.RightAlt) ||
                Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightAlt))
            {
                return '\x00';
            }

            bool caplock = Console.CapsLock;
            bool shift = Keyboard.IsKeyDown(Key.LeftShift) ||
                         Keyboard.IsKeyDown(Key.RightShift);
            bool iscap = (caplock && !shift) || (!caplock && shift);

            switch (key)
            {
                case Key.Enter: return '\n';
                case Key.A: return (iscap ? 'A' : 'a');
                case Key.B: return (iscap ? 'B' : 'b');
                case Key.C: return (iscap ? 'C' : 'c');
                case Key.D: return (iscap ? 'D' : 'd');
                case Key.E: return (iscap ? 'E' : 'e');
                case Key.F: return (iscap ? 'F' : 'f');
                case Key.G: return (iscap ? 'G' : 'g');
                case Key.H: return (iscap ? 'H' : 'h');
                case Key.I: return (iscap ? 'I' : 'i');
                case Key.J: return (iscap ? 'J' : 'j');
                case Key.K: return (iscap ? 'K' : 'k');
                case Key.L: return (iscap ? 'L' : 'l');
                case Key.M: return (iscap ? 'M' : 'm');
                case Key.N: return (iscap ? 'N' : 'n');
                case Key.O: return (iscap ? 'O' : 'o');
                case Key.P: return (iscap ? 'P' : 'p');
                case Key.Q: return (iscap ? 'Q' : 'q');
                case Key.R: return (iscap ? 'R' : 'r');
                case Key.S: return (iscap ? 'S' : 's');
                case Key.T: return (iscap ? 'T' : 't');
                case Key.U: return (iscap ? 'U' : 'u');
                case Key.V: return (iscap ? 'V' : 'v');
                case Key.W: return (iscap ? 'W' : 'w');
                case Key.X: return (iscap ? 'X' : 'x');
                case Key.Y: return (iscap ? 'Y' : 'y');
                case Key.Z: return (iscap ? 'Z' : 'z');
                case Key.D0: return (shift ? ')' : '0');
                case Key.D1: return (shift ? '!' : '1');
                case Key.D2: return (shift ? '@' : '2');
                case Key.D3: return (shift ? '#' : '3');
                case Key.D4: return (shift ? '$' : '4');
                case Key.D5: return (shift ? '%' : '5');
                case Key.D6: return (shift ? '^' : '6');
                case Key.D7: return (shift ? '&' : '7');
                case Key.D8: return (shift ? '*' : '8');
                case Key.D9: return (shift ? '(' : '9');
                case Key.OemPlus: return (shift ? '+' : '=');
                case Key.OemMinus: return (shift ? '_' : '-');
                case Key.OemQuestion: return (shift ? '?' : '/');
                case Key.OemComma: return (shift ? '<' : ',');
                case Key.OemPeriod: return (shift ? '>' : '.');
                case Key.OemOpenBrackets: return (shift ? '{' : '[');
                case Key.OemQuotes: return (shift ? '"' : '\'');
                case Key.Oem1: return (shift ? ':' : ';');
                case Key.Oem3: return (shift ? '~' : '`');
                case Key.Oem5: return (shift ? '|' : '\\');
                case Key.Oem6: return (shift ? '}' : ']');
                case Key.Tab: return '\t';
                case Key.Space: return ' ';

                // Number Pad
                case Key.NumPad0: return '0';
                case Key.NumPad1: return '1';
                case Key.NumPad2: return '2';
                case Key.NumPad3: return '3';
                case Key.NumPad4: return '4';
                case Key.NumPad5: return '5';
                case Key.NumPad6: return '6';
                case Key.NumPad7: return '7';
                case Key.NumPad8: return '8';
                case Key.NumPad9: return '9';
                case Key.Subtract: return '-';
                case Key.Add: return '+';
                case Key.Decimal: return '.';
                case Key.Divide: return '/';
                case Key.Multiply: return '*';

                default: return '\x00';
            }
        }
    }
}