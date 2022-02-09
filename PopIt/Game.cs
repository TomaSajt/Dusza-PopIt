using PopIt.Data;
using PopIt.Exception;
using System.Text;

namespace PopIt;
class Game
{
    private readonly ColorPair[] colorPairs = { ColorPair.Blue, ColorPair.Red, ColorPair.Green, ColorPair.Yellow };
    private readonly Dictionary<char, ColorPair> colorMap;
    public int PlayerCount { get; private set; }
    public int CurrentPlayer { get; private set; } = 0;
    private Board CurrentBoard { get; set; }
    private Point CursorPosition { get; set; } = new(0, 0);
    public void NextPlayer() => CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;

    public Game(int playerCount = 2, string boardPath = "palya1.txt")
    {
        PlayerCount = playerCount;
        CurrentBoard = BoardUtils.CreateFromFile(boardPath);
        if (!BoardUtils.ValidateBoard(CurrentBoard)) throw new InvalidBoardFormatException("The board cannot contain the same letter in a different component");
        colorMap = BoardUtils.CreateColorMap(CurrentBoard, colorPairs);
    }
    public void Run()
    {
        Console.CursorVisible = false;
        while (true)
        {
            HandleInput();
            Render();
        }
    }
    public void HandleInput()
    {
        var key = Console.ReadKey().Key;
        switch (key)
        {
            case ConsoleKey.W:
            case ConsoleKey.UpArrow:
                break;
            case ConsoleKey.A:
            case ConsoleKey.LeftArrow:
                break;
            case ConsoleKey.S:
            case ConsoleKey.DownArrow:
                break;
            case ConsoleKey.D:
            case ConsoleKey.RightArrow:
                break;
        }
    }
    public void Render()
    {
        Console.Clear();
        StringBuilder sb = new();
        for (int i = 0; i < CurrentBoard.Height; i++)
        {
            for (int j = 0; j < CurrentBoard.Width; j++)
            {
                char ch = CurrentBoard[j, i].Char;
                var col = ch == '.' ? Color.BLACK : CurrentBoard[j, i].Pushed ? colorMap[ch].Light : colorMap[ch].Dark;
                sb.Append($"{col.ToBackColStr()}  ");
            }
            sb.AppendLine();
        }
        sb.Append(ConsoleCodes.RESET);
        Console.Write(sb);

    }



}