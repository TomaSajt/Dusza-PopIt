using System.Runtime.InteropServices;
using static Popit.IO.NativeMethods;

namespace Popit.IO
{
    public static class ConsoleListener
    {
        public static event ConsoleMouseEvent? MouseEvent;
        private static bool Run = false;

        public static void Setup()
        {
            IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
            uint mode = 0;
            GetConsoleMode(inHandle, ref mode);
            // Ki kell kapcsolni a kijelölés módot, hogy ne írja felül a sima MouseEvent-et
            mode &= ~ENABLE_QUICK_EDIT_MODE;
            mode |= ENABLE_MOUSE_INPUT;
            SetConsoleMode(inHandle, mode);
        }
        public static void Start()
        {
            if (!Run)
            {
                Run = true;
                IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);
                new Thread(() =>
                {
                    while (true)
                    {
                        uint numRead = 0;
                        INPUT_RECORD[] record = new INPUT_RECORD[1];
                        record[0] = new INPUT_RECORD();
                        ReadConsoleInput(handleIn, record, 1, ref numRead);
                        if (!Run)
                        {
                            uint numWritten = 0;
                            WriteConsoleInput(handleIn, record, 1, ref numWritten);
                            return;
                        }

                        if(record[0].EventType == INPUT_RECORD.MOUSE_EVENT)
                        {
                            MouseEvent?.Invoke(record[0].MouseEvent);
                        }
                    }
                }).Start();
            }
        }

        public static void Stop() => Run = false;


        public delegate void ConsoleMouseEvent(MOUSE_EVENT_RECORD r);

        public delegate void ConsoleKeyEvent(KEY_EVENT_RECORD r);

        public delegate void ConsoleWindowBufferSizeEvent(WINDOW_BUFFER_SIZE_RECORD r);

        [DllImport("user32.dll")]
        public static extern bool GetAsyncKeyState(int button);
        public static bool IsMouseButtonPressed(MouseButton button) => GetAsyncKeyState((int)button);
        
        public enum MouseButton
        {
            LeftMouseButton = 0x01,
            RightMouseButton = 0x02,
            MiddleMouseButton = 0x04,
        }
    }


    public static class NativeMethods
    {
        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            public const ushort KEY_EVENT = 0x0001,
                MOUSE_EVENT = 0x0002,
                WINDOW_BUFFER_SIZE_EVENT = 0x0004; //more

            [FieldOffset(0)]
            public ushort EventType;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
        }

        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;

            public const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001,
                FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004,
                FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008,
                FROM_LEFT_4TH_BUTTON_PRESSED = 0x0010,
                RIGHTMOST_BUTTON_PRESSED = 0x0002;
            public uint dwButtonState;

            public const int CAPSLOCK_ON = 0x0080,
                ENHANCED_KEY = 0x0100,
                LEFT_ALT_PRESSED = 0x0002,
                LEFT_CTRL_PRESSED = 0x0008,
                NUMLOCK_ON = 0x0020,
                RIGHT_ALT_PRESSED = 0x0001,
                RIGHT_CTRL_PRESSED = 0x0004,
                SCROLLLOCK_ON = 0x0040,
                SHIFT_PRESSED = 0x0010;
            public uint dwControlKeyState;

            public const int DOUBLE_CLICK = 0x0002,
                MOUSE_HWHEELED = 0x0008,
                MOUSE_MOVED = 0x0001,
                MOUSE_WHEELED = 0x0004;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0)]
            public bool bKeyDown;
            [FieldOffset(4)]
            public ushort wRepeatCount;
            [FieldOffset(6)]
            public ushort wVirtualKeyCode;
            [FieldOffset(8)]
            public ushort wVirtualScanCode;
            [FieldOffset(10)]
            public char UnicodeChar;
            [FieldOffset(10)]
            public byte AsciiChar;

            public const int CAPSLOCK_ON = 0x0080,
                ENHANCED_KEY = 0x0100,
                LEFT_ALT_PRESSED = 0x0002,
                LEFT_CTRL_PRESSED = 0x0008,
                NUMLOCK_ON = 0x0020,
                RIGHT_ALT_PRESSED = 0x0001,
                RIGHT_CTRL_PRESSED = 0x0004,
                SCROLLLOCK_ON = 0x0040,
                SHIFT_PRESSED = 0x0010;
            [FieldOffset(12)]
            public uint dwControlKeyState;
        }

        public struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;
        }

        public const uint STD_INPUT_HANDLE = unchecked((uint)-10),
            STD_OUTPUT_HANDLE = unchecked((uint)-11),
            STD_ERROR_HANDLE = unchecked((uint)-12);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);


        public const uint ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_WINDOW_INPUT = 0x0008; //more

        [DllImportAttribute("kernel32.dll")]
        public static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

        [DllImportAttribute("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleInput, uint dwMode);


        [DllImportAttribute("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

        [DllImportAttribute("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern bool WriteConsoleInput(IntPtr hConsoleInput, INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsWritten);

    }
}