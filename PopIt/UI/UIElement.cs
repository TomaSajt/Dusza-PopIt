using PopIt.IO;
using PopIt.Data;

namespace PopIt.UI
{
    using MouseEventCallback = IOManager.MouseEventData;
    internal class UIElement
    {
        public Rectangle Region { get; protected set; }
        public UIElement? Parent { get; private set; }
        public List<UIElement> Children { get; } = new();
        private bool mouseContained = false;

        delegate bool ForwardCondition(int x, int y);

        private UIElement()
        {
            Parent = null;
            Region = new Rectangle(0, 0, (short)Console.WindowWidth, (short)Console.WindowHeight);
        }

        public static UIElement CreateGlobalParent()
        {
            return new UIElement();
        }

        public UIElement(UIElement parent, Rectangle region)
        {
            Parent = parent;
            Parent.Children.Add(this);
            Region = region;
            IOManager.LeftMouseUp += HandleMouseEvent(OnMouseUp, Region.Contains);
            IOManager.LeftMouseDown += HandleMouseEvent(OnMouseDown, Region.Contains);
            IOManager.MouseMove += HandleMouseEvent(OnMouseMove, (x, y) =>
            {
                if (Region.Contains(x, y))
                {
                    if (!mouseContained)
                    {
                        mouseContained = true;
                        OnMouseEnter();
                    }
                    return true; // call mouse move
                }
                else
                {
                    if (mouseContained)
                    {
                        mouseContained = false;
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

        public void Render()
        {
            Draw();
            Children.ForEach(c => c.Render());
        }

        public virtual void Draw()
        {

        }

        protected virtual void OnMouseDown(int x, int y)
        {

        }

        protected virtual void OnMouseUp(int x, int y)
        {

        }

        protected virtual void OnMouseMove(int x, int y)
        {

        }

        protected virtual void OnMouseEnter()
        {

        }

        protected virtual void OnMouseLeave()
        {

        }
    }
}
