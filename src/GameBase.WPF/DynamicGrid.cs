using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DPoint = System.Drawing.Point;

namespace GameBase.WPF
{
    public class DynamicGrid : Grid
    {
        public DynamicGrid()
        {
            OverCell += (sender, args) => { };
            Background = new SolidColorBrush(Colors.Transparent);
            Background.Freeze();
        }

        #region OverCell

        public static readonly RoutedEvent OverCellEvent = EventManager.RegisterRoutedEvent(
            nameof(OverCell), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DynamicGrid));

        public event RoutedEventHandler OverCell
        {
            add => AddHandler(OverCellEvent, value);
            remove => RemoveHandler(OverCellEvent, value);
        }

        private DPoint m_lastCell = new DPoint(-1, -1);

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (MonitorMouse)
            {
                Debug.WriteLine("DynamicGrid.OnMouseLeave");
                CellEntered(int.MinValue, int.MinValue);
            }
            else
            {
                e.Handled = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (MonitorMouse)
            {
                var p = e.GetPosition(this);
                if (p.X < 0 || p.Y < 0) return;
                int row = 0, column = 0;
                int x = 0, y = 0;
                foreach (var cd in ColumnDefinitions)
                {
                    x += (int) cd.ActualWidth;
                    if (p.X < x) break;
                    column++;
                }

                foreach (var rd in RowDefinitions)
                {
                    y += (int) rd.ActualHeight;
                    if (p.Y < y) break;
                    row++;
                }

                if (row < RowDefinitions.Count && column < ColumnDefinitions.Count)
                {
                    CellEntered(column, row);
                }
            }
            else
            {
                e.Handled = false;
            }
        }

        private void CellEntered(int column, int row)
        {
            var gc = new DPoint(column, row);
            if (gc == m_lastCell) return;
            m_lastCell = gc;
            var args = new GridCellRoutedEventArgs(OverCellEvent, gc);
            RaiseEvent(args);
        }

        #endregion

        #region LeftClick

        public static readonly RoutedEvent LeftClickEvent = EventManager.RegisterRoutedEvent(
            nameof(LeftClickEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameGrid));

        public event RoutedEventHandler LeftClick
        {
            add => AddHandler(LeftClickEvent, value);
            remove => RemoveHandler(LeftClickEvent, value);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            RaiseEvent(new RoutedEventArgs(LeftClickEvent, this));
        }

        #endregion

        #region RightClick

        public static readonly RoutedEvent RightClickEvent = EventManager.RegisterRoutedEvent(
            nameof(RightClickEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameGrid));

        public event RoutedEventHandler RightClick
        {
            add => AddHandler(RightClickEvent, value);
            remove => RemoveHandler(RightClickEvent, value);
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            RaiseEvent(new RoutedEventArgs(RightClickEvent, this));
        }

        #endregion

        #region MonitorMouse

        public static readonly DependencyProperty MonitorMouseProperty =
            DependencyProperty.Register(nameof(MonitorMouse), typeof(bool), typeof(DynamicGrid));

        public bool MonitorMouse
        {
            get => (bool)GetValue(MonitorMouseProperty);
            set => SetValue(MonitorMouseProperty, value);
        }

        #endregion

        #region Columns

        public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.Register(nameof(Columns), typeof(int), typeof(DynamicGrid),
            new FrameworkPropertyMetadata(0,
                FrameworkPropertyMetadataOptions.AffectsMeasure 
                | FrameworkPropertyMetadataOptions.AffectsMeasure
                | FrameworkPropertyMetadataOptions.AffectsRender, ColumnsChanged));

        private static void ColumnsChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is DynamicGrid g)
            {
                g.ColumnsChanged((int)e.NewValue);
            }
        }

        private void ColumnsChanged(int value)
        {
            Debug.WriteLine($"DynamicGrid.Columns ColumnDefinition.Count(Before): {ColumnDefinitions.Count}");
            while (ColumnDefinitions.Count > value)
            {
                ColumnDefinitions.RemoveAt(0);
            }
            while (ColumnDefinitions.Count < value)
            {
                ColumnDefinitions.Add(new ColumnDefinition { Width = m_columnWidth });
            }
            Debug.WriteLine($"DynamicGrid.Columns ColumnDefinition.Count (After): {ColumnDefinitions.Count}");
        }

        public int Columns
        {
            get => (int) GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        #endregion

        #region ColumnWidth
        private GridLength m_columnWidth = new GridLength(1, GridUnitType.Star);

        public static readonly DependencyProperty ColumnWidthProperty =
        DependencyProperty.Register(
            nameof(ColumnWidth), typeof(int), typeof(DynamicGrid),
            new PropertyMetadata(-1, ColumnWidthChanged));

        private static void ColumnWidthChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is DynamicGrid g)
            {
                g.ColumnsWidthChanged((int)e.NewValue);
            }
        }

        private void ColumnsWidthChanged(int value)
        {
            m_columnWidth = value < 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(value);
            foreach (var cd in ColumnDefinitions)
            {
                cd.Width = m_columnWidth;
            }
        }

        public int ColumnWidth
        {
            get => (int) GetValue(ColumnWidthProperty);
            set => SetValue(ColumnWidthProperty, value);
        }

        #endregion

        #region Rows

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(nameof(Rows), typeof(int), typeof(DynamicGrid),
                new FrameworkPropertyMetadata (0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure 
                    | FrameworkPropertyMetadataOptions.AffectsMeasure
                    | FrameworkPropertyMetadataOptions.AffectsRender, RowsChanged));

        private static void RowsChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is DynamicGrid g)
            {
                g.RowsChanged((int)e.NewValue);
            }
        }

        private void RowsChanged(int value)
        {
            Debug.WriteLine($"DynamicGrid.Rows RowDefinitions.Count(Before): {RowDefinitions.Count}");
            while (RowDefinitions.Count > value)
            {
                RowDefinitions.RemoveAt(0);
            }
            while (RowDefinitions.Count < value)
            {
                RowDefinitions.Add(new RowDefinition { Height = m_rowHeight });
            }
            Debug.WriteLine($"DynamicGrid.Rows RowDefinitions.Count (After): {RowDefinitions.Count}");
        }

        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        #endregion

        #region RowHeight
        private GridLength m_rowHeight = new GridLength(1, GridUnitType.Star);

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register(nameof(RowHeight), typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(-1, RowHeightChanged));

        private static void RowHeightChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is DynamicGrid g)
            {
                g.RowHeightChanged((int) e.NewValue);
            }
        }

        private void RowHeightChanged(int value)
        {
            m_rowHeight = value < 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(value);
            foreach (var rd in RowDefinitions)
            {
                rd.Height = m_rowHeight;
            }
        }

        public int RowHeight
        {
            get => (int)GetValue(RowHeightProperty);
            set => SetValue(RowHeightProperty, value);
        }

        #endregion
    }
}
