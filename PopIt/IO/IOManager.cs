namespace PopIt.IO;
public static class IOManager
{
    #region Events
    public static event MouseEventData? MouseDown = null;
    public static event MouseEventData? MouseUp = null;
    public static event ResizeEventData? ResizeEvent = null;
    #endregion
    static bool heldState = false;
    public static void Start()
    {
        ConsoleListener.Setup();
        ConsoleListener.Start();
        ConsoleListener.MouseEvent += e =>
        {
            bool newHeldState = (e.dwButtonState & NativeMethods.MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED) == 1;
            if (newHeldState != heldState)
            {
                if (newHeldState) MouseDown?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
                else MouseUp?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
            }
            heldState = newHeldState;
        };
        ConsoleListener.ConsoleWindowResizeEvent += e =>
        {
            ResizeEvent?.Invoke(e.dwSize.X, e.dwSize.Y);
        };
    }
    public static void Stop()
    {
        ConsoleListener.Start();
        MouseDown = null;
        MouseUp = null;
    }
    public delegate void MouseEventData(short x, short y);
    public delegate void ResizeEventData(short w, short h);
}
