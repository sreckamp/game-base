using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public static class PointExtensions
    {
        public static Point Sum(this Point me, params Point[] points)
        {
            var x = me.X;
            var y = me.Y;
            foreach(var p in points)
            {
                x += p.X;
                y += p.Y;
            }
            return new Point(x, y);
        }
    }
}
