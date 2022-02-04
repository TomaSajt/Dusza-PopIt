namespace PopIt.Game.Data;
internal class Cell
{
    private Board board;
    public Cell(Board board)
    {
        this.board = board;
    }
    public char Char { get; set; } = '.';
    public bool Pushed { get; private set; } = false;
    public bool PushedNow { get; private set; } = false;
    public void Push() => Pushed = PushedNow = true;
    public void ResetPushedNow() => PushedNow = false;

}
