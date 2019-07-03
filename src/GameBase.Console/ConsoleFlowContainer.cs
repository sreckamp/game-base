using GameBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public class ConsoleFlowContainer : ConsoleContainer
    {
        public event EventHandler<ChangedValueArgs<bool>> IsHorizontalLayoutChanged;
        private bool m_isHorizontalLayout = false;
        public bool IsHorizontalLayout
        {
            get => m_isHorizontalLayout;
            set
            {
                var old = m_isHorizontalLayout;
                m_isHorizontalLayout = value;
                InvalidateLayout();
                IsHorizontalLayoutChanged?.Invoke(this, new ChangedValueArgs<bool>(old, value));
            }
        }

        //protected override void ComponentLayout(ref int layoutWidth, ref int layoutHeight)
        //{
        //    int width = 0;
        //    int height = 0;
        //    foreach (var c in Children)
        //    {
        //        c.Layout(-1, -1);
        //        if (IsHorizontalLayout)
        //        {
        //            width += c.ActualWidth + Padding.Left + Padding.Right;
        //            height = Math.Max(height, c.ActualHeight + Padding.Top + Padding.Bottom);
        //        }
        //        else
        //        {
        //            width = Math.Max(width, c.ActualWidth + Padding.Left + Padding.Right);
        //            height += c.ActualHeight + Padding.Top + Padding.Bottom;
        //        }
        //    }
        //    if(width > 0 && height > 0)
        //    {
        //        layoutWidth = width;
        //        layoutHeight = height;
        //    }
        //}

        protected override void UpdateBounds(int layoutWidth, int layoutHeight, ref int width, ref int height)
        {
            if (IsHorizontalLayout)
            {
                width += layoutWidth;
                height = Math.Max(height, layoutHeight);
            }
            else
            {
                width = Math.Max(width, layoutWidth);
                height += layoutHeight;
            }
        }

        protected override void PostRender(AbstractConsoleComponent component, ref int left, ref int top)
        {
            if (IsHorizontalLayout)
            {
                left += component.ActualWidth + Padding.Left + Padding.Right;
            }
            else
            {
                top += component.ActualHeight + Padding.Top + Padding.Bottom;
            }
        }
    }
}
