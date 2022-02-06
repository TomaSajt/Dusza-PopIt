using PopIt.Data;
using PopIt.Exception;

namespace PopIt;
internal class Game
{
    private ColorPair[] colorPairs = { ColorPair.Blue, ColorPair.Red, ColorPair.Green, ColorPair.Yellow };
    private Dictionary<char, ColorPair> colorMap;
    public int PlayerCount { get; private set; }
    public int CurrentPlayer { get; private set; } = 0;
    private Board CurrentBoard { get; set; }
    private Point CursorPosition { get; set; } = new(0, 0);
    public void NextPlayer() => CurrentPlayer = (CurrentPlayer + 1) % PlayerCount;

    public Game(int playerCount = 2, string boardPath = "palya1.txt")
    {
        PlayerCount = playerCount;
        CurrentBoard = BoardUtils.CreateFromFile(boardPath);
        colorMap = CreateColorMap();
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
    private Dictionary<char, ColorPair> CreateColorMap()
    {
        IEnumerable<Cell> GetNeighbours(int x, int y)
        {
            if (x > 0) yield return CurrentBoard[x - 1, y];
            if (y > 0) yield return CurrentBoard[x, y - 1];
            if (x < CurrentBoard.Width - 1) yield return CurrentBoard[x + 1, y];
            if (y < CurrentBoard.Height - 1) yield return CurrentBoard[x, y + 1];
        }
        Dictionary<char, HashSet<char>> adjDict = new();
        for (int i = 0; i < CurrentBoard.Width; i++)
        {
            for (int j = 0; j < CurrentBoard.Height; j++)
            {
                char ch = CurrentBoard[i, j].Char;
                if (ch == '.') continue;
                if (!adjDict.ContainsKey(ch)) adjDict.Add(ch, new());
                foreach (var nei in GetNeighbours(i, j))
                {
                    if (nei.Char == '.' || nei.Char == ch) continue;
                    adjDict[ch].Add(nei.Char);
                }
            }
        }
        return GetMColoring(adjDict, 4).ToDictionary(x => x.Key, x => colorPairs[x.Value]);
    }
    private static Dictionary<char, int> GetMColoring(Dictionary<char, HashSet<char>> graph, int m)
    {
        Dictionary<char, int> colors = new();
        char[] chars = graph.Keys.ToArray();
        foreach (char ch in graph.Keys) colors.Add(ch, -1);
        if (!Backtrack(graph, 0)) throw new GraphColoringException("Couldn't color graph with the given number of colors");
        return colors;

        bool Backtrack(Dictionary<char, HashSet<char>> graph, int v)
        {
            // If all vertices are assigned a color then return true
            if (v == chars.Length)
                return true;

            // Try different colors for vertex V
            for (int i = 0; i < m; i++)
            {
                // check for assignment safety
                if (IsSafeToColor(chars[v], graph, i))
                {
                    colors[chars[v]] = i;
                    // recursion for checking other vertices
                    if (Backtrack(graph, v + 1))
                        return true;
                    // if color doesnt lead to solution
                    colors[chars[v]] = -1;
                }
            }
            return false;
        }
        bool IsSafeToColor(char ch, Dictionary<char, HashSet<char>> graph, int col) => graph[ch].All(nei => colors[nei] != col);
    }
}