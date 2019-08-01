using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public enum ConsoleBorderStyle
    {
        None,
        Single,
        Double,
    }

    public class RenderTools
    {
        private static readonly char[,] s_boxParts = new char[,]{
            //  H,       V,       TL,      TR,      BL,      BR,      T,       B,       L,       R,
            {'\u0020','\u0020','\u0020','\u0020','\u0020','\u0020','\u0020','\u0020','\u0020','\u0020','\u0020' },
            {'\u2500','\u2502','\u250C','\u2510','\u2514','\u2518','\u2530','\u2538','\u2520','\u2528','\u2549' },
            {'\u2550','\u2551','\u2554','\u2557','\u255A','\u255D','\u2566','\u2569','\u2560','\u2563','\u256C' },
        };

        public static void RenderBlank(IConsoleContext ctx, int left, int top, int width, int height)
        {
            if (width > 0 && height > 0)
            {
                //System.Diagnostics.Debug.WriteLine($"Clear {width}x{height} @ ({left},{top})");
                var padding = string.Empty.PadLeft(width);
                for (int y = 0; y < height; y++)
                {
                    ctx.Write(padding, left, top + y);
                }
            }
        }
        public static void RenderBox(IConsoleContext ctx, int left, int top, int width, int height, ConsoleBorderStyle style)
        {
            if (width > 0)
            {
                var right = left + width - 1;
                var bottom = top + height - 1;
                ctx.Write(s_boxParts[(int)style, (int)BoxPart.CornerTopLeft]/*'\u250C'*/, left, top);
                ctx.Write(s_boxParts[(int)style, (int)BoxPart.CornerTopRight]/*'\u2510'*/, right, top);
                ctx.Write(s_boxParts[(int)style, (int)BoxPart.CornerBottomLeft]/*'\u2514'*/, left, bottom);
                ctx.Write(s_boxParts[(int)style, (int)BoxPart.CornerBottomRight]/*'\u2518'*/, right, bottom);
                for (int i = 1; i < width - 1; i++)
                {
                    ctx.Write(s_boxParts[(int)style, (int)BoxPart.Horizontal]/*'\u2500'*/, left + i, top);
                    ctx.Write(s_boxParts[(int)style, (int)BoxPart.Horizontal]/*'\u2500'*/, left + i, bottom);
                }
                for (int i = 1; i < height - 1; i++)
                {
                    ctx.Write(s_boxParts[(int)style, (int)BoxPart.Vertical]/*'\u2502'*/, left, top + i);
                    ctx.Write(s_boxParts[(int)style, (int)BoxPart.Vertical]/*'\u2502'*/, right, top + i);
                }
            }
        }

        private enum BoxPart
        {
            Horizontal,
            Vertical,
            CornerTopLeft,
            CornerTopRight,
            CornerBottomLeft,
            CornerBottomRight,
            JoinTop,
            JoinBottom,
            JoinLeft,
            JoinRight,
            Cross,
        }
    }
}
