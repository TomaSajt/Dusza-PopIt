using PopIt.IO;

namespace PopIt.UI;
using MouseEventCallback = IOManager.MouseEventCallback;
/// <summary>
/// Manages events on <see cref="UIElement"/>s
/// </summary>
static class UIManager
{
    public static HashSet<UIElement> UIElements = new();
    public static void Add(UIElement element) => UIElements.Add(element);
    public static void Remove(UIElement element) => UIElements.Remove(element);
    public static void Run()
    {
        IOManager.ResizeEvent += (w, h) =>
        {
            foreach (var el in UIElements)
            {
                el.Render();
            }
        };
        IOManager.LeftMouseUp += HandleMouseEvent(el => el.OnMouseUp, el => el.Region.Contains);
        IOManager.LeftMouseDown += HandleMouseEvent(el => el.OnMouseDown, el => el.Region.Contains);
        IOManager.MouseMove += HandleMouseEvent(el => el.OnMouseMove, el => (x, y) =>
        {
            if (el.Region.Contains(x, y))
            {
                if (!el.MouseContained)
                {
                    el.MouseContained = true;
                    el.OnMouseEnter();
                }
                return true; // call mouse move
            }
            else
            {
                if (el.MouseContained)
                {
                    el.MouseContained = false;
                    el.OnMouseLeave();
                }
                return false; // don't call mouse move
            }
        });
    }
    static MouseEventCallback HandleMouseEvent(MouseEventCallbackSelectorCallback callback, ForwardConditionSelectorCallback condition)
    {
        return (x, y) =>
        {
            foreach (var el in UIElements)
            {
                if (condition(el)(x, y))
                {
                    callback(el)(x - el.Region.X, y - el.Region.Y);
                }
            }
        };
    }
    delegate bool ForwardCondition(int x, int y);
    delegate MouseEventCallback MouseEventCallbackSelectorCallback(UIElement el);
    delegate ForwardCondition ForwardConditionSelectorCallback(UIElement el);
}
