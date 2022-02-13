namespace PopIt.Data;
public record struct Color(byte R, byte G, byte B)
{
    public static readonly Color BLACK = new(12, 12, 12);
    public static readonly Color DARK_RED = new(197, 15, 31);
    public static readonly Color DARK_GREEN = new(19, 161, 14);
    public static readonly Color DARK_YELLOW = new(193, 156, 0);
    public static readonly Color DARK_BLUE = new(0, 55, 218);
    public static readonly Color DARK_MAGENTA = new(136, 23, 152);
    public static readonly Color DARK_CYAN = new(58, 150, 221);
    public static readonly Color LIGHT_GRAY = new(204, 204, 204);
    public static readonly Color GRAY = new(118, 118, 118);
    public static readonly Color RED = new(231, 72, 86);
    public static readonly Color GREEN = new(22, 198, 12);
    public static readonly Color YELLOW = new(249, 241, 165);
    public static readonly Color BLUE = new(59, 120, 255);
    public static readonly Color MAGENTA = new(180, 0, 158);
    public static readonly Color CYAN = new(97, 214, 214);
    public static readonly Color WHITE = new(242, 242, 242);
    public string ToBackColStr() => ConsoleCodes.BackColorStr(this);
    public string ToForeColStr() => ConsoleCodes.ForeColorStr(this);
}

