using static PopIt.IO.NativeMethods;

namespace PopIt.IO;
static class ConsoleListener
{
    #region Events

    public static event ConsoleMouseEvent? MouseEvent;
    public static event ConsoleKeyEvent? KeyEvent;
    public static event ConsoleWindowBufferSizeEvent? WindowResizeEvent;

    #endregion

    #region Delegates

    public delegate void ConsoleMouseEvent(MOUSE_EVENT_RECORD r);

    public delegate void ConsoleKeyEvent(KEY_EVENT_RECORD r);

    public delegate void ConsoleWindowBufferSizeEvent(WINDOW_BUFFER_SIZE_RECORD r);

    #endregion

    static ConsoleListener() => ClearEvents();
    private static void ClearEvents()
    {
        MouseEvent = null;
        KeyEvent = null;
        WindowResizeEvent = null;
    }

    private static bool Running = false;
    private static uint savedMode = 0;
    public static void Run()
    {
        if (Running) return;
        Running = true;

        IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
        uint mode = 0;
        GetConsoleMode(inHandle, ref mode);
        savedMode = mode;
        // Ki kell kapcsolni a kijelölés módot, hogy ne írja felül a sima kattintás Eventet
        mode &= ~ENABLE_QUICK_EDIT_MODE;
        mode |= ENABLE_MOUSE_INPUT;
        SetConsoleMode(inHandle, mode);


        IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);
        new Thread(() =>
        {
            while (true)
            {
                uint numRead = 0;
                INPUT_RECORD[] record = { new INPUT_RECORD() };
                ReadConsoleInput(handleIn, record, 1, ref numRead);
                if (!Running)
                {
                    uint numWritten = 0;
                    WriteConsoleInput(handleIn, record, 1, ref numWritten);
                    return;
                }
                switch (record[0].EventType)
                {
                    case INPUT_RECORD.MOUSE_EVENT:
                        MouseEvent?.Invoke(record[0].MouseEvent);
                        break;
                    case INPUT_RECORD.KEY_EVENT:
                        KeyEvent?.Invoke(record[0].KeyEvent);
                        break;
                    case INPUT_RECORD.WINDOW_BUFFER_SIZE_EVENT:
                        WindowResizeEvent?.Invoke(record[0].WindowBufferSizeEvent);
                        break;
                }
            }
        }).Start();
    }
    public static void Stop()
    {
        Running = false;
        MouseEvent = null;
        KeyEvent = null;
        WindowResizeEvent = null;

        IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
        SetConsoleMode(inHandle, savedMode);
    }
}

