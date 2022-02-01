namespace PopIt.Game.Data;
internal record struct Point(int X, int Y)
{
    public static implicit operator Point((int x, int y) t) => new(t.x, t.y);
}

