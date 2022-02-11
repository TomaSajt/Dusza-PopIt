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
    public int CurrentPlayer { get; private set; }
    public int RemainingCells { get; private set; }
    private Board Board { get; set; }
    private Point CursorPosition { get; set; }
    private bool Selecting { get; set; }
    private bool Release { get; set; }
    public void NextPlayer() => CurrentPlayer = CurrentPlayer % PlayerCount + 1;

    public Game(string boardPath, int playerCount) : this(Board.CreateFromFile(boardPath), playerCount) { }
    public Game(Board board, int playerCount)
    {
        if (playerCount == 0) throw new ArgumentException($"Value of {nameof(playerCount)} cannot be less than 1.");

        PlayerCount = playerCount;
        CurrentPlayer = 1;
        Board = board;
        if (!BoardUtils.CheckComponentsNotBroken(Board)) throw new InvalidBoardFormatException("The board cannot contain the same letter in a different, not connected component");
        //TODO: Validate whether or not every cell is reachable from every other point

        colorMap = BoardUtils.CreateColorMap(Board, colorPairs);
        CursorPosition = FindFirstValidPos();
        RemainingCells = board.CountCells();
        Selecting = false;
        Release = false;
    }
    
    /// <summary>
    /// Finds the first valid position for the cursor. This function scans from top-to-bottom left-to-right.
    /// </summary>
    /// <returns>A <see cref="Point"></see> with the coordinates</returns>
    /// <exception cref="InvalidBoardFormatException"></exception>
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
    /// <summary>
    /// Starts the game, which locks the thread until the game is over
    /// </summary>
    public void Run()
    {
        IOManager.Run();
        Console.CursorVisible = false;
        Render();
        IOManager.KeyPressed += HandleKeyboardInput;
        IOManager.LeftMouseDown += HandleMouseClcik;
        while (!Release) { }
        IOManager.Stop();
        Console.ReadKey();
    }
    public void HandleMouseClcik(int x, int y)
    {
        x /= 2;
        if (!IsValidPosition(x, y)) return;
        if (Selecting && !IsNeighbourPushedNowWithSameColor(x, y)) return;
        if (!TryPush(x, y)) return;
        CursorPosition = new(x, y);
        Render();
    }
    public void HandleKeyboardInput(ConsoleKey key)
    {
        int X = CursorPosition.X, Y = CursorPosition.Y;


        switch (key)
        {
            case ConsoleKey.Enter:
                if (!TrySubmit()) return;
                break;
            case ConsoleKey.Spacebar:
                if (!TryPush(CursorPosition.X, CursorPosition.Y)) return;
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
            default:
                return;
        }
        CursorPosition = new(X, Y);
        Render();
    }

    private bool TrySubmit()
    {
        if (!Selecting) return false;
        Board.ResetPushedNow();
        Selecting = false;
        NextPlayer();
        if (RemainingCells == 0)
        {
            HandleWin();
            return false;
        }
        return true;
    }
    private bool TryPush(int x, int y)
    {
        if (Board[x, y].Pushed) return false;
        Selecting = true;
        RemainingCells--;
        Board[x, y].Push();
        return true;
    }

    private void HandleWin()
    {
        IOManager.KeyPressed -= HandleKeyboardInput;
        Console.Clear();
        Console.WriteLine($"Gratulálok {CurrentPlayer}. játékos, győztél!");
        Release = true;
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
        Console.WriteLine();
        Console.WriteLine($"{CurrentPlayer}. játékos");
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