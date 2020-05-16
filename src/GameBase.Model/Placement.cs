using System;
using System.Drawing;

namespace GameBase.Model
{
    public class Placement<TP, TM> : IComparable, IComparable<Placement<TP, TM>> where TP : Piece where TM : Move
    {
        public Placement(TP piece, TM move)
        {
            Piece = piece;
            Move = move;
        }

        public TP Piece { get; }
        public TM Move { get; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Placement<TP, TM>);
        }

        #endregion

        #region IComparable<Placement<P,M>> Members

        public int CompareTo(Placement<TP,TM> other)
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
                return Move.Location.X == other.Move.Location.X ? Move.Location.Y.CompareTo(other.Move.Location.Y) : Move.Location.X.CompareTo(other.Move.Location.X);
            }

            if (other.Move == null)
            {
                return 0;
            }

            return -1;
        }

        #endregion
        public static implicit operator Point(Placement<TP,TM> p)
        {
            return p?.Move?.Location ?? new Point();
        }

        public override string ToString()
        {
            return Piece + "@" + Move;
        }
    }
}
