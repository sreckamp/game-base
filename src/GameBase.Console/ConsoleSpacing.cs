using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public struct ConsoleSpacing : IEquatable<ConsoleSpacing>
    {
        public ConsoleSpacing(int value = 0) : this(value, value) { }
        public ConsoleSpacing(int horizontal, int vertical) : this(horizontal, vertical, horizontal, vertical) { }
        public ConsoleSpacing(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public override bool Equals(object obj)
        {
            if(obj is ConsoleSpacing cs)
            {
                return Equals(cs);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Top.GetHashCode()
                ^ Right.GetHashCode() ^ Bottom.GetHashCode();
        }

        public bool Equals(ConsoleSpacing other)
        {
            return other.Left == Left && other.Top == Top
                && other.Right == Right && other.Bottom == Bottom;
        }
        public override string ToString()
        {
            string format = "({0},{1},{2},{3})";
            if (Left == Right && Top == Bottom)
            {
                if (Left == Top)
                {
                    format = "({0})";
                }
                else
                {
                    format = "({0},{1})";
                }
            }
            return string.Format(format, Left, Top, Right, Bottom);
        }

        public static bool operator ==(ConsoleSpacing cs1, ConsoleSpacing cs2) => cs1.Equals(cs2);
        public static bool operator !=(ConsoleSpacing cs1, ConsoleSpacing cs2) => !cs1.Equals(cs2);
    }
}
