using System;
using System.Drawing;

namespace GameBase.Model
{
    public class Placement<T> : IComparable, IComparable<Placement<T>>
    {
        public Placement(T piece, Point location)
        {
            Piece = piece;
            Location = location;
        }

        public T Piece { get; set; }
        public Point Location { get; set; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (!(obj is Placement<T> plc)) return -1;
            return CompareTo(plc);
        }

        #endregion

        #region IComparable<Placement<T>> Members

        public int CompareTo(Placement<T> other)
        {
            return Location.X == other.Location.X ? Location.Y.CompareTo(other.Location.Y) : Location.X.CompareTo(other.Location.X);
        }

        #endregion
        public static implicit operator Point(Placement<T> p)
        {
            return p.Location;
        }

        public override string ToString()
        {
            return Piece + " @ " + Location;
        }
    }
}
