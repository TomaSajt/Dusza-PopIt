﻿using PopIt;
using PopIt.Data;
using PopIt.IO;
using PopIt.UI;
using System.Text;

bool exit = false;
do
{
    Console.Clear();
    Console.CursorVisible = true;
    Console.WriteLine("=== Pop It ===");
    int sel = Selection(new string[]
    {
                    "Indítás meglévő pályán",
                    "Pálya generálása és indítás",
                    "Kilépés"
    });
    var globalParent = UIElement.CreateGlobalParent();

    switch (sel)
    {
        case 1:
            Console.Clear();
            Console.WriteLine("Add meg a pálya számát:");
            new Game(globalParent, 10, 10, $"palya{ReadInt()}.txt", 3).Run();
            break;
        case 2:
            Console.Clear();
            Console.WriteLine("Mekkora legyen a pálya? (4-10)");
            var size = ReadInt();
            Console.WriteLine("Hány darab hajlítás legyen a pályán? (0-4) (Jelenleg nem működik)");
            var bends = ReadInt();
            var board = BoardUtils.GenerateBoard(size, size, bends);
            Console.WriteLine("Mi legyen a pálya sorszáma? (későbbi betöltéshez)");
            BoardUtils.SaveToFile(board, $"palya{ReadInt()}.txt");
            new Game(globalParent, 10, 10, board, 3).Run();
            break;
        case 3:
            exit = true;
            break;
        default:
            break;
    }
} while (!exit);


static void EraseLine(int line)
{
    Console.SetCursorPosition(0, line);
    Console.WriteLine(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, line);
}

static int Selection(string[] choices)
{
    Console.WriteLine("Lehetőségek:");
    StringBuilder sb = new();
    for (int i = 0; i < choices.Length; i++)
    {
        sb.Append(i + 1);
        sb.Append(" - ");
        sb.AppendLine(choices[i]);
    }

    Console.Write(sb);

    int n = ReadInt();
    while (true)
    {
        if (n > 0 && n <= choices.Length)
            return n;
        EraseLine(Console.CursorTop - 1);
        n = ReadInt();
    }
}

static int ReadInt()
{
    while (true)
    {
        if (int.TryParse(Console.ReadLine(), out int n)) return n;
        EraseLine(Console.CursorTop - 1);
    }
}