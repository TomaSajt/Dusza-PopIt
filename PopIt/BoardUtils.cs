using PopIt.Data;
using PopIt.Exception;

namespace PopIt;
static class BoardUtils
{
    public static Board CreateFromFile(string path)
    {
        var lines = File.ReadAllLines(path);
        var height = lines.Length;
        var width = lines[0].Length;
        if (lines.Any(x => x.Length != width)) throw new InvalidBoardFormatException("The board was not rectangular");
        var board = new Board(width, height);
        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                board[j, i].Char = lines[i][j];
            }
        }
        return board;
    }
    /// <summary>
    /// Saves the given board to a file with the given path
    /// </summary>
    /// <param name="board"></param>
    /// <param name="path"></param>
    public static void SaveToFile(Board board, string path)
    {
        using var sw = new StreamWriter(path);
        for (int i = 0; i < board.Height; i++)
        {
            for (int j = 0; j < board.Width; j++) sw.Write(board[j, i]);
            sw.WriteLine();
        }
    }
    public static bool CheckComponentsNotBroken(Board board)
    {
        HashSet<char> seen = new();
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                char ch = board[i, j].Char;
                if (ch == '.') continue;
                if (seen.Add(ch)) continue;
                if (board.GetNeighboursAt(i, j).All(x => x.Char != ch)) return false;
            }
        }
        return true;
    }
    public static Dictionary<char, ColorPair> CreateColorMap(Board board, ColorPair[] colorPairs)
    {
        Dictionary<char, HashSet<char>> adjDict = new();
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                char ch = board[i, j].Char;
                if (ch == '.') continue;
                if (!adjDict.ContainsKey(ch)) adjDict.Add(ch, new());
                foreach (var nei in board.GetNeighboursAt(i, j))
                {
                    if (nei.Char == '.' || nei.Char == ch) continue;
                    adjDict[ch].Add(nei.Char);
                }
            }
        }
        return GetMColoring(adjDict, 4).ToDictionary(x => x.Key, x => colorPairs[x.Value]);
    }
    public static Dictionary<char, int> GetMColoring(Dictionary<char, HashSet<char>> graph, int m)
    {
        Dictionary<char, int> colors = new();
        char[] chars = graph.Keys.ToArray();
        foreach (char ch in graph.Keys) colors.Add(ch, -1);
        if (!Backtrack(graph, 0)) throw new GraphColoringException($"Couldn't color graph with the {m} colors");
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
