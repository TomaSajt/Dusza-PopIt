using PopIt.Data;
using PopIt.Exception;

namespace PopIt;

/// <summary>
/// An utility class which has useful functions for working with <see cref="Board"/>s.
/// </summary>
public static class BoardUtils
{
    private static readonly Random rand = new();
    /// <summary>
    /// Loads a <see cref="Board"/> from the given path.
    /// </summary>
    /// <param name="path">The path of the new file</param>
    /// <returns>The <see cref="Board"/> read from the file</returns>
    /// <exception cref="InvalidBoardFormatException"></exception>
    public static Board CreateFromFile(string path)
    {
        var lines = File.ReadAllLines(path);
        var height = lines.Length;
        var width = lines[0].Length;
        if (lines.Any(x => x.Length != width)) throw new InvalidBoardFormatException("The read board was not rectangular");
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
    /// Saves the given board to a file with the given path. Will override the file if it exists. Might throw an exception if the file is not accessible.
    /// </summary>
    /// <param name="board">The board to save</param>
    /// <param name="path">The path of the new file</param>
    public static void SaveToFile(Board board, string path)
    {
        using var sw = new StreamWriter(path, false);
        for (int i = 0; i < board.Height; i++)
        {
            for (int j = 0; j < board.Width; j++) sw.Write(board[j, i].Char);
            sw.WriteLine();
        }
    }
    /// <summary>
    /// Generates a board with the given dimensions and bends.
    /// The board will be completely filled up with chars, so there won't be any '.'-s.
    /// The <paramref name="bends"/> parameter dictates how many turns the first generated region/component is going to have.
    /// </summary>
    /// <param name="w">The width of the board</param>
    /// <param name="h">The height of the board</param>
    /// <param name="bends">The numer of bends the board will have</param>
    /// <returns></returns>
    public static Board GenerateBoard(int w, int h, int bends)
    {
        if (w <= 0 || h <= 0) throw new InvalidBoardFormatException("A generated board has to have atleast 1 cell");
        Point GetRandOffset() => rand.Next(2) == 0 ? new(0, 1) : new(1, 0);
        bool InBound(Board board, int x, int y) => 0 <= x && x < w && 0 <= y && y < h && board[x, y].Char == '.';
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
                        if (!InBound(board, nx, ny)) break;
                        board[nx, ny].Char = c;
                        next = true;
                    }
                    if (next) c++;
                }
            }
            if (c <= 'z') return board;
        }


    }

    public static IEnumerable<Point> GetNeighboursPositions(Board board, int x, int y)
    {
        if (x > 0) yield return new(x - 1, y);
        if (y > 0) yield return new(x, y - 1);
        if (x < board.Width - 1) yield return new(x + 1, y);
        if (y < board.Height - 1) yield return new(x, y + 1);
    }
    public static IEnumerable<Cell> GetNeighboursAt(Board board, int x, int y) => GetNeighboursPositions(board, x, y).Select(p => board[p.X, p.Y]);

    /// <summary>
    /// Checks whether or not there are any dijoint components in the given <see cref="Board"/>.
    /// </summary>
    /// <param name="board">The board to check</param>
    /// <returns><c>true</c> if the board is broken, <c>false</c> if not</returns>
    public static bool AreComponentsBroken(Board board)
    {
        HashSet<char> seen = new();
        var vis = new bool[board.Width, board.Height];
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                char ch = board[i, j].Char;
                if (vis[i, j] || ch == '.') continue;
                if (seen.Contains(ch)) return true;
                seen.Add(ch);
                FillFrom(i, j);
            }
        }
        return false;
        void FillFrom(int x, int y)
        {
            var queue = new Queue<Point>();
            queue.Enqueue(new(x, y));
            char ch = board[x, y].Char;
            vis[x, y] = true;
            while (queue.Any())
            {
                var curr = queue.Dequeue();
                foreach (var nei in GetNeighboursPositions(board, curr.X, curr.Y))
                {
                    if (board[nei.X, nei.Y].Char != ch || vis[nei.X, nei.Y]) continue;
                    vis[nei.X, nei.Y] = true;
                    queue.Enqueue(nei);
                }
            }
        }
    }
    public static bool IsBoardBroken(Board board)
    {
        var p = FindFirstValidPos(board);
        var queue = new Queue<Point>();
        var vis = new bool[board.Width, board.Height];
        queue.Enqueue(p);
        vis[p.X, p.Y] = true;
        while (queue.Any())
        {
            var curr = queue.Dequeue();
            foreach (var nei in GetNeighboursPositions(board, curr.X, curr.Y))
            {
                if (board[nei.X, nei.Y].Char == '.' || vis[nei.X, nei.Y]) continue;
                vis[nei.X, nei.Y] = true;
                queue.Enqueue(nei);
            }
        }
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                if (board[i, j].Char != '.' && !vis[i, j]) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Finds the first valid position for the cursor. This function scans from top-to-bottom left-to-right.
    /// </summary>
    /// <returns>A <see cref="Point"></see> with the coordinates</returns>
    /// <exception cref="InvalidBoardFormatException"></exception>
    public static Point FindFirstValidPos(Board board)
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                if (board[i, j].Char != '.') return new(i, j);
            }
        }
        throw new InvalidBoardFormatException("The board has to contain at least one valid cell");
    }

    /// <summary>
    /// Takes in a <see cref="Board"/> and returns a valid coloring using the given array of <see cref="ColorPair"/>s.
    /// </summary>
    /// <param name="board"></param>
    /// <param name="colorPairs"></param>
    /// <returns></returns>
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
                foreach (var nei in GetNeighboursAt(board, i, j))
                {
                    if (nei.Char == '.' || nei.Char == ch) continue;
                    adjDict[ch].Add(nei.Char);
                }
            }
        }
        return GetMColoring(adjDict, colorPairs.Length).ToDictionary(x => x.Key, x => colorPairs[x.Value]);
    }

    /// <summary>
    /// This function uses backtracking to create the m-coloring of an undirected graph.
    /// </summary>
    /// <typeparam name="T">The type of the graph nodes</typeparam>
    /// <param name="graph">A graph in the form of an adjecency list</param>
    /// <param name="m">The number of different colors to use</param>
    /// <returns>A <see cref="Dictionary{T, int}"/>, which maps a node of the graph to a color index.</returns>
    /// <exception cref="GraphColoringException"/>
    public static Dictionary<T, int> GetMColoring<T, U>(Dictionary<T, U> graph, int m) where T : notnull where U : IEnumerable<T>
    {
        if (m < 4) throw new GraphColoringException($"{nameof(m)} cannot be less than 4");
        Dictionary<T, int> colors = new();
        T[] chars = graph.Keys.ToArray();
        foreach (T ch in graph.Keys) colors.Add(ch, -1);
        if (!Backtrack(graph, 0)) throw new GraphColoringException($"Couldn't color graph with the {m} colors");
        return colors;

        bool Backtrack(Dictionary<T, U> graph, int v)
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
        bool IsSafeToColor(T ch, Dictionary<T, U> graph, int col) => graph[ch].All(nei => colors[nei] != col);
    }
}
