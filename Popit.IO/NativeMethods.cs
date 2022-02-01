namespace PopIt.IO;
using System.Runtime.InteropServices;
internal static class NativeMethods
{
    public record struct COORD(short X, short Y);

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT_RECORD
    {
        public const ushort
            KEY_EVENT = 1 << 0,
            MOUSE_EVENT = 1 << 1,
            WINDOW_BUFFER_SIZE_EVENT = 1 << 2;
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

        public const uint
            FROM_LEFT_1ST_BUTTON_PRESSED = 1 << 0,
            RIGHTMOST_BUTTON_PRESSED = 1 << 1,
            FROM_LEFT_2ND_BUTTON_PRESSED = 1 << 2,
            FROM_LEFT_3RD_BUTTON_PRESSED = 1 << 3,
            FROM_LEFT_4TH_BUTTON_PRESSED = 1 << 4;
        public uint dwButtonState;

        public const int
            RIGHT_ALT_PRESSED = 1 << 0,
            LEFT_ALT_PRESSED = 1 << 1,
            RIGHT_CTRL_PRESSED = 1 << 2,
            LEFT_CTRL_PRESSED = 1 << 3,
            SHIFT_PRESSED = 1 << 4,
            NUMLOCK_ON = 1 << 5,
            SCROLLLOCK_ON = 1 << 6,
            CAPSLOCK_ON = 1 << 7,
            ENHANCED_KEY = 1 << 8;
        public uint dwControlKeyState;

        public const int
            MOUSE_MOVED = 1 << 0,
            DOUBLE_CLICK = 1 << 1,
            MOUSE_WHEELED = 1 << 2,
            MOUSE_HWHEELED = 1 << 3;
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

        public const int
            RIGHT_ALT_PRESSED = 1 << 0,
            LEFT_ALT_PRESSED = 1 << 1,
            RIGHT_CTRL_PRESSED = 1 << 2,
            LEFT_CTRL_PRESSED = 1 << 3,
            SHIFT_PRESSED = 1 << 4,
            NUMLOCK_ON = 1 << 5,
            SCROLLLOCK_ON = 1 << 6,
            CAPSLOCK_ON = 1 << 7,
            ENHANCED_KEY = 1 << 8;
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


    public const uint
        ENABLE_ECHO_INPUT = 1 << 2,
        ENABLE_WINDOW_INPUT = 1 << 3,
        ENABLE_MOUSE_INPUT = 1 << 4,
        ENABLE_QUICK_EDIT_MODE = 1 << 6,
        ENABLE_EXTENDED_FLAGS = 1 << 7; //more

    [DllImport("kernel32.dll")]
    public static extern bool GetConsoleMode(IntPtr hConsoleInput, ref uint lpMode);

    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleMode(IntPtr hConsoleInput, uint dwMode);


    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern bool ReadConsoleInput(IntPtr hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern bool WriteConsoleInput(IntPtr hConsoleInput, INPUT_RECORD[] lpBuffer, uint nLength, ref uint lpNumberOfEventsWritten);

}

