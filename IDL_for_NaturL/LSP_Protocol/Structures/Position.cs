namespace IDL_for_NaturL
{
    public struct Position
    {
        public int line;
        public int character;

        public Position(int line, int character)
        {
            this.line = line;
            this.character = character;
        }
    }
}