using PopIt;
using PopIt.IO;

IOManager.Start();
IOManager.MouseDown += (x, y) => Console.WriteLine($"down {x} {y}");
IOManager.MouseUp += (x, y) => Console.WriteLine($"up {x} {y}");
IOManager.ResizeEvent += (w, h) => Console.WriteLine($"{w} {h}");
var game = new Game();
game.Render();
