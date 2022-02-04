using PopIt.Game.Data;

namespace PopIt.Game;
static internal class BoardUtils
{
    private static Random rand = new();
    public static Board CreateFromFile(string path)
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
    public static void SaveToFile(Board board, string path)
    {
        using var sw = new StreamWriter(path);
        for (int i = 0; i < board.Height; i++)
        {
            for (int j = 0; j < board.Width; j++) sw.Write(board[j, i]);
            sw.WriteLine();
        }
    }
}
