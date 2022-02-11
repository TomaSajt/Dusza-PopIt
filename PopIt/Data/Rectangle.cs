namespace PopIt.Data;
record struct Rectangle(int X, int Y, int Width, int Height)
{
    public int Top { get => Y; }
    public int Bottom { get => Y + Height; }
    public int Left { get => X; }
    public int Right { get => X + Width; }

    public bool Contains(int x, int y) => x >= Left && x < Right && y >= Top && y < Bottom;
}
