namespace PopIt.IO;
using static PopIt.IO.NativeMethods;
internal static class ConsoleListener
{
    #region Events

    public static event ConsoleMouseEvent? MouseEvent = null;
    public static event ConsoleWindowBufferSizeEvent? ConsoleWindowResizeEvent = null;

    #endregion

    #region Delegates

    public delegate void ConsoleMouseEvent(MOUSE_EVENT_RECORD r);

    public delegate void ConsoleKeyEvent(KEY_EVENT_RECORD r);

    public delegate void ConsoleWindowBufferSizeEvent(WINDOW_BUFFER_SIZE_RECORD r);

    #endregion

    private static bool Started = false;

    public static void Setup()
    {
        IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
        uint mode = 0;
        GetConsoleMode(inHandle, ref mode);
        // Ki kell kapcsolni a kijelölés módot, hogy ne írja felül a sima kattintás Eventet
        mode &= ~ENABLE_QUICK_EDIT_MODE;
        mode |= ENABLE_MOUSE_INPUT;
        SetConsoleMode(inHandle, mode);
    }
    public static void Start()
    {
        if (Started) return;
        Started = true;
        IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);
        new Thread(() =>
        {
            while (true)
            {
                uint numRead = 0;
                INPUT_RECORD[] record = new INPUT_RECORD[1];
                record[0] = new INPUT_RECORD();
                ReadConsoleInput(handleIn, record, 1, ref numRead);
                if (!Started)
                {
                    uint numWritten = 0;
                    WriteConsoleInput(handleIn, record, 1, ref numWritten);
                    return;
                }
                if (record[0].EventType == INPUT_RECORD.MOUSE_EVENT)
                {
                    MouseEvent?.Invoke(record[0].MouseEvent);
                }
                if (record[0].EventType == INPUT_RECORD.WINDOW_BUFFER_SIZE_EVENT)
                {
                    ConsoleWindowResizeEvent?.Invoke(record[0].WindowBufferSizeEvent);
                }
            }
        }).Start();
    }
    public static void EndCapture() => Started = false;
}

