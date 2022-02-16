using System.Text;

namespace PopIt.IO;
public static class IOManager
{
    #region Events
    public static event MouseEventCallback? LeftMouseDown;
    public static event MouseEventCallback? LeftMouseUp;
    public static event MouseEventCallback? RightMouseDown;
    public static event MouseEventCallback? RightMouseUp;
    public static event KeyEventCallback? KeyPressed;
    public static event ResizeEventCallback? ResizeEvent;
    public static event MouseEventCallback? MouseMove;
    #endregion
    private static bool leftState = false;
    private static bool rightState = false;
    private static bool running = false;
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
    public static bool YesNoPrompt()
    {
        while (true)
        {
            var key = ReadKey(true).Key;
            if (key is ConsoleKey.Y or ConsoleKey.I) return true;
            if (key is ConsoleKey.N) return false;
        }
    }
    public static int Selection(string[] choices)
    {
        Console.WriteLine("Lehetőségek:");
        StringBuilder sb = new();
        for (int i = 0; i < choices.Length; i++)
        {
            sb.Append(i + 1);
            sb.Append(" - ");
            sb.AppendLine(choices[i]);
        }

        Console.Write(sb);

        int n = ReadInt();
        while (true)
        {
            if (n > 0 && n <= choices.Length)
                return n;
            EraseLine(Console.CursorTop - 1);
            n = ReadInt();
        }
    }


    public static int ReadInt(Predicate<int>? predicate = null) => WrapPauseUnpause(() =>
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int n) && (predicate is null || predicate(n))) return n;
                EraseLine(Console.CursorTop - 1);
            }
        });

    public static ConsoleKeyInfo ReadKey(bool intercept = false) => WrapPauseUnpause(() => Console.ReadKey(intercept));


    static void EraseLine(int line)
    {
        Console.SetCursorPosition(0, line);
        Console.WriteLine(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, line);
    }

    static T WrapPauseUnpause<T>(Func<T> action)
    {
        ConsoleListener.Pause();
        var res = action();
        ConsoleListener.Unpause();
        return res;
    }

    public static void Run()
    {
        if (running) return;
        running = true;

        ConsoleListener.Run();
        ConsoleListener.MouseEventRecieved += e =>
        {
            {
                if (e.dwMousePosition.X != prevX || e.dwMousePosition.Y != prevY)
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



        ConsoleListener.KeyEventRecieved += e =>
        {
            if (e.bKeyDown)
            {
                KeyPressed?.Invoke((ConsoleKey)e.wVirtualKeyCode);
            }
        };
        ConsoleListener.WindowResizeEventRecieved += e => ResizeEvent?.Invoke(e.dwSize.X, e.dwSize.Y);
    }
    public static void Stop()
    {
        running = false;
        ConsoleListener.Stop();
        ClearEvents();
    }
    public delegate void MouseEventCallback(int x, int y);
    public delegate void ResizeEventCallback(int w, int h);
    public delegate void KeyEventCallback(ConsoleKey key);
}
