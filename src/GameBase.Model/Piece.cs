namespace GameBase.Model
{
    public class Piece
    {
        public virtual Piece Clone()
        {
            var p = new Piece();
            return p;
        }

    }
}