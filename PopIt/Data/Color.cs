namespace PopIt.Data;
public struct Color
{
    public int ID { get; }
    private Color(int id) => ID = id;

    public static readonly Color BLACK = new(0);
    public static readonly Color RED = new(1);
    public static readonly Color GREEN = new(2);
    public static readonly Color YELLOW = new(3);
    public static readonly Color BLUE = new(4);
    public static readonly Color MAGENTA = new(5);
    public static readonly Color CYAN = new(6);
    public static readonly Color WHITE = new(7);
    public string ToForeColStr() => $"\u001b[{30 + ID}m";
    public string ToBackColStr() => $"\u001b[{40 + ID}m";
    public string ToForeColStrHI() => $"\u001b[{90 + ID}m";
    public string ToBackColStrHI() => $"\u001b[{100 + ID}m";
}

