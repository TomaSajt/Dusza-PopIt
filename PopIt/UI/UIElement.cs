using PopIt.IO;
using PopIt.Data;

namespace PopIt.UI
{
    using MouseEventCallback = IOManager.MouseEventCallback;
    internal class UIElement
    {
        public Rectangle Region { get; protected set; }
        public bool MouseContained { get; set; }

        delegate bool ForwardCondition(int x, int y);

        public UIElement(Rectangle region)
        {
            Region = region;
            IOManager.LeftMouseUp += HandleMouseEvent(OnMouseUp, Region.Contains);
            IOManager.LeftMouseDown += HandleMouseEvent(OnMouseDown, Region.Contains);
            IOManager.MouseMove += HandleMouseEvent(OnMouseMove, (x, y) =>
            {
                if (Region.Contains(x, y))
                {
                    if (!MouseContained)
                    {
                        MouseContained = true;
                        OnMouseEnter();
                    }
                    return true; // call mouse move
                }
                else
                {
                    if (MouseContained)
                    {
                        MouseContained = false;
                        OnMouseLeave();
                    }
                    return false; // don't call mouse move
                }
            });
        }

        /// <summary>
        /// Wraps a callback with a custom condition, and calls it with relative coordinates.
        /// </summary>
        /// <param name="callback">The callback to wrap around.</param>
        /// <param name="condition">(short x, short y) => bool; The callback runs if this evaluates to true.</param>
        /// <returns>The handler to add to the event.</returns>
        MouseEventCallback HandleMouseEvent(MouseEventCallback callback, ForwardCondition? condition = null)
        {
            return (x, y) =>
            {
                if (condition is not null && condition(x, y))
                {
                    // call with relative coords
                    callback(x - Region.X, y - Region.Y);
                }
            };
        }
        public static void DrawString(string[] lines, int x, int y)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.WriteLine(lines[i]);
            }
        }
        public static void DrawString(string str, int x, int y) => DrawString(str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None), x, y);

        public static void DrawStringCentered(string str, int x, int y)
        {
            string[] lines = str.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                Console.SetCursorPosition(x - lines[i].Length/2, y + i);
                Console.WriteLine(lines[i]);
            }
        }

        public virtual void Render() { }
        public virtual void OnMouseDown(int x, int y) { }
        public virtual void OnMouseUp(int x, int y) { }
        public virtual void OnMouseMove(int x, int y) { }
        public virtual void OnMouseEnter() { }
        public virtual void OnMouseLeave() { }
    }
}
