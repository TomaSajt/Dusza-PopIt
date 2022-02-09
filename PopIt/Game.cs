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
    public int CurrentPlayer { get; private set; } = 1;
    public int RemainingCells { get; private set; }
    private Board Board { get; set; }
    private Point CursorPosition { get; set; }
    private bool Selecting { get; set; } = false;
    private bool Return { get; set; } = false;
    public void NextPlayer() => CurrentPlayer = CurrentPlayer % PlayerCount + 1;

    public Game(string boardPath, int playerCount = 2) : this(BoardUtils.CreateFromFile(boardPath), playerCount) { }
    public Game(Board board, int playerCount = 2)
    {
        if (playerCount == 0) throw new ArgumentException($"Value of {nameof(playerCount)} cannot be less than 1.");
        PlayerCount = playerCount;
        Board = board;
        if (!BoardUtils.CheckComponentsNotBroken(Board)) throw new InvalidBoardFormatException("The board cannot contain the same letter in a different, not connected component");
        colorMap = BoardUtils.CreateColorMap(Board, colorPairs);
        CursorPosition = FindFirstValidPos();
        RemainingCells = CountCells();
    }
    private int CountCells()
    {
        int count = 0;
        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                if (Board[i, j].Char != '.') count++;
            }
        }
        return count;
    }
    private Point FindFirstValidPos()
    {
        for (int i = 0; i < Board.Width; i++)
        {
            for (int j = 0; j < Board.Height; j++)
            {
                if (Board[i, j].Char != '.') return new(i, j);
            }
        }
        throw new InvalidBoardFormatException("The board has to contain at least 1 valid cell");
    }
    public void Run()
    {
        IOManager.Run();
        Console.CursorVisible = false;
        Render();
        IOManager.KeyPressed += HandleKeyboardInput;
        IOManager.LeftMouseDown += HandleMouseClcik;
        while (!Return) { }
    }
    public void HandleMouseClcik(short x, short y)
    {
        x /= 2;
        if (IsValidPosition(x, y))
        {
            if (!Selecting || IsNeighbourPushedNowWithSameColor(x, y))
            {
                if (Board[x, y].Pushed) return;
                Selecting = true;
                RemainingCells--;
                Board[x, y].Push();
                CursorPosition = new(x, y);
                Render();
            }
        }
    }
    public void HandleKeyboardInput(ConsoleKey key)
    {
        int X = CursorPosition.X, Y = CursorPosition.Y;


        switch (key)
        {
            case ConsoleKey.Enter:
                if (!Selecting) return;
                Board.ResetPushedNow();
                Selecting = false;
                NextPlayer();
                if (RemainingCells == 0)
                {
                    HandleWin();
                    return;
                }

                break;
            case ConsoleKey.Spacebar:
                if (Board[CursorPosition.X, CursorPosition.Y].Pushed) return;
                Selecting = true;
                RemainingCells--;
                Board[CursorPosition.X, CursorPosition.Y].Push();
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

    private void HandleWin()
    {
        IOManager.KeyPressed -= HandleKeyboardInput;
        Console.Clear();
        Console.WriteLine($"Gratulálok {CurrentPlayer}. játékos, győztél!");
        IOManager.Stop();
        Console.ReadKey();
        Return = true;
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
        Console.WriteLine(CurrentPlayer);
    }
    public string GetCellTextAt(int x, int y)
    {
        var cell = Board[x, y];
        var col = cell.Char == '.' ? Color.BLACK : Board[x, y].Pushed ? colorMap[cell.Char].Light : colorMap[cell.Char].Dark;
        var posMatch = new Point(x, y) == CursorPosition;
        var text = cell.PushedBefore ? posMatch ? "[]" : "##" : posMatch ? "><" : "  ";
        return $"{col.ToBackColStr()}{text}";
    }
    bool IsNeighbourOrSelfPushedNow(int x, int y) =>
        (IsValidPosition(x, y) && Board[x, y].PushedNow) ||
        (IsValidPosition(x + 1, y) && Board[x + 1, y].PushedNow) ||
        (IsValidPosition(x - 1, y) && Board[x - 1, y].PushedNow) ||
        (IsValidPosition(x, y + 1) && Board[x, y + 1].PushedNow) ||
        (IsValidPosition(x, y - 1) && Board[x, y - 1].PushedNow);
    bool IsNeighbourPushedNowWithSameColor(int x, int y) =>
        IsValidPosition(x, y) && (
            (IsValidPosition(x + 1, y) && Board[x + 1, y].PushedNow && Board[x, y].Char == Board[x + 1, y].Char) ||
            (IsValidPosition(x - 1, y) && Board[x - 1, y].PushedNow && Board[x, y].Char == Board[x - 1, y].Char) ||
            (IsValidPosition(x, y + 1) && Board[x, y + 1].PushedNow && Board[x, y].Char == Board[x, y + 1].Char) ||
            (IsValidPosition(x, y - 1) && Board[x, y - 1].PushedNow && Board[x, y].Char == Board[x, y - 1].Char)
        );
    bool CanStepToFrom(int fx, int fy, int tx, int ty) => IsValidPosition(tx, ty) && (!Selecting || (Board[fx, fy].Char == Board[tx, ty].Char && !Board[tx, ty].PushedBefore && IsNeighbourOrSelfPushedNow(tx, ty)));
    bool IsValidPosition(int x, int y) => x >= 0 && y >= 0 && x < Board.Width && y < Board.Height && Board[x, y].Char != '.';

}