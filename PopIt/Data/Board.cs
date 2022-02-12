using PopIt.Exception;

namespace PopIt.Data;
class Board
{
    private readonly Cell[,] cells;
    public int Width { get => cells.GetLength(0); }
    public int Height { get => cells.GetLength(1); }
    public int PushedCount { get; set; } = 0;

    public Cell this[int x, int y] { get => cells[x, y]; }
    public Board(int width, int height)
    {
        cells = new Cell[width, height];
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                cells[i, j] = new();
            }
        }
    }
    public IEnumerable<Cell> GetNeighboursAt(int x, int y)
    {
        if (x > 0) yield return this[x - 1, y];
        if (y > 0) yield return this[x, y - 1];
        if (x < Width - 1) yield return this[x + 1, y];
        if (y < Height - 1) yield return this[x, y + 1];
    }
    /// <summary>
    /// Utility function, which iterates through every cell, and resets their <see cref="F:Cell.PushedNow"/> state.
    /// </summary>
    public void ResetPushedNow()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                this[i, j].ResetPushedNow();
            }
        }
    }

    public int CountCells()
    {
        int count = 0;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (this[i, j].Char != ' ') count++;
            }
        }
        return count;
    }
}
