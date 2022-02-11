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
    public static Board CreateFromFile(string path)
    {
        var lines = File.ReadAllLines(path);
        var height = lines.Length;
        var width = lines[0].Length;
        if (lines.Any(x => x.Length != width)) throw new InvalidBoardFormatException("The board was not rectangular");
        var board = new Board(width, height);
        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                board[j, i].Char = lines[i][j];
            }
        }
        return board;
    }
    public void SaveToFile(string path)
    {
        using var sw = new StreamWriter(path);
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++) sw.Write(this[j, i]);
            sw.WriteLine();
        }
    }

}
