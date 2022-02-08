namespace PopIt.IO;
static class IOManager
{
    #region Events
    public static event MouseEventData? LeftMouseDown = null;
    public static event MouseEventData? LeftMouseUp = null;
    public static event MouseEventData? RightMouseDown = null;
    public static event MouseEventData? RightMouseUp = null;
    public static event ResizeEventData? ResizeEvent = null;
    #endregion
    private static bool leftState = false;
    private static bool rightState = false;
    private static bool started = false;
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
        ConsoleListener.ConsoleWindowResizeEvent += e => ResizeEvent?.Invoke(e.dwSize.X, e.dwSize.Y);
    }
    public static void Stop()
    {
        ConsoleListener.Stop();
        LeftMouseDown = null;
        LeftMouseUp = null;
        RightMouseDown = null;
        RightMouseUp = null;
    }
    public delegate void MouseEventData(short x, short y);
    public delegate void ResizeEventData(short w, short h);
}
