using PopIt;
using PopIt.Data;
using PopIt.IO;


IOManager.Run();
IOManager.LeftMouseDown += (x, y) => Console.WriteLine($"down {x} {y}");
IOManager.LeftMouseUp += (x, y) => Console.WriteLine($"up {x} {y}");
IOManager.ResizeEvent += (w, h) => Console.WriteLine($"{w} {h}");

var game = new Game();
game.Run();

/**/