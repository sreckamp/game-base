using System;
using System.Drawing;

namespace GameBase.Model
{
    public class Move : IEquatable<Move>
    {
        public static readonly Move None = new Move(-1, -1) { IsEmpty = true };

        private Move(int x, int y)
            : this(new Point(x, y))
        {
        }

        protected Move(Point location)
        {
            Location = location;
        }

        public bool IsEmpty
        {
            get;
            protected set;
        }

        public Point Location { get; protected set; }

        public override bool Equals(object other)
        {
            return Equals(other as Move);
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }

        #region IEquatable<Move> Members

        public bool Equals(Move other)
        {
            return Location.Equals(other?.Location);
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
