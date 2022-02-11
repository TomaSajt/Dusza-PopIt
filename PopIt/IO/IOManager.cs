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
    public static event MouseEventData? MouseMove;
    #endregion
    private static bool leftState = false;
    private static bool rightState = false;
    private static bool started = false;
    private static int prevX = -1, prevY = -1;

    static IOManager() => ClearEvents();
    private static void ClearEvents()
    {
        LeftMouseDown = null;
        LeftMouseUp = null;
        RightMouseDown = null;
        RightMouseUp = null;
        KeyPressed = null;
        ResizeEvent = null;
        MouseMove = null;
    }
    public static void Run()
    {
        if (started) return;
        started = true;

        ConsoleListener.Run();
        ConsoleListener.MouseEvent += e =>
        {
            {
                if(e.dwMousePosition.X != prevX || e.dwMousePosition.Y != prevY)
                {
                    MouseMove?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
                }
                prevX = e.dwMousePosition.X;
                prevY = e.dwMousePosition.Y;
            }
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
    public delegate void MouseEventData(int x, int y);
    public delegate void ResizeEventData(int w, int h);
    public delegate void KeyEventData(ConsoleKey key);
}
