using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DPoint = System.Drawing.Point;

namespace GameBase.WPF
{
    public class GridCellRoutedEventArgs:RoutedEventArgs
    {
        public GridCellRoutedEventArgs(RoutedEvent @event, DPoint cell) : base(@event)
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

        public static readonly RoutedEvent LeftClickCellEvent = EventManager.RegisterRoutedEvent(
            nameof(LeftClickCellEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameGrid));

        public event RoutedEventHandler LeftClickCell
        {
            add => AddHandler(LeftClickCellEvent, value);
            remove => RemoveHandler(LeftClickCellEvent, value);
        }

        public static readonly RoutedEvent RightClickCellEvent = EventManager.RegisterRoutedEvent(
            nameof(RightClickCellEvent), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameGrid));

        public event RoutedEventHandler RightClickCell
        {
            add => AddHandler(RightClickCellEvent, value);
            remove => RemoveHandler(RightClickCellEvent, value);
        }

        public GameGrid()
        {
            OverCell += (sender, args) => { };
            RightClickCell += (sender, args) => { };
            LeftClickCell += (sender, args) => { };
            Background = new SolidColorBrush(Colors.Transparent);
            Background.Freeze();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            RaiseEvent(new GridCellRoutedEventArgs(LeftClickCellEvent, m_lastGrid));
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);
            RaiseEvent(new GridCellRoutedEventArgs(RightClickCellEvent, m_lastGrid));
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            var gc = new DPoint(int.MinValue, int.MinValue);
            if (gc == m_lastGrid) return;
            m_lastGrid = gc;
            var args = new GridCellRoutedEventArgs(OverCellEvent, gc);
            RaiseEvent(args);
        }

        private DPoint m_lastGrid = new DPoint(-1,-1);
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var p = e.GetPosition(this);
            if (!(p.X >= 0) || !(p.Y >= 0)) return;
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

            if (row >= RowDefinitions.Count || column >= ColumnDefinitions.Count) return;
            var gc = new DPoint(column, row);
            if (gc == m_lastGrid) return;
            m_lastGrid = gc;
            var args = new GridCellRoutedEventArgs(OverCellEvent, gc);
            RaiseEvent(args);
        }
    }
}
