using PopIt.Data;
using PopIt.Exception;

namespace PopIt;

/// <summary>
/// An utility class which has useful functions for working with <see cref="Board"/>s.
/// </summary>
public static class BoardUtils
{
    private const int maxRetries = 100;
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
    /// Saves the given board to a file with the given path.
    /// </summary>
    /// <remarks>Will override the file if it exists.</remarks>
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
    /// The <paramref name="bends"/> parameter dictates how many turns the first generated region/component is going to have.
    /// </summary>
    /// <remarks>The board will be completely filled up with valid chars.</remarks>
    /// <param name="w">The width of the board</param>
    /// <param name="h">The height of the board</param>
    /// <param name="bends">The numer of bends the board will have</param>
    /// <returns>The generated board</returns>
    public static Board GenerateBoard(int w, int h, int bends)
    {
        if (w < 4 || h < 4) throw new InvalidBoardFormatException("A generated board has to be at least 4 by 4");
        if (w > 10 || h > 10) throw new InvalidBoardFormatException("A generated board can only be at most 10 by 10");
        static Point GetRandOffset() => rand.Next(2) == 0 ? new(0, 1) : new(1, 0);
        for (int iters = 0; iters < maxRetries; iters++)
        {
            char c = 'a';
            var board = new Board(w, h);
            if (bends > 0)
            {
                var bentPath = FindPathOfNBends(bends, new(rand.Next(1, w - 1), rand.Next(1, h - 1)), w, h);
                if (bentPath is null) throw new PathfindingException($"Couldn't find a path with {bends} bends.");
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h; j++)
                    {
                        if (bentPath.Contains(new(i, j))) board[i, j].Char = c;
                    }
                }
                c++;
            }
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
                        if (!IsInBounds(board, nx, ny) || board[nx, ny].Char != '.') break;
                        board[nx, ny].Char = c;
                        next = true;
                    }
                    if (next) c++;
                }
            }
            if (c <= 'z') return board;
        }
        throw new InvalidBoardFormatException($"Couldn't generate a board of size ({w};{h}) after {maxRetries} tries.")
    }

    /// <summary>
    /// Finds a path, bound by a given area, which has exactly <paramref name="bends"/> number of bends.
    /// </summary>
    /// <remarks>This method uses backtracking, so it might take a long time in certain situations.</remarks>
    /// <param name="bends"></param>
    /// <param name="startPoint"></param>
    /// <param name="w">The width of the area</param>
    /// <param name="h">The height of the area</param>
    /// <returns>A <see cref="HashSet(Point)"/> containing the points making up the path</returns>
    public static HashSet<Point>? FindPathOfNBends(int bends, Point startPoint, int w, int h)
    {
        List<Point> cardDirs = new() { new(0, 1), new(1, 0), new(0, -1), new(-1, 0) };
        HashSet<Point> visited = new();
        // Track visited state so we don't touch/cross.
        visited.Add(startPoint);
        foreach (var direction in cardDirs.OrderBy(x => rand.Next()))
        {
            // Check if there's a path of the desired number of bends in this direction.
            var res = Extend(bends, startPoint, direction);
            if (res == null)
                continue;
            res.Add(startPoint);
            // Finish & return the path.
            return res;
        }

        // Search failed. No such path exists!
        return null;


        HashSet<Point>? Extend(int remainingBends, Point epicstartPoint, Point currentDirection)
        {
            HashSet<Point> path = new();
            // Proceed in the given direction to find our next point.    
            Point point = epicstartPoint + currentDirection;

            if (visited.Contains(point) || !IsInBounds(w, h, point.X, point.Y)) return null;

            // Avoid touching/crossing path so far.    
            foreach (var neighbour in cardDirs.Where(x => x != -currentDirection).Select(x => point + x))
                if (IsInBounds(w, h, neighbour.X, neighbour.Y) && visited.Contains(neighbour)) return null;

            visited.Add(point);

            // Endpoint! Time to bubble back up.    
            if (remainingBends == 0)
                return new() { point };
            foreach (var turnDirection in cardDirs.Where(x => x != -currentDirection).OrderBy(x => rand.Next()))
            {
                // Search for a feasible solution to a smaller sub-problem.
                var res = Extend(remainingBends - (currentDirection != turnDirection ? 1 : 0), point, turnDirection);
                if (res == null) continue;
                res.Add(point);
                return res;
            }

            // No such path through this point panned out. Undo our choice to try it.    
            visited.Remove(point);
            return null;
        }
    }



    /// <summary>
    /// Gets the positions of neighbouring cells of a cell at the given coordinates.
    /// </summary>
    /// <remarks>If the given position is on the edge of the board it will not yield positions out of bounds.</remarks>
    /// <param name="board"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>An <see cref="IEnumerable(Point)"/> which contains the positions of the neighbours</returns>
    public static IEnumerable<Point> GetNeighboursPositions(Board board, int x, int y) =>
        new Point[] { new(x - 1, y), new(x, y - 1), new(x + 1, y), new(x, y + 1) }.Where(p => IsInBounds(board, p.X, p.Y));

    /// <summary>
    /// Gets the actuall <see cref="Cell"/> instances of the neighbours of the cell at the given coordinates.
    /// </summary>
    /// <param name="board"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>An <see cref="IEnumerable(Cell)"/> containing the neighbours.</returns>
    public static IEnumerable<Cell> GetNeighboursAt(Board board, int x, int y) => GetNeighboursPositions(board, x, y).Select(p => board[p.X, p.Y]);

    /// <summary>
    /// Checks whether or not there are any dijoint components in the given <see cref="Board"/>.
    /// </summary>
    /// <param name="board">The board to check</param>
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
    /// <param name="board">The board</param>
    /// <param name="colorPairs"></param>
    /// <returns>A valid coloring of the board</returns>
    public static Dictionary<char, ColorPair> CreateColorMap(Board board)
    {
        var colorPairs = new ColorPair[] { ColorPair.Blue, ColorPair.Red, ColorPair.Green, ColorPair.Yellow };
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
    /// This function creates the m-coloring of an undirected graph.
    /// </summary>
    /// <remarks>This uses backtracing so it might take a long time in certain situations.</remarks>
    /// <typeparam name="T">The type of the graph nodes</typeparam>
    /// <param name="graph">A graph in the form of an adjecency list</param>
    /// <param name="m">The number of different colors to use</param>
    /// <returns>A <see cref="Dictionary(T, int)"/>, which maps a node of the graph to a color index.</returns>
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


    public static bool IsInBounds(Board board, int x, int y) => IsInBounds(board.Width, board.Height, x, y);
    public static bool IsInBounds(int w, int h, int x, int y) => 0 <= x && x < w && 0 <= y && y < h;

}
