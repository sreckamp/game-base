using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DPoint = System.Drawing.Point;

namespace GameBase.WPF
{
    public class GridCellRoutedEventArgs:RoutedEventArgs
    {
        public GridCellRoutedEventArgs(RoutedEvent evnt, DPoint cell) : base(evnt)
        {
            Cell = cell;
        }
        public DPoint Cell { get; }
    }

    public class GameGrid : Grid
    {
        public static readonly RoutedEvent OverCellEvent = EventManager.RegisterRoutedEvent(
            nameof(OverCell), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameGrid));

        public event RoutedEventHandler OverCell
        {
            add => AddHandler(OverCellEvent, value);
            remove => RemoveHandler(OverCellEvent, value);
        }

        public GameGrid()
        {
            OverCell += (sender, args) => { };
            Background = new SolidColorBrush(Colors.Transparent);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            var gc = new DPoint(-1, -1);
            if (gc != m_lastGrid)
            {
                m_lastGrid = gc;
                var args = new GridCellRoutedEventArgs(OverCellEvent, gc);
                RaiseEvent(args);
            }
        }

        private DPoint m_lastGrid = new DPoint(-1,-1);
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
                    var gc = new DPoint(column, row);
                    if (gc != m_lastGrid)
                    {
                        m_lastGrid = gc;
                        var args = new GridCellRoutedEventArgs(OverCellEvent, gc);
                        RaiseEvent(args);
                    }
                }
            }
        }
    }
}
