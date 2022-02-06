namespace PopIt.Game.IO;
public static class IOManager
{
    #region Events
    public static event MouseEvent? MouseDown = null;
    public static event MouseEvent? MouseUp = null;
    #endregion
    static bool heldState = false;
    public static void Start()
    {
        ConsoleListener.Setup();
        ConsoleListener.Start();
        ConsoleListener.MouseEventRecieved += e =>
        {
            bool newHeldState = (e.dwButtonState & NativeMethods.MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED) == 1;
            if (newHeldState != heldState)
            {
                if (newHeldState) MouseDown?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
                else MouseUp?.Invoke(e.dwMousePosition.X, e.dwMousePosition.Y);
            }
            heldState = newHeldState;
        };
    }
    public static void Stop()
    {
        ConsoleListener.Start();
        MouseDown = null;
        MouseUp = null;
    }
    public delegate void MouseEvent(short x, short y);
}
