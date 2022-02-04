namespace PopIt.Game.Data;
internal class Board
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
                cells[i, j] = new(this);
            }
        }
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

    private static Random rand = new();
    public static Board LoadFromFile(string path)
    {
        try
        {
            var lines = File.ReadAllLines(path);
            var height = lines.Length;
            var width = lines[0].Length;
            if (lines.Any(x => x.Length != width)) throw new InvalidDataException("The board was not rectangular");
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
        catch
        {
            throw new InvalidDataException("Couldn't process file");
        }
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
