using GameBase.Model;
using System;
using SConsole = System.Console;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GameBase.Console
{
    public abstract class AbstractConsoleElement
    {
        private readonly ChangeValueProperty<int> m_minWidthProperty;
        private readonly ChangeValueProperty<int> m_maxWidthProperty;
        private readonly ChangeValueProperty<int> m_widthProperty;
        private readonly ChangeValueProperty<int> m_minHeightProperty;
        private readonly ChangeValueProperty<int> m_maxHeightProperty;
        private readonly ChangeValueProperty<int> m_heightProperty;

        private readonly ChangeValueProperty<Size> m_desiredSizeProperty;
        private readonly ChangeValueProperty<Size> m_renderSizeProperty;
        private readonly ChangeValueProperty<Point> m_visualOffsetProperty;
        private readonly ChangeValueProperty<ConsoleSpacing> m_marginProperty;
        private readonly ChangeValueProperty<ConsoleColor?> m_foregroundProperty;
        private readonly ChangeValueProperty<ConsoleColor?> m_backgroundProperty;
        private readonly ChangeValueProperty<bool> m_isEnabledProperty;
        private readonly ChangeValueProperty<bool> m_isVisibleProperty;
        private readonly ChangeValueProperty<HorizontalAlignment> m_horizontalAlignmentProperty;
        private readonly ChangeValueProperty<VerticalAlignment> m_verticalAlignmentProperty;
        protected AbstractConsoleElement(string name)
        {
            Name = name;
            m_minWidthProperty = new ChangeValueProperty<int>(-1, measurePropertyChanged);
            m_maxWidthProperty = new ChangeValueProperty<int>(-1, measurePropertyChanged);
            m_widthProperty = new ChangeValueProperty<int>(-1, measurePropertyChanged);

            m_minHeightProperty = new ChangeValueProperty<int>(-1, measurePropertyChanged);
            m_maxHeightProperty = new ChangeValueProperty<int>(-1, measurePropertyChanged);
            m_heightProperty = new ChangeValueProperty<int>(-1, measurePropertyChanged);

            m_desiredSizeProperty = new ChangeValueProperty<Size>(new Size(-1,-1));
            m_renderSizeProperty = new ChangeValueProperty<Size>(new Size(-1, -1));
            m_marginProperty = new ChangeValueProperty<ConsoleSpacing>(new ConsoleSpacing(0), measurePropertyChanged);
            m_foregroundProperty = new ChangeValueProperty<ConsoleColor?>(ConsoleColor.White, paintPropertyChanged);
            m_backgroundProperty = new ChangeValueProperty<ConsoleColor?>(ConsoleColor.Black, paintPropertyChanged);

            m_isEnabledProperty = new ChangeValueProperty<bool>(true, paintPropertyChanged);
            m_visualOffsetProperty = new ChangeValueProperty<Point>(new Point(0, 0), paintPropertyChanged);
            m_isVisibleProperty = new ChangeValueProperty<bool>(true, measurePropertyChanged);

            m_horizontalAlignmentProperty = new ChangeValueProperty<HorizontalAlignment>(HorizontalAlignment.Stretch, arrangePropertyChanged);
            m_verticalAlignmentProperty = new ChangeValueProperty<VerticalAlignment>(VerticalAlignment.Stretch, arrangePropertyChanged);
        }

        private void measurePropertyChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void arrangePropertyChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
        private void paintPropertyChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        public string Name { get; private set; }

        public event EventHandler<ChangedValueArgs<int>> MinWidthChanged
        {
            add { m_minWidthProperty.ValueChanged += value; }
            remove { m_minWidthProperty.ValueChanged -= value; }
        }
        /// <summary>
        /// The requested minimum width of the item.
        /// (First constraint during measure)
        /// </summary>
        public int MinWidth
        {
            get => m_minWidthProperty.Value;
            set { m_minWidthProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<int>> MaxWidthChanged
        {
            add { m_maxWidthProperty.ValueChanged += value; }
            remove { m_maxWidthProperty.ValueChanged -= value; }
        }
        /// <summary>
        /// The requested maximum width of the item.
        /// (Second constraint during measure)
        /// </summary>
        public int MaxWidth
        {
            get => m_maxWidthProperty.Value;
            set { m_maxWidthProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<int>> WidthChanged
        {
            add { m_widthProperty.ValueChanged += value; }
            remove { m_widthProperty.ValueChanged -= value; }
        }
        /// <summary>
        /// The requested width of the item.
        /// (Third constraint during measure)
        /// </summary>
        public int Width
        {
            get => m_widthProperty.Value;
            set { m_widthProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<int>> MinHeightChanged
        {
            add { m_minHeightProperty.ValueChanged += value; }
            remove { m_minHeightProperty.ValueChanged -= value; }
        }

        /// <summary>
        /// The requested minimum height of the item.
        /// (First constraint during measure)
        /// </summary>
        public int MinHeight
        {
            get => m_minHeightProperty.Value;
            set { m_minHeightProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<int>> MaxHeightChanged
        {
            add { m_maxHeightProperty.ValueChanged += value; }
            remove { m_maxHeightProperty.ValueChanged -= value; }
        }

        /// <summary>
        /// The requested maximum height of the item.
        /// (Second constraint during measure)
        /// </summary>
        public int MaxHeight
        {
            get => m_maxHeightProperty.Value;
            set { m_maxHeightProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<int>> HeightChanged
        {
            add { m_heightProperty.ValueChanged += value; }
            remove { m_heightProperty.ValueChanged -= value; }
        }

        public int Height
        {
            get => m_heightProperty.Value;
            set { m_heightProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<Size>> DesiredSizeChanged
        {
            add { m_desiredSizeProperty.ValueChanged += value; }
            remove { m_desiredSizeProperty.ValueChanged -= value; }
        }

        public Size DesiredSize
        {
            get => m_desiredSizeProperty.Value;
            set { m_desiredSizeProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<Size>> RenderSizeChanged
        {
            add { m_renderSizeProperty.ValueChanged += value; }
            remove { m_renderSizeProperty.ValueChanged -= value; }
        }
        /// <summary>
        /// The Arranged Height of the item
        /// </summary>
        public Size RenderSize
        {
            get => m_renderSizeProperty.Value;
            set { m_renderSizeProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<Point>> VisualOffsetChanged
        {
            add { m_visualOffsetProperty.ValueChanged += value; }
            remove { m_visualOffsetProperty.ValueChanged -= value; }
        }
        public Point RenderLocation
        {
            get => m_visualOffsetProperty.Value;
            set { m_visualOffsetProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<ConsoleSpacing>> MarginChanged
        {
            add { m_marginProperty.ValueChanged += value; }
            remove { m_marginProperty.ValueChanged -= value; }
        }
        public ConsoleSpacing Margin
        {
            get => m_marginProperty.Value;
            set { m_marginProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<ConsoleColor?>> BackgroundChanged
        {
            add { m_backgroundProperty.ValueChanged += value; }
            remove { m_backgroundProperty.ValueChanged += value; }
        }
        public ConsoleColor? BackgroundColor
        {
            get => m_backgroundProperty.Value;
            set { m_backgroundProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<ConsoleColor?>> ForegroundChanged
        {
            add { m_foregroundProperty.ValueChanged += value; }
            remove { m_foregroundProperty.ValueChanged += value; }
        }
        public ConsoleColor? ForegroundColor
        {
            get => m_foregroundProperty.Value;
            set { m_foregroundProperty.Value = value; }
        }

        public event EventHandler<ChangedValueArgs<bool>> IsEnabledChanged
        {
            add { m_isEnabledProperty.ValueChanged += value; }
            remove { m_isEnabledProperty.ValueChanged -= value; }
        }
        public bool IsEnabled
        {
            get => m_isEnabledProperty.Value;
            set { m_isEnabledProperty.Value = value; }
        }

        protected virtual void OnIsEnabledChanged(bool oldVal, bool newVal) { }

        public event EventHandler<ChangedValueArgs<bool>> IsVisibleChanged
        {
            add { m_isVisibleProperty.ValueChanged += value; }
            remove { m_isVisibleProperty.ValueChanged -= value; }
        }
        public bool IsVisible
        {
            get => m_isVisibleProperty.Value;
            set { m_isVisibleProperty.Value = value; }
        }

        //private bool m_layoutRequred = true;

        public event EventHandler Invalidated;
        public virtual void InvalidateLayout()
        {
            //m_layoutRequred = true;
            Invalidated?.Invoke(this, new EventArgs());
        }

        public void Measure(Size availableSize)
        {
            ///available space = DesiredSize (includes margin)
            var size = new Size(Width, Height);
            if (Height < 0 || Width < 0)
            {
                var measuredSize = MeasureOverride(availableSize);
                size = new Size(Width < 0 ? measuredSize.Width : Width, Height < 0 ? measuredSize.Height : Height);
            }
            DesiredSize = size + Margin.TotalSize;
            Debug.WriteLine($"{Name ?? GetType().Name}:{DesiredSize}");
        }

        /// <summary>
        /// Return the minimum size required within the given size. (Excluding Margin)
        /// </summary>
        /// <param name="availableSize">The amout of space available</param>
        /// <returns>The amount of space required</returns>
        public abstract Size MeasureOverride(Size availableSize);

        public void Arrange(Rectangle window)
        {
            RenderLocation = window.Location.Sum(Margin.TopLeft);
            ///window.Size = RenderSize + Margin
            var sz = window.Size - Margin.TotalSize;
            RenderSize = ArrangeOverride(window.Size - Margin.TotalSize);
            Debug.WriteLine($"{Name ?? GetType().Name}:{RenderSize}@{RenderLocation}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSizeWithoutMargin">Placement without the margin.</param>
        /// <returns></returns>
        public abstract Size ArrangeOverride(Size availableSizeWithoutMargin);

        public void Render(IConsoleContext context)
        {
            context.SetForegroundColor(ForegroundColor);
            context.SetBackgroundColor(BackgroundColor);
            RenderTools.RenderBlank(context, RenderLocation.X, RenderLocation.Y, RenderSize.Width, RenderSize.Height);
            RenderOverride(context);
            context.PopBackgroundColor();
            context.PopForegroundColor();
        }

        public abstract void RenderOverride(IConsoleContext context);

        /// <summary>
        /// Prepare the layout of the control
        /// </summary>
        /// <param name="targetWidth">The target width for the control.</param>
        /// <param name="targetHeight">The target height for the control.</param>
        /// <returns>true if the layout has been updated.</returns>
        public bool Layout(int targetWidth, int targetHeight)
        {
            //int layoutWidth = Width;
            //int layoutHeight = Height;

            //if(layoutWidth < 0)
            //{
            //    layoutWidth = targetWidth;
            //}
            //if (layoutHeight < 0)
            //{
            //    layoutHeight = targetHeight;
            //}

            //var layoutUpdated = m_layoutRequred;
            //if(m_layoutRequred)
            //{
            //    ComponentLayout(ref layoutWidth, ref layoutHeight);
            //    if(layoutWidth >= 0 && layoutHeight >=0)
            //    {
            //        if (layoutHeight != Height)
            //        {
            //            layoutHeight += Margin.Top + Margin.Bottom;
            //        }
            //        ActualHeight = layoutHeight;
            //        if (layoutWidth != Width)
            //        {
            //            layoutWidth += Margin.Left + Margin.Right;
            //        }
            //        ActualWidth = layoutWidth;
            //    }
            //    else
            //    {
            //        layoutUpdated = false;
            //    }
            //    m_layoutRequred = false;
            //}
            //return layoutUpdated;
            return false;
        }

        //protected abstract void ComponentLayout(ref int layoutWidth, ref int layoutHeight);

        //private int m_renderedTop = -1;
        //private int m_renderedLeft = -1;
        //private int m_renderedWidth = -1;
        //private int m_renderedHeight = -1;
        /// <summary>
        /// Draw the control on the console using the left and top provided.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void Render(int left, int top, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            //if (IsVisible && ActualHeight > 0 && ActualWidth > 0)
            //{
            //    var bg = SConsole.BackgroundColor;
            //    var fg = SConsole.ForegroundColor;
            //    SConsole.ForegroundColor = foreground ?? ForegroundColor;
            //    SConsole.BackgroundColor = background ?? BackgroundColor;

            //    if (HasBackground && (m_renderedTop != top || m_renderedLeft != left || m_renderedHeight < ActualHeight))
            //    {
            //        RenderBlank(left, top, ActualWidth, ActualHeight);
            //    }
            //    ComponentRender(left + Margin.Left, top + Margin.Top);
            //    if (m_renderedWidth > 0 && ActualWidth < m_renderedWidth)
            //    {
            //        RenderBlank(left + Margin.Left + ActualWidth, top + Margin.Top, m_renderedWidth - ActualWidth, ActualHeight);
            //    }
            //    if (m_renderedHeight > 0 && ActualHeight < m_renderedHeight)
            //    {
            //        RenderBlank(left + Margin.Left, top + Margin.Top + ActualHeight, Math.Max(m_renderedWidth, ActualWidth), m_renderedHeight - ActualHeight);
            //    }
            //    m_renderedTop = top;
            //    m_renderedLeft = left;
            //    m_renderedWidth = ActualWidth;
            //    m_renderedHeight = ActualHeight;
            //    SConsole.ForegroundColor = fg;
            //    SConsole.BackgroundColor = bg;
            //}
        }

        //protected abstract void ComponentRender(int left, int top);

        /// <summary>
        /// Handle that the enter key has been pressed.
        /// </summary>
        /// <param name="line">The string typed in before the key is pressed.</param>
        /// <returns>true if the event is consumed.</returns>
        public bool LineEntered(string line)
        {
            if(IsVisible && IsEnabled)
            {
                return ComponentLineEntered(line);
            }
            return false;
        }

        protected virtual bool ComponentLineEntered(string line) { return false; }

        /// <summary>
        /// Handles when a key is pressed in the UI
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        /// <returns>true if the event is consumed.</returns>
        public bool KeyPressed(ConsoleKeyInfo key)
        {
            if (IsVisible && IsEnabled)
            {
                return ComponentKeyPressed(key);
            }
            return false;
        }
        protected virtual bool ComponentKeyPressed(ConsoleKeyInfo key) { return false; }

        public override string ToString()
        {
            if (Name != null) return Name;
            return base.ToString();
        }
    }
}
