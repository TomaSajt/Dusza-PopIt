namespace PopIt.IO;
static class IOManager
{
    #region Events
    public static event MouseEventData? LeftMouseDown;
    public static event MouseEventData? LeftMouseUp;
    public static event MouseEventData? RightMouseDown;
    public static event MouseEventData? RightMouseUp;
    public static event KeyEventData? KeyPressed;
    public static event ResizeEventData? ResizeEvent;
    #endregion
    private static bool leftState = false;
    private static bool rightState = false;
    private static bool started = false;

    static IOManager() => ClearEvents();
    private static void ClearEvents()
    {
        LeftMouseDown = null;
        LeftMouseUp = null;
        RightMouseDown = null;
        RightMouseUp = null;
        KeyPressed = null;
        ResizeEvent = null;
    }
    public static void Run()
    {
        if (started) return;
        started = true;

        ConsoleListener.Run();
        ConsoleListener.MouseEvent += e =>
        {
            {
                bool newState = (e.dwButtonState & NativeMethods.MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED) == NativeMethods.MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED;
                if (newState != leftState)
                {
                    if (newState) LeftMouseDown?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
                    else LeftMouseUp?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
                }
                leftState = newState;
            }
            {
                bool newState = (e.dwButtonState & NativeMethods.MOUSE_EVENT_RECORD.RIGHTMOST_BUTTON_PRESSED) == NativeMethods.MOUSE_EVENT_RECORD.RIGHTMOST_BUTTON_PRESSED;
                if (newState != rightState)
                {
                    if (newState) RightMouseDown?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
                    else RightMouseUp?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
                }
                rightState = newState;
            }
        };



        ConsoleListener.KeyEvent += e =>
        {
            if (e.bKeyDown)
            {
                KeyPressed?.Invoke((ConsoleKey)e.wVirtualKeyCode);
            }
        };
        ConsoleListener.WindowResizeEvent += e => ResizeEvent?.Invoke(e.dwSize.X, e.dwSize.Y);
    }
    public static void Stop()
    {
        started = false;
        ConsoleListener.Stop();
        ClearEvents();
    }
    public delegate void MouseEventData(short x, short y);
    public delegate void KeyEventData(ConsoleKey key);
    public delegate void ResizeEventData(short w, short h);
}
