namespace PopIt.Data;
public record struct ColorPair(Color Dark, Color Light)
{
    public static readonly ColorPair Red = new(Color.DARK_RED, Color.RED);
    public static readonly ColorPair Blue = new(Color.DARK_BLUE, Color.BLUE);
    public static readonly ColorPair Cyan = new(Color.DARK_CYAN, Color.CYAN);
    public static readonly ColorPair Green = new(Color.DARK_GREEN, Color.GREEN);
    public static readonly ColorPair Yellow = new(Color.DARK_YELLOW, Color.YELLOW);
    public static readonly ColorPair Magenta = new(Color.DARK_MAGENTA, Color.MAGENTA);
}
