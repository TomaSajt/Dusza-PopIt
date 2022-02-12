using PopIt.Data;
using PopIt.Exception;

namespace PopIt;
static class BoardUtils
{
    private static readonly Random rand = new();
    /// <summary>
    /// Loads a <c>Board</c> from the given path.
    /// </summary>
    /// <param name="path"></param>
    /// <returns>The read board from the file</returns>
    /// <exception cref="InvalidBoardFormatException"></exception>
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
    /// Saves the given board to a file with the given path. Might throw an exception if the file is not accessible.
    /// </summary>
    /// <param name="board">The board to save</param>
    /// <param name="path">The path of the new file</param>
    public static void SaveToFile(Board board, string path)
    {
        using var sw = new StreamWriter(path);
        for (int i = 0; i < board.Height; i++)
        {
            for (int j = 0; j < board.Width; j++) sw.Write(board[j, i].Char);
            sw.WriteLine();
        }
    }
    /// <summary>
    /// Generates a board with the given dimensions and bends.
    /// The board will be completely filled up with chars, so there won't be any '.'-s.<
    /// The <paramref name="bends"/> parameter dictates how many turns the first generated region/component is going to have.
    /// </summary>
    /// <param name="w">The width of the board</param>
    /// <param name="h">The height of the board</param>
    /// <param name="bends">The numer of bends the board will have</param>
    /// <returns></returns>
    public static Board GenerateBoard(int w, int h, int bends)
    {
        Point GetRandOffset() => rand.Next(2) == 0 ? new(0, 1) : new(1, 0);
        bool InBound(int x, int y) => 0 <= x && x < w && 0 <= y && y < h;
        while (true)
        {
            char c = 'a';
            var board = new Board(w, h);
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (board[i, j].Char != '.') continue;
                    var offset = GetRandOffset();
                    bool next = false;
                    for (int k = 0; k < 5; k++)
                    {
                        int nx = i + offset.X * k;
                        int ny = j + offset.Y * k;
                        if (!InBound(nx, ny)) break;
                        board[nx, ny].Char = c;
                        next = true;
                    }
                    if(next )c++;
                }
            }
            if (c <= 'z') return board;
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
            foreach (int i in Enumerable.Range(0, m).OrderBy(x => rand.Next()))
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
