using PopIt.Data;
using PopIt.Exception;
using PopIt.IO;
using System.Text;

namespace PopIt;
class Game
{
    private readonly ColorPair[] colorPairs = { ColorPair.Blue, ColorPair.Red, ColorPair.Green, ColorPair.Yellow };
    private readonly Dictionary<char, ColorPair> colorMap;
    public int PlayerCount { get; private set; }
    public int CurrentPlayer { get; private set; } = 0;
    private Board Board { get; set; }
    private Point CursorPosition { get; set; } = new(0, 0);
    private Cell HoveredCell { get => Board[CursorPosition.X, CursorPosition.Y]; }
    private bool Selecting { get; set; } = false;
    public void NextPlayer() => CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;

    public Game(int playerCount = 2, string boardPath = "palya2.txt")
    {
        PlayerCount = playerCount;
        Board = BoardUtils.CreateFromFile(boardPath);
        if (!BoardUtils.ValidateBoard(Board)) throw new InvalidBoardFormatException("The board cannot contain the same letter in a different component");
        colorMap = BoardUtils.CreateColorMap(Board, colorPairs);
    }
    public void Run()
    {
        Console.CursorVisible = false;
        IOManager.KeyPressed += HandleInput;
    }
    public void HandleInput(ConsoleKey key)
    {
        int X = CursorPosition.X, Y = CursorPosition.Y;
        switch (key)
        {
            case ConsoleKey.Enter:
                Board.ResetPushedNow();
                Selecting = false;
                break;
            case ConsoleKey.Spacebar:
                if (HoveredCell.Pushed) break;
                Selecting = true;
                HoveredCell.Push();
                break;
            case ConsoleKey.A or ConsoleKey.LeftArrow:
                if (CanStepToFrom(X, Y, X - 1, Y)) X--;
                break;
            case ConsoleKey.D or ConsoleKey.RightArrow:
                if (CanStepToFrom(X, Y, X + 1, Y)) X++;
                break;
            case ConsoleKey.W or ConsoleKey.UpArrow:
                if (CanStepToFrom(X, Y, X, Y - 1)) Y--;
                break;
            case ConsoleKey.S or ConsoleKey.DownArrow:
                if (CanStepToFrom(X, Y, X, Y + 1)) Y++;
                break;
        }
        CursorPosition = new(X, Y);
        Render();
    }
    public void Render()
    {
        Console.SetCursorPosition(0, 0);
        StringBuilder sb = new();
        for (int j = 0; j < Board.Height; j++)
        {
            for (int i = 0; i < Board.Width; i++)
            {
                sb.Append(GetCellTextAt(i, j));
            }
            sb.AppendLine();
        }
        sb.Append(ConsoleCodes.RESET);
        Console.Write(sb);
        Console.WriteLine(CursorPosition);
    }
    public string GetCellTextAt(int x, int y)
    {
        var cell = Board[x, y];
        var col = cell.Char == '.' ? Color.BLACK : Board[x, y].Pushed ? colorMap[cell.Char].Light : colorMap[cell.Char].Dark;
        var posMatch = new Point(x, y) == CursorPosition;
        var text = cell.PushedBefore ? posMatch ? "[]" : "##" : posMatch ? "><" : "  ";
        return $"{col.ToBackColStr()}{text}";
    }
    bool HasNeighbourOrSelfPushedNow(int x, int y)
    {
        if (IsValidPosition(x, y) && Board[x, y].PushedNow) return true;
        if (IsValidPosition(x + 1, y) && Board[x + 1, y].PushedNow) return true;
        if (IsValidPosition(x - 1, y) && Board[x - 1, y].PushedNow) return true;
        if (IsValidPosition(x, y + 1) && Board[x, y + 1].PushedNow) return true;
        if (IsValidPosition(x, y - 1) && Board[x, y - 1].PushedNow) return true;
        return false;
    }
    bool CanStepToFrom(int fx, int fy, int tx, int ty) => IsValidPosition(tx, ty) && (!Selecting || (Board[fx, fy].Char == Board[tx, ty].Char && !Board[tx, ty].PushedBefore && HasNeighbourOrSelfPushedNow(tx,ty)));
    bool IsValidPosition(int x, int y) => x >= 0 && y >= 0 && x < Board.Width && y < Board.Height && Board[x, y].Char != '.';

}