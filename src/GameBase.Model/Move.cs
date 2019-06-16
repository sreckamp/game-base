using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GameBase.Model
{
    public class Move : IEquatable<Move>
    {
        public Move(int x, int y)
            : this(new Point(x, y))
        {
        }

        public Move(Point location)
        {
            Location = location;
        }

        public Point Location { get; set; }

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
            if (other != null)
            {
                return Location.Equals(other.Location);
            }
            return false;
        }

        #endregion

        public override string ToString()
        {
            return Location.ToString();
        }

        public static implicit operator Point(Move m)
        {
            return (m == null) ? new Point(-1,-1) : m.Location;
        }
        public static implicit operator Move(Point p)
        {
            return new Move(p);
        }
    }
}
