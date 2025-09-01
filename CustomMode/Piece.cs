namespace CustomMode
{
    public struct Piece
    {
        public string type;
        public bool isWhite;
        public bool isNull;
        public Piece(string type, bool isWhite)
        {
            this.type = type;
            this.isWhite = isWhite;
            this.isNull = false;
        }

        public Piece(bool isNull = false)
        {
            this.type = "";
            this.isWhite = false;
            this.isNull = isNull;
        }
    }
}