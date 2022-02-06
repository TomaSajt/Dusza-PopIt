﻿using PopIt.Data;
using PopIt.Exception;

namespace PopIt;
static internal class BoardUtils
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
    public static void SaveToFile(Board board, string path)
    {
        using var sw = new StreamWriter(path);
        for (int i = 0; i < board.Height; i++)
        {
            for (int j = 0; j < board.Width; j++) sw.Write(board[j, i]);
            sw.WriteLine();
        }
    }
}
