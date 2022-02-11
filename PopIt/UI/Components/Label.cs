using PopIt.Data;

namespace PopIt.UI.Components
{
    internal class Label : UIElement
    {
        public enum TextAlignment
        {
            TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight
        }
        public struct Properties
        {
            public string Text = "";
            public TextAlignment Alignment = TextAlignment.TopLeft;
            public Color? TextColor = null;
            public Color? BackgroundColor = null;
        }

        public string Text { get; set; }
        public TextAlignment Alignment { get; set; }
        public Color? TextColor { get; set; }
        public Color? BackgroundColor { get; set; }
        public Label(Properties props, UIElement parent, Rectangle region) : base(parent, region)
        {
            Text = props.Text;
            Alignment = props.Alignment;
            TextColor = props.TextColor;
            BackgroundColor = props.BackgroundColor;
        }

        private Point CalculatePosition()
        {
            return Alignment switch
            {
                TextAlignment.TopLeft => new Point(Region.X, Region.Y),
                TextAlignment.TopCenter => new Point(Region.X + Region.Width / 2 - Text.Length / 2, Region.Y),
                TextAlignment.TopRight => new Point(Region.Right - Text.Length, Region.Y),
                TextAlignment.MiddleLeft => new Point(Region.X, Region.Y + Region.Height / 2),
                TextAlignment.MiddleCenter => new Point(Region.X + Region.Width / 2 - Text.Length / 2, Region.Y + Region.Height / 2),
                TextAlignment.MiddleRight => new Point(Region.Right - Text.Length, Region.Y + Region.Height / 2),
                TextAlignment.BottomLeft => new Point(Region.X, Region.Bottom - 1),
                TextAlignment.BottomCenter => new Point(Region.Width / 2 - Text.Length / 2 + 1, Region.Bottom - 1),
                TextAlignment.BottomRight => new Point(Region.Right - Text.Length, Region.Bottom - 1),
                _ => new Point(-1, -1),
            };
        }

        public override void Draw()
        {
            (int x, int y) = CalculatePosition();
            Console.SetCursorPosition(x, y);
            Console.Write(BackgroundColor?.ToBackColStr() + TextColor?.ToForeColStr() + Text);
            Console.Write(ConsoleCodes.RESET);
        }
    }
}
