using PopIt.IO;
using PopIt.Data;

namespace PopIt.UI
{
    internal class UIElement
    {
        public Rectangle Region { get; protected set; }
        public bool MouseContained { get; set; }

        delegate bool ForwardCondition(int x, int y);

        public UIElement(Rectangle region)
        {
            Region = region;
        }
        public static void DrawString(string[] lines, int x, int y)
        {
            try
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    Console.SetCursorPosition(Math.Max(0, x), y + i);
                    Console.WriteLine(lines[i][Math.Max(0, -x)..]);
                }
            }
            catch { }
        }
        public static void DrawString(string str, int x, int y) => DrawString(str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None), x, y);
        public virtual void Render() { }
        public virtual void OnMouseDown(int x, int y) { }
        public virtual void OnMouseUp(int x, int y) { }
        public virtual void OnMouseMove(int x, int y) { }
        public virtual void OnMouseEnter() { }
        public virtual void OnMouseLeave() { }
        public virtual void OnKeyDown(ConsoleKey key) { }
    }
}
