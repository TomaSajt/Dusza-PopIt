using PopIt.Game.Data;

namespace PopIt.Game;
internal class Game
{
    private int CurrentPlayer { get; set; }
    private int PlayerCount { get; }
    private void NextPlayer() => CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;

    public Game()
    {
        CurrentPlayer = 0;
        PlayerCount = 2;
        var board = new Board(10, 20);
    }
}