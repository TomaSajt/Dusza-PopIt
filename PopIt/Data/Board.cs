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
    /// <summary>
    /// Loads a <c>Board</c> from the given path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>The read board from the file</returns>
    /// <exception cref="InvalidBoardFormatException"></exception>
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
    /// <summary>
    /// Saves the board to a file with the given path. Might throw an exception if the file is not accessible.
    /// </summary>
    /// <param name="path">The path to the file</param>
    public void SaveToFile(string path)
    {
        using var sw = new StreamWriter(path);
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++) sw.Write(this[j, i].Char);
            sw.WriteLine();
        }
    }

}
