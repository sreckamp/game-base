using GameBase.Model;
using System;
using System.Diagnostics;
using System.Drawing;

namespace GameBase.Console
{
    public class ConsoleFlowContainer : ConsoleContainer
    {
        public ConsoleFlowContainer(string name = null) : base(name)
        {
        }
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

        public override Size MeasureOverride(Size availableSize)
        {
            var size = new Size(0, 0);
            var offset = 0;
            foreach (var c in Children)
            {
                c.Measure(availableSize);
                if (IsHorizontalLayout)
                {
                    //TODO: Wrap layout here.
                    availableSize = new Size(availableSize.Width - c.DesiredSize.Width, availableSize.Height);
                    size = new Size(size.Width + c.DesiredSize.Width, Math.Max(c.DesiredSize.Height, size.Height));
                }
                else
                {
                    //TODO: Wrap layout here.
                    availableSize = new Size(availableSize.Width, availableSize.Height - c.DesiredSize.Height);
                    size = new Size(Math.Max(size.Width, c.DesiredSize.Width), size.Height + c.DesiredSize.Height);
                }
            }
            return size;
        }

        public override Size ArrangeOverride(Size availableSizeWithoutMargin)
        {
            var size = new Size(0, 0);
            var top = RenderLocation.Y + Padding.Top;
            var left = RenderLocation.X + Padding.Left;
            var availableHeight = availableSizeWithoutMargin.Height - Padding.TotalSize.Height;
            var availableWidth = availableSizeWithoutMargin.Width - Padding.TotalSize.Width;
            var height = Padding.TotalSize.Height;
            var width = Padding.TotalSize.Width;
            var offset = 0;
            foreach (var c in Children)
            {
                //TODO Alignment
                if (IsHorizontalLayout)
                {
                    left += Math.Max(offset, c.Margin.Left);
                    c.Arrange(new Rectangle(left, top, c.DesiredSize.Width, availableHeight));
                    availableWidth = Math.Max(0, availableWidth - c.RenderSize.Width);
                    width = Math.Min(availableSizeWithoutMargin.Width, width + c.RenderSize.Width);
                    height = Math.Max(height, c.RenderSize.Height);
                    left += c.RenderSize.Width;
                    offset = c.Margin.Right;
                }
                else
                {
                    top += Math.Max(offset, c.Margin.Top);
                    c.Arrange(new Rectangle(left, top, availableWidth, c.DesiredSize.Height));
                    availableHeight = Math.Max(0, availableHeight - c.RenderSize.Height);
                    width = Math.Max(width, c.RenderSize.Width);
                    height = Math.Min(availableSizeWithoutMargin.Height, height + c.RenderSize.Height);
                    top += c.RenderSize.Height;
                    offset = c.Margin.Bottom;
                }
            }
            return new Size(width, height);
        }
    }
}
