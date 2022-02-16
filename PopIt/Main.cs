using PopIt;
using PopIt.Data;
using PopIt.IO;


bool exit = false;
IOManager.Run();
do
{
    Console.Clear();
    Console.CursorVisible = true;
    Console.WriteLine("=== Pop It ===");
    int sel = IOManager.Selection(new[] { "Indítás meglévő pályán", "Pálya generálása és indítás", "Kilépés" });
    Console.Clear();
    switch (sel)
    {
        case 1:
            Console.WriteLine("Add meg a pálya számát:");
            var path = $"palya{IOManager.ReadInt()}.txt";
            if (!File.Exists(path))
            {
                Console.WriteLine("Ez a pálya nem létezik.");
                break;
            }
            Console.Clear();
            new Game(path, 2).Run();
            break;
        case 2:
            Console.WriteLine("Mekkora legyen a pálya? (4-10)");
            var size = IOManager.ReadInt(x => x >= 4 && x <= 10);
            Console.WriteLine("Hány darab hajlítás legyen a pályán? (0-4)");
            var bends = IOManager.ReadInt(x => x >= 0 && x <= 4);
            var board = BoardUtils.GenerateBoard(size, size, bends);
            DoSaveBoard(board);
            Console.Clear();
            new Game(board, 2).Run();
            break;
        case 3:
            exit = true;
            break;
    }
} while (!exit);

// IOManager is still running, using IOManager.Stop() will still consume an extra key, so instead use force exit
Environment.Exit(Environment.ExitCode);


static void DoSaveBoard(Board board)
{
    while (true)
    {
        Console.WriteLine("Mi legyen a pálya sorszáma? (későbbi betöltéshez)");
        int id = IOManager.ReadInt();
        string path = $"palya{id}.txt";
        if (File.Exists(path))
        {
            Console.WriteLine("Ez a fájl már létezik, felül szeretné írni? (y/n) (i/n)");
            if (!IOManager.YesNoPrompt()) continue;
        }
        BoardUtils.SaveToFile(board, $"palya{id}.txt");
        break;
    }
}

