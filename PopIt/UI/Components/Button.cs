using PopIt.Data;

namespace PopIt.UI.Components
{
    internal class Button : UIElement
    {
        public Color BaseColor { get; set; }
        public Color HoverColor { get; set; }
        public Color PressedColor { get; set; }
        public Padding Padding { get; set; }
        private Color CurrentColor { get; set; }
        public event Action? Clicked;
        public Label Label { get; private set; }
        private bool clicking = false;

        public struct Properties
        {
            public Color BaseColor;
            public Color HoverColor;
            public Color PressedColor;
            public string Text = "";
            public Color? TextColor = null;
            public Color? TextBackgroundColor = null;
            public Label.TextAlignment TextAlignment = Label.TextAlignment.MiddleCenter;
            public Padding Padding = new(0, 0, 0, 0);
        }

        public Button(Properties props, UIElement parent, Rectangle region) : base(parent, region)
        {
            BaseColor = props.BaseColor;
            HoverColor = props.HoverColor;
            PressedColor = props.PressedColor;
            Padding = props.Padding;
            Label = new Label(new Label.Properties
            {
                Text = props.Text,
                Alignment = props.TextAlignment,
                TextColor = props.TextColor
            },
            this,
            new Rectangle(Region.Left + Padding.Left, Region.Top + Padding.Top, Region.Width - Padding.Right, Region.Height - Padding.Bottom)
            );
            CurrentColor = BaseColor;
        }

        public override void Draw()
        {
            Console.Write(CurrentColor.ToBackColStr());
            Label.BackgroundColor = CurrentColor;
            for (int i = 0; i < Region.Height; i++)
            {
                Console.SetCursorPosition(Region.X, Region.Y + i);
                Console.Write(new string(' ', Region.Width));
            }
            Console.Write(ConsoleCodes.RESET);
        }

        protected override void OnMouseEnter()
        {
            CurrentColor = HoverColor;
            Render();
        }

        protected override void OnMouseLeave()
        {
            clicking = false;
            CurrentColor = BaseColor;
            Render();
        }

        protected override void OnMouseDown(int x, int y)
        {
            clicking = true;
            CurrentColor = PressedColor;
            Render();
        }

        protected override void OnMouseUp(int x, int y)
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
}
