namespace PopIt.Data;
internal record struct ColorPair(ConsoleColor Dark, ConsoleColor Light)
{
    public static ColorPair Red = new(ConsoleColor.DarkRed, ConsoleColor.Red);
    public static ColorPair Blue = new(ConsoleColor.DarkBlue, ConsoleColor.Blue);
    public static ColorPair Green = new(ConsoleColor.DarkGreen, ConsoleColor.Green);
    public static ColorPair Yellow = new(ConsoleColor.DarkYellow, ConsoleColor.Yellow);
    public static ColorPair Magenta = new(ConsoleColor.DarkMagenta, ConsoleColor.Magenta);
}
