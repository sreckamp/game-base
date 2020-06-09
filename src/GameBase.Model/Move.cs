using System.Drawing;

namespace GameBase.Model
{
    public class Move : IMove
    {
        public Move(Point location)
        {
            Location = location;
        }
    
        public bool IsEmpty
        {
            get;
            protected set;
        }
    
        public Point Location { get; }
    
        public override bool Equals(object other)
        {
            if (!(other is Move m)) return false;
            return Equals(m);
        }
    
        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }
    
        #region IEquatable<Move> Members
    
        public bool Equals(IMove other)
        {
            return Location.Equals(other.Location);
        }
    
        #endregion
    
        public override string ToString()
        {
            return Location.ToString();
        }
    
        public static implicit operator Point(Move m)
        {
            return m.Location;
        }
    
        public static implicit operator Move(Point p)
        {
            return new Move(p);
        }
    }
}
