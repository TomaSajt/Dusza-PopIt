using Popit.IO;


ConsoleListener.Setup();
ConsoleListener.Start();
Console.WriteLine("Hello, World!");
ConsoleListener.MouseEvent += (a) => {
    if (a.dwButtonState == NativeMethods.MOUSE_EVENT_RECORD.FROM_LEFT_1ST_BUTTON_PRESSED)
        Console.WriteLine("asd");
};
