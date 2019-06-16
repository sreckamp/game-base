using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Model
{
    public class Placement<P, M> : IComparable, IComparable<Placement<P, M>> where P : Piece where M : Move
    {
        public Placement(P piece, M move)
        {
            Piece = piece;
            Move = move;
        }

        public P Piece { get; private set; }
        public M Move { get; private set; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Placement<P, M>);
        }

        #endregion

        #region IComparable<Placement<P,M>> Members

        public int CompareTo(Placement<P,M> other)
        {
            if (other == null)
            {
                return 1;
            }
            if (Move != null)
            {
                if (other.Move == null)
                {
                    return 1;
                }
                if (Move.Location.X == other.Move.Location.X)
                {
                    return Move.Location.Y.CompareTo(other.Move.Location.Y);
                }
                return Move.Location.X.CompareTo(other.Move.Location.X);
            }
            else if (other.Move == null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        #endregion
        public static implicit operator Point(Placement<P,M> p)
        {
            return p?.Move?.Location ?? new Point();
        }

        public override string ToString()
        {
            string val = Piece?.ToString() ?? "<<null>>";
            if (val != null) val += "@";
            return val + Move.ToString();
        }
    }
}
