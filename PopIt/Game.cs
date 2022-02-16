using PopIt.Data;
using PopIt.Exception;
using PopIt.IO;
using PopIt.UI;
using System.Text;

namespace PopIt;
class Game : UIElement
{
    const string
PopItASCIIArt =
@"  ___          ___ _   
| _ \___ _ __|_ _| |_ 
|  _/ _ \ '_ \| ||  _|
|_| \___/ .__/___|\__|
        |_|           ",
RulesText =
@"Mozgás: Nyíl gombok
Gömböcske benyomása: Szóköz / Bal katt
Következő játékos: Enter
",
        TrophyASCIIArt =
@"   .-=========-.
   \'-=======-'/
   _|   .=.   |_
  ((|  {{1}}  |))
   \|   /|\   |/
    \__ '`' __/
      _`) (`_
    _/_______\_
   /___________\";
    public int PlayerCount { get; private set; }
    public int CurrentPlayer { get; private set; }
    public int RemainingCells { get; private set; }
    private Dictionary<char, Color> ColorMap { get; }
    private Board Board { get; }
    private Point CursorPosition { get; set; }
    private bool Selecting { get; set; }
    private bool ReleaseThread { get; set; }
    /// <summary>
    /// Advances to the next player, while resetting the board PushedNow states
    /// </summary>
    public void NextTurn()
    {
        CurrentPlayer = CurrentPlayer % PlayerCount + 1;
        Board.ResetPushedNow();
        Selecting = false;
    }

    public Game(string boardPath, int playerCount) : this(BoardUtils.CreateFromFile(boardPath), playerCount) { }
    public Game(Board board, int playerCount) : base(new(10, 6, board.Width * 2, board.Height))
    {
        if (playerCount == 0) throw new ArgumentException($"Value of {nameof(playerCount)} cannot be less than 1.");

        PlayerCount = playerCount;
        CurrentPlayer = 1;
        Board = board;
        if (BoardUtils.AreComponentsBroken(Board)) throw new InvalidBoardFormatException("The board cannot contain the same letter in a different, not connected component");
        if (BoardUtils.IsBoardBroken(Board)) throw new InvalidBoardFormatException("The board should be traversable from every point to every other point.");
        CursorPosition = BoardUtils.FindFirstValidPos(Board);
        ColorMap = BoardUtils.CreateColorMap(Board);
        RemainingCells = board.CountCells();
        Selecting = false;
        ReleaseThread = false;
        UIManager.Add(this);
    }

    /// <summary>
    /// Starts the game, which locks the thread until the game is over
    /// </summary>
    public void Run()
    {
        Console.CursorVisible = false;
        Render();
        while (!ReleaseThread) { }
        IOManager.ReadKey();
        UIManager.Remove(this);
    }
    public override void OnKeyDown(ConsoleKey key)
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
        NextTurn();
        if (RemainingCells == 0)
        {
            DoWin();
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

    private void DoWin()
    {
        Console.Clear();
        Console.WriteLine($"Gratulálok {CurrentPlayer}. játékos, győztél!");
        DrawString(TrophyASCIIArt, Region.X, Region.Y);
        ReleaseThread = true;
    }

    public override void OnMouseDown(int x, int y)
    {
        // The console cells are 1x2 so divide by 2 to align hitbox
        x /= 2;
        if (!IsValidPosition(x, y)) return;
        if (Selecting && !IsNeighbourPushedNowWithSameColor(x, y)) return;
        if (!TryPush(x, y)) return;
        CursorPosition = new(x, y);
        Render();
    }
    public override void Render()
    {
        Console.CursorVisible = false;
        DrawString(PopItASCIIArt, Region.X + Board.Width - 12, 0);
        for (int j = 0; j < Board.Height; j++)
        {
            StringBuilder sb = new();
            sb.Append(Color.MAGENTA.ToForeColStr());
            Console.SetCursorPosition(Region.X, Region.Y + j);
            for (int i = 0; i < Board.Width; i++) sb.Append(GetCellTextAt(i, j));
            // This prevents bleeding on resize
            sb.Append(Color.BLACK.ToBackColStr());
            sb.Append(Color.BLACK.ToForeColStr());
            sb.Append('█');
            sb.Append(ConsoleCodes.RESET);
            Console.WriteLine(sb);
        }
        Console.WriteLine(Color.BLACK.ToBackColStr());
        DrawString($"{CurrentPlayer}. játékos", Region.Right + 5, Region.Y);
        DrawString(RulesText, Region.Right + 5, Region.Y + 2);
    }
    public string GetCellTextAt(int x, int y)
    {
        var cell = Board[x, y];
        var col = cell.Char == '.' ? Color.BLACK.ToBackColStr() : Board[x, y].Pushed ? ColorMap[cell.Char].ToBackColStrHI() : ColorMap[cell.Char].ToBackColStr();
        var posMatch = new Point(x, y) == CursorPosition;
        var text = cell.PushedBefore ? posMatch ? "[]" : "##" : posMatch ? "><" : "  ";
        return $"{col}{text}";
    }
    bool IsNeighbourOrSelfPushedNow(int x, int y) =>
        IsValidPosition(x, y) && (Board[x, y].PushedNow || BoardUtils.GetNeighboursAt(Board, x, y).Any(ne => ne.PushedNow));
    bool IsNeighbourPushedNowWithSameColor(int x, int y) =>
        IsValidPosition(x, y) && BoardUtils.GetNeighboursAt(Board, x, y).Any(ne => ne.PushedNow && ne.Char == Board[x, y].Char);
    bool CanStepToFrom(int fx, int fy, int tx, int ty) =>
        IsValidPosition(tx, ty) && (!Selecting || (Board[fx, fy].Char == Board[tx, ty].Char && !Board[tx, ty].PushedBefore && IsNeighbourOrSelfPushedNow(tx, ty)));
    bool IsValidPosition(int x, int y) => BoardUtils.IsInBounds(Board, x, y) && Board[x, y].Char != '.';

}