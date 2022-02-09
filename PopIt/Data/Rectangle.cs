namespace PopIt.Data
{
    record struct Rectangle(short X, short Y, short Width, short Height)
    {
        public short Top { get => Y; }
        public short Bottom { get => (short)(Y + Height); }
        public short Left { get => X; }
        public short Right { get => (short)(X + Width); }

        public bool Contains(short x, short y) => x >= Left && x <= Right && y >= Top && y <= Bottom;
    }
}
