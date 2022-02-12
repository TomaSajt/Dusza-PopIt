using PopIt;
using PopIt.Data;
using PopIt.UI;
using System.Text;

bool exit = false;
var globalParent = UIElement.CreateGlobalParent();
do
{
    Console.Clear();
    Console.CursorVisible = true;
    Console.WriteLine("=== Pop It ===");
    int sel = Selection(new[] { "Indítás meglévő pályán", "Pálya generálása és indítás", "Kilépés" });
    switch (sel)
    {
        case 1:
            Console.Clear();
            Console.WriteLine("Add meg a pálya számát:");
            new Game(globalParent, 10, 10, $"palya{ReadInt()}.txt", 3).Run();
            //Todo: check if board exists
            break;
        case 2:
            Console.Clear();
            Console.WriteLine("Mekkora legyen a pálya? (4-10)");
            var size = ReadInt();
            Console.WriteLine("Hány darab hajlítás legyen a pályán? (0-4) (Jelenleg nem működik)");
            var bends = ReadInt();
            var board = BoardUtils.GenerateBoard(size, size, bends);
            DoSaveBoard(board);
            new Game(globalParent, 10, 10, board, 3).Run();
            break;
        case 3:
            exit = true;
            break;
    }
} while (!exit);


static void DoSaveBoard(Board board)
{
    while (true)
    {
        Console.WriteLine("Mi legyen a pálya sorszáma? (későbbi betöltéshez)");
        int id = ReadInt();
        string path = $"palya{id}.txt";
        if (File.Exists(path))
        {
            Console.WriteLine("Ez a fájl már létezik, felül szeretné írni? (y/n) (i/n)");
            if (!YesNoPrompt()) continue;
        }
        BoardUtils.SaveToFile(board, $"palya{id}.txt");
        break;
    }
}

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

static bool YesNoPrompt()
{
    while (true)
    {
        var key = Console.ReadKey(true).Key;
        if (key is ConsoleKey.Y or ConsoleKey.I) return true;
        if (key is ConsoleKey.N) return false;
    }
}