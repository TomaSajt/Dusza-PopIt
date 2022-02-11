using PopIt;
using PopIt.Data;
using PopIt.IO;
using PopIt.UI;
using PopIt.UI.Components;

namespace PopIt
{
    static class Program
    {
        static Random rand = new();
        public static void Main(string[] args)
        {
            IOManager.Run();
            Console.CursorVisible = false;
            var globalParent = UIElement.CreateGlobalParent();
            var button = new Button(
                new Button.Properties
                {
                    BaseColor = new Color(0x18, 0x15, 0xBF),
                    HoverColor = new Color(0x25, 0x22, 0xF0),
                    PressedColor = new Color(0x16, 0x14, 0x85),
                    TextColor = Color.WHITE,
                    Text = "Button"
                },
                globalParent,
                new Rectangle(11, 5, 12, 3)
            );
            int counter = 0;
            button.Clicked += () =>
            {
                counter++;
                Console.SetCursorPosition(10, 9);
                Console.WriteLine($"Clicked {counter} time(s)");
            };
            button.Render();
        }
    }
}
