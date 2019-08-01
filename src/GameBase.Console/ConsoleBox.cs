using GameBase.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public class ConsoleBox : AbstractConsoleElement
    {
        private readonly ChangeValueProperty<ConsoleBorderStyle> m_borderProperty;
        public ConsoleBox(string name = null) : base(name)
        {
            m_borderProperty = new ChangeValueProperty<ConsoleBorderStyle>(ConsoleBorderStyle.Single, borderChanged);
        }

        public ConsoleBorderStyle BorderStyle
        {
            get => m_borderProperty;
            set { m_borderProperty.Value = value; }
        }

        private void borderChanged(object sender, EventArgs e)
        {
        }

        public override Size ArrangeOverride(Size availableSize)
        {
            return availableSize;
        }

        public override Size MeasureOverride(Size availableSize)
        {
            return new Size(0,0);
        }

        //protected override void ComponentLayout(ref int layoutWidth, ref int layoutHeight)
        //{
        //}

        public override void RenderOverride(IConsoleContext context)
        {
            RenderTools.RenderBox(context, RenderLocation.X, RenderLocation.Y, RenderSize.Width, RenderSize.Height, BorderStyle);
        }

        //protected override void ComponentRender(int left, int top)
        //{
        //    if (ActualWidth > 0 && ActualHeight > 0)
        //    {
        //        RenderBox(left, top, ActualWidth - Margin.Left - Margin.Right, ActualHeight - Margin.Top - Margin.Bottom);
        //    }
        //}
    }
}
