namespace PopIt.Data;
public record struct Point(int X, int Y)
{
    public static Point operator +(Point p1, Point p2) => new(p1.X + p2.X, p1.Y + p2.Y);
    public static Point operator -(Point p) => new(-p.X,-p.Y);
    public static Point operator -(Point p1, Point p2) => p1 + -p2;
}