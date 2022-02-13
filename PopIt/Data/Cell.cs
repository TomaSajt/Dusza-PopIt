namespace PopIt.Data;
public class Cell
{
    public char Char { get; set; } = '.';
    public bool Pushed { get; private set; } = false;
    public bool PushedNow { get; private set; } = false;
    public bool PushedBefore { get => Pushed && !PushedNow; }
    public void Push() => Pushed = PushedNow = true;
    public void ResetPushedNow() => PushedNow = false;
}
