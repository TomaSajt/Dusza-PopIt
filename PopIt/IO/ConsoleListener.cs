using static PopIt.IO.NativeMethods;

namespace PopIt.IO;
static class ConsoleListener
{

    public static event ConsoleMouseEventCallback? MouseEventRecieved;
    public static event ConsoleKeyEventCallback? KeyEventRecieved;
    public static event ConsoleWindowBufferSizeEventCallback? WindowResizeEventRecieved;

    public delegate void ConsoleMouseEventCallback(MOUSE_EVENT_RECORD r);
    public delegate void ConsoleKeyEventCallback(KEY_EVENT_RECORD r);
    public delegate void ConsoleWindowBufferSizeEventCallback(WINDOW_BUFFER_SIZE_RECORD r);

    static ConsoleListener() => ClearEvents();
    private static void ClearEvents()
    {
        MouseEventRecieved = null;
        KeyEventRecieved = null;
        WindowResizeEventRecieved = null;
    }

    private static bool running = false;
    private static bool paused = false;
    public static void Run()
    {
        if (running) return;
        running = true;

        IntPtr inHandle = GetStdHandle(STD_INPUT_HANDLE);
        uint inMode = 0;
        GetConsoleMode(inHandle, ref inMode);
        // Disables quick edit mode, which would override mouse events if not turned off
        inMode &= ~ENABLE_QUICK_EDIT_MODE;
        inMode |= ENABLE_MOUSE_INPUT;
        SetConsoleMode(inHandle, inMode);


        IntPtr outHandle = GetStdHandle(STD_OUTPUT_HANDLE);
        uint outMode = 0;
        GetConsoleMode(outHandle, ref outMode);
        // This turns on ANSI escape code support for CMD
        outMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
        SetConsoleMode(outHandle, outMode);
        IntPtr handleIn = GetStdHandle(STD_INPUT_HANDLE);
        new Thread(() =>
        {
            while (true)
            {
                if (paused) continue;
                uint numRead = 0;
                INPUT_RECORD[] record = { new INPUT_RECORD() };
                ReadConsoleInput(handleIn, record, 1, ref numRead);
                if (paused || !running)
                {
                    uint numWritten = 0;
                    //Pipes the last read char back into stdin
                    WriteConsoleInput(handleIn, record, 1, ref numWritten);
                    if (!running) return;
                    continue;
                };
                switch (record[0].EventType)
                {
                    case INPUT_RECORD.MOUSE_EVENT:
                        MouseEventRecieved?.Invoke(record[0].MouseEvent);
                        break;
                    case INPUT_RECORD.KEY_EVENT:
                        KeyEventRecieved?.Invoke(record[0].KeyEvent);
                        break;
                    case INPUT_RECORD.WINDOW_BUFFER_SIZE_EVENT:
                        WindowResizeEventRecieved?.Invoke(record[0].WindowBufferSizeEvent);
                        break;
                }
            }
        }).Start();
    }
    public static void Stop()
    {
        running = false;
        ClearEvents();
    }
    public static void Pause() => paused = true;
    public static void Unpause() => paused = false;
}

