using PopIt.Data;

namespace PopIt.UI.Components;
/// <summary>
/// This class is unused
/// </summary>
internal class Button : UIElement
{
    public Color BaseColor { get; set; }
    public Color HoverColor { get; set; }
    public Color PressedColor { get; set; }
    private Color CurrentColor { get; set; }
    public event Action? Clicked;
    public string Text { get; set; }
    private bool clicking = false;

    public record struct Properties(Color BaseColor, Color HoverColor, Color PressedColor, string Text = "");

    public Button(Properties props, Rectangle region) : base(region)
    {
        Text = props.Text;
        BaseColor = props.BaseColor;
        HoverColor = props.HoverColor;
        PressedColor = props.PressedColor;
        CurrentColor = BaseColor;
    }
    public override void Render()
    {
        Console.Write(CurrentColor.ToBackColStr());
        for (int i = 0; i < Region.Height; i++)
        {
            Console.SetCursorPosition(Region.X, Region.Y + i);
            Console.Write(new string(' ', Region.Width));
        }
        Console.Write(ConsoleCodes.RESET);
    }

    public override void OnMouseEnter()
    {
        CurrentColor = HoverColor;
        Render();
    }

    public override void OnMouseLeave()
    {
        clicking = false;
        CurrentColor = BaseColor;
        Render();
    }

    public override void OnMouseDown(int x, int y)
    {
        clicking = true;
        CurrentColor = PressedColor;
        Render();
    }

    public override void OnMouseUp(int x, int y)
    {
        if (clicking)
        {
            Clicked?.Invoke();
        }
        clicking = false;
        CurrentColor = HoverColor;
        Render();
    }
}
