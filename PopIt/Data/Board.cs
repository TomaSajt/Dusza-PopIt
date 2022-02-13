namespace PopIt.Data;
public class Board
{
    private readonly Cell[,] cells;
    public int Width { get => cells.GetLength(0); }
    public int Height { get => cells.GetLength(1); }

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
    /// <summary>
    /// Utility function, which iterates through every cell, and resets their PushedNow state.
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
