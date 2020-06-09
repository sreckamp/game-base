using System.Linq;
using System.Windows;

namespace GameBase.WPF.ViewModel
{
    public static class VisibilityExtensions
    {
        public static Visibility ToVisibility(this bool vis) => vis ? Visibility.Visible : Visibility.Hidden;
        public static Visibility Or(this Visibility vis, params Visibility[] others)
        {
            return vis == Visibility.Visible || others.Any(b => b == Visibility.Visible)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        public static Visibility Or(this Visibility vis, params bool[] booleans)
        {
            return vis == Visibility.Visible || booleans.Any(b => b) ? Visibility.Visible : Visibility.Hidden;
        }

        public static Visibility And(this Visibility vis, params Visibility[] others)
        {
            return vis == Visibility.Hidden || others.Any(b => b == Visibility.Hidden)
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        public static Visibility And(this Visibility vis, params bool[] booleans)
        {
            return vis == Visibility.Hidden || booleans.Any(b => !b) ? Visibility.Hidden : Visibility.Visible;
        }

        public static Visibility Not(this Visibility vis)
        {
            return vis != Visibility.Visible ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
