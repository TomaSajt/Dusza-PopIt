using PopIt.Game.Data;
using System.Text;

namespace PopIt.Game;
internal class Game
{
    public int PlayerCount { get; init; } = 2;
    private int CurrentPlayer { get; set; } = 0;
    private Board CurrentBoard { get; set; }
    private void NextPlayer() => CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;

    public Game()
    {
        CurrentBoard = BoardUtils.CreateFromFile("palya1.txt");
    }
    public void Render()
    {
        StringBuilder sb = new();
        for (int i = 0; i < CurrentBoard.Height; i++)
        {
            for (int j = 0; j < CurrentBoard.Width; j++)
            {
                sb.Append(CurrentBoard[j, i].Char);
            }
            sb.AppendLine();
        }
        Console.WriteLine(sb);
    }
}