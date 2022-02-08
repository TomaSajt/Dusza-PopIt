namespace PopIt.Data;

static class ConsoleCodes
{
    public const string RESET = "\u001b[0m";
    public const string BOLD = "\u001b[1m";
    public const string UNDERLINE = "\u001b[4m";
    private static string ColString(int r, int g, int b, bool fore) => $"\u001b[{(fore ? '3' : '4')}8;2;{r};{g};{b}m";
    private static string ColString(Color color, bool fore) => ColString(color.R, color.G, color.B, fore);
    public static string BackColorStr(Color color) => ColString(color, false);
    public static string ForeColorStr(Color color) => ColString(color, true);
}


