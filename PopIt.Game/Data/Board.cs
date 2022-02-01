namespace PopIt.Game.Data;
internal class Board
{
    private readonly Cell[,] cells;
    public int Width { get => cells.GetLength(0); }
    public int Height { get => cells.GetLength(1); }
    public Cell this[int x, int y] { get => cells[x, y]; set => cells[x, y] = value; }
    public Board(int width, int height)
    {
        cells = new Cell[width, height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                cells[i, j] = default;
            }
        }
    }
    public void ResetPushedNow()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                cells[i, j].PushedNow = false;
            }
        }
    }
}
