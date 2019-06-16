using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using DPoint = System.Drawing.Point;

namespace GameBase.WPF
{
    public class DynamicGrid : Grid
    {
        public DynamicGrid()
        {
            Background = new SolidColorBrush(Colors.Transparent);
        }

        #region OverCell

        public static readonly RoutedEvent OverCellEvent = EventManager.RegisterRoutedEvent(
            "OverCell", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DynamicGrid));

        public event RoutedEventHandler OverCell
        {
            add { AddHandler(OverCellEvent, value); }
            remove { RemoveHandler(OverCellEvent, value); }
        }

        private DPoint m_lastCell = new DPoint(-1, -1);

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            cellEntered(-1, -1);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var p = e.GetPosition(this);
            if (p.X >= 0 && p.Y >= 0)
            {
                int row = 0, column = 0;
                int x = 0, y = 0;
                foreach (var cd in ColumnDefinitions)
                {
                    x += (int)cd.ActualWidth;
                    if (p.X < x)
                    {
                        break;
                    }
                    column++;
                }
                foreach (var rd in RowDefinitions)
                {
                    y += (int)rd.ActualHeight;
                    if (p.Y < y)
                    {
                        break;
                    }
                    row++;
                }
                if (row < RowDefinitions.Count && column < ColumnDefinitions.Count)
                {
                    cellEntered(column, row);
                }
            }
        }

        private void cellEntered(int column, int row)
        {
            var gc = new DPoint(column, row);
            if (gc != m_lastCell)
            {
                m_lastCell = gc;
                var args = new GridCellRoutedEventArgs(OverCellEvent, gc);
                RaiseEvent(args);
            }
        }

        #endregion

        #region Columns

        public static readonly DependencyProperty ColumnsProperty =
        DependencyProperty.RegisterAttached("Columns", typeof(int), typeof(DynamicGrid),
            new PropertyMetadata(0, ColumnsChanged));

        public static void ColumnsChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is DynamicGrid g)
            {
                g.Columns = (int)e.NewValue;
            }
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set
            {
                while (ColumnDefinitions.Count > value)
                {
                    ColumnDefinitions.RemoveAt(0);
                }
                while (ColumnDefinitions.Count < value)
                {
                    ColumnDefinitions.Add(new ColumnDefinition() { Width = m_columnWidth });
                }
                SetValue(ColumnsProperty, value);
            }
        }

        #endregion

        #region ColumnWidth
        private GridLength m_columnWidth = new GridLength(1, GridUnitType.Star);

        public static readonly DependencyProperty ColumnWidthProperty =
        DependencyProperty.RegisterAttached(
            "ColumnWidth", typeof(int), typeof(DynamicGrid),
            new PropertyMetadata(-1, ColumnWidthChanged));

        public static void ColumnWidthChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is DynamicGrid g)
            {
                g.ColumnWidth = (int)e.NewValue;
            }
        }

        public int ColumnWidth
        {
            get { return (int)GetValue(ColumnWidthProperty); }
            set
            {
                if (value < 0)
                {
                    m_columnWidth = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    m_columnWidth = new GridLength(value);
                }
                foreach (var cd in ColumnDefinitions)
                {
                    cd.Width = m_columnWidth;
                }
                SetValue(ColumnWidthProperty, value);
            }
        }
        #endregion

        #region Rows

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.RegisterAttached("Rows", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(0, RowsChanged));

        public static void RowsChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is DynamicGrid g)
            {
                g.Rows = (int)e.NewValue;
            }
        }

        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set
            {
                while (RowDefinitions.Count > value)
                {
                    RowDefinitions.RemoveAt(0);
                }
                while (RowDefinitions.Count < value)
                {
                    RowDefinitions.Add(new RowDefinition() { Height = m_rowHeight });
                }
                SetValue(RowsProperty, value);
            }
        }

        #endregion

        #region RowHeight
        private GridLength m_rowHeight = new GridLength(1, GridUnitType.Star);

        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.RegisterAttached("RowHeight", typeof(int), typeof(DynamicGrid),
                new PropertyMetadata(-1, RowHeightChanged));

        public static void RowHeightChanged(DependencyObject obj,
            DependencyPropertyChangedEventArgs e)
        {
            if (obj is DynamicGrid g)
            {
                g.RowHeight = (int)e.NewValue;
            }
        }

        public int RowHeight
        {
            get { return (int)GetValue(RowHeightProperty); }

            set
            {
                if (value < 0)
                {
                    m_rowHeight = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    m_rowHeight = new GridLength(value);
                }
                foreach (var rd in RowDefinitions)
                {
                    rd.Height = m_rowHeight;
                }
                SetValue(RowHeightProperty, value);
            }
        }

        #endregion
    }
}
