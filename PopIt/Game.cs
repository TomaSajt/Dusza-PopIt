using PopIt.Data;
using PopIt.Exception;

namespace PopIt;
internal class Game
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
    public void Render()
    {
        /*
        StringBuilder sb = new();
        for (int i = 0; i < CurrentBoard.Height; i++)
        {
            for (int j = 0; j < CurrentBoard.Width; j++)
            {
                sb.Append($"\u001b[43m{CurrentBoard[j, i].Char}");
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb);*/
        for (int i = 0; i < CurrentBoard.Height; i++)
        {
            for (int j = 0; j < CurrentBoard.Width; j++)
            {
                char ch = CurrentBoard[j, i].Char;
                Console.BackgroundColor = ch == '.' ? ConsoleColor.Black : CurrentBoard[j, i].Pushed ? colorMap[ch].Light : colorMap[ch].Dark;
                Console.Write($"  ");
            }
            Console.WriteLine();
        }
        Console.ResetColor();

    }
    
    
}