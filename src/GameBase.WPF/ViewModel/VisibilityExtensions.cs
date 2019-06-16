using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace GameBoard.WPF.ViewModel
{
    public static class VisibilityExtensions
    {
        public static Visibility Or(this Visibility vis, params Visibility[] others)
        {
            if (vis == Visibility.Visible)
            {
                return vis;
            }
            foreach (var v in others)
            {
                if (v == Visibility.Visible)
                {
                    return v;
                }
            }
            return Visibility.Hidden;
        }

        public static Visibility Or(this Visibility vis, params bool[] bools)
        {
            if (vis == Visibility.Visible)
            {
                return vis;
            }
            foreach (var b in bools)
            {
                if (b)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Hidden;
        }

        public static Visibility And(this Visibility vis, params Visibility[] others)
        {
            if (vis == Visibility.Hidden)
            {
                return vis;
            }
            foreach (var v in others)
            {
                if (v == Visibility.Hidden)
                {
                    return v;
                }
            }
            return Visibility.Visible;
        }

        public static Visibility And(this Visibility vis, params bool[] bools)
        {
            if (vis == Visibility.Hidden)
            {
                return vis;
            }
            foreach (var b in bools)
            {
                if (!b)
                {
                    return Visibility.Hidden;
                }
            }
            return Visibility.Visible;
        }

        public static Visibility Not(this Visibility vis)
        {
            if (vis != Visibility.Visible)
            {
                return Visibility.Visible;
            }
            return Visibility.Hidden;
        }
    }
}
