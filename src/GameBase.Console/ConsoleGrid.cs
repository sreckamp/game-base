using GameBase.Model;
using System;
using SConsole = System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GameBase.Console
{
    public class ConsoleGrid<CC> : AbstractConsoleItemsContainer<CC> where CC : AbstractConsoleElement
    {
        private readonly int m_columns;
        private readonly int m_rows;

        private Func<CC, int> m_getColumnFunc;
        private Func<CC, int> m_getRowFunc;

        public ConsoleGrid(int columns, int rows, string name = null, Func < CC, int> getColumnFunc = null, Func<CC, int> getRowFunc = null) : base(name)
        {
            m_colWidths = new int[columns];
            m_rowHeights = new int[rows];
            m_columns = columns;
            m_rows = rows;
            if (getColumnFunc != null)
            {
                m_getColumnFunc = getColumnFunc;
            }
            else
            {
                m_getColumnFunc = new Func<CC, int>((cc) =>
                {
                    int idx;
                    if ((idx = ItemsSource?.IndexOf(cc) ?? -1) >= 0)
                    {
                        return idx % m_columns;
                    }
                    return -1;
                });
            }
            if (getRowFunc != null)
            {
                m_getRowFunc = getRowFunc;
            }
            else
            {
                getRowFunc = new Func<CC, int>((cc) =>
                {
                    int idx;
                    if ((idx = ItemsSource?.IndexOf(cc) ?? -1) >= 0)
                    {
                        return idx / m_columns;
                    }
                    return -1;
                });
            }
        }

        protected override bool ComponentKeyPressed(ConsoleKeyInfo key)
        {
            var handled = false;
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if(activeColumn > 0)
                    {
                        activeColumn--;
                        activeRow = Math.Max(activeRow, 0);
                    }
                    handled = true;
                    break;
                case ConsoleKey.RightArrow:
                    if (activeColumn < m_columns -1)
                    {
                        activeColumn++;
                        activeRow = Math.Max(activeRow, 0);
                    }
                    handled = true;
                    break;
                case ConsoleKey.UpArrow:
                    if (activeRow > 0)
                    {
                        activeColumn = Math.Max(activeColumn, 0);
                        activeRow--;
                    }
                    handled = true;
                    break;
                case ConsoleKey.DownArrow:
                    if (activeRow < m_rows - 1)
                    {
                        activeColumn = Math.Max(activeColumn, 0);
                        activeRow++;
                    }
                    handled = true;
                    break;
                case ConsoleKey.Enter:
                    SelectedLocation = new Point(activeColumn, activeRow);
                    break;
            }
            return handled;
        }

        private int m_activeColumn = -1;
        private int activeColumn
        {
            get => m_activeColumn;
            set
            {
                m_activeColumn = value;
                OverItem = getItemAt(m_activeRow, m_activeColumn);
                InvalidateLayout();
            }
        }

        private int m_activeRow = -1;
        private int activeRow
        {
            get => m_activeRow;
            set
            {
                m_activeRow = value;
                OverItem = getItemAt(m_activeRow, m_activeColumn);
                InvalidateLayout();
            }
        }

        private CC getItemAt(int row, int column)
        {
            for (int idx = 0; idx < (ItemsSource?.Count ?? 0); idx++)
            {
                var i = ItemsSource[idx];
                if (m_getRowFunc(i) == row && m_getColumnFunc(i) == column) return i;
            }
            return null;
        }

        public EventHandler<ChangedValueArgs<Point>> SelectedLocationChanged;
        private Point m_selectedLocation = new Point(-1,-1);
        public Point SelectedLocation
        {
            get => m_selectedLocation;
            set
            {
                var old = SelectedLocation;
                m_selectedLocation = value;
                SelectedItem = getItemAt(value.X, value.Y);
                SelectedLocationChanged?.Invoke(this, new ChangedValueArgs<Point>(old, value));
                InvalidateLayout();
            }
        }

        protected override void OnIsEnabledChanged(bool oldVal, bool newVal)
        {
            if (newVal)
            {
                activeColumn = SelectedLocation.X;
                activeRow = SelectedLocation.Y;
            }
            else
            {
                activeColumn = activeRow = -1;
            }
        }

        private int[] m_colWidths;
        private int[] m_rowHeights;

        //protected override void ComponentLayout(ref int layoutWidth, ref int layoutHeight)
        //{
        //    var width = 0;
        //    var height = 0;
        //    for (int i = 0; i < m_colWidths.Length; i++)
        //    {
        //        m_colWidths[i] = layoutWidth < 0 ? layoutWidth : (layoutWidth - (m_columns + 1)) / m_columns;
        //    }
        //    for (int i = 0; i < m_rowHeights.Length; i++)
        //    {
        //        m_rowHeights[i] = layoutHeight < 0 ? layoutHeight : (layoutHeight - (m_rows + 1)) / m_rows;
        //    }
        //    if (ItemsSource != null)
        //    {
        //        foreach (var item in ItemsSource)
        //        {
        //            int col = m_getColumnFunc(item);
        //            int row = m_getRowFunc(item);
        //            item.Layout(layoutWidth < 0 ? layoutWidth : layoutWidth / m_rows, layoutHeight < 0 ? layoutHeight : layoutHeight / m_columns);
        //            m_colWidths[col] = Math.Max(m_colWidths[col], item.ActualWidth + Padding.Left + Padding.Right);
        //            m_rowHeights[row] = Math.Max(m_rowHeights[row], item.ActualHeight + Padding.Top+Padding.Bottom);
        //        }
        //        width = 1;
        //        foreach(var cw in m_colWidths)
        //        {
        //            width += cw + 1;
        //        }
        //        height = 1;
        //        foreach (var rh in m_rowHeights)
        //        {
        //            height += rh + 1;
        //        }
        //        if (width > 1 && height > 1)
        //        {
        //            layoutHeight = height;
        //            layoutWidth = width;
        //        }
        //    }
        //}

        //protected override void ComponentRender(int left, int top)
        //{
        //    string topLine = "\u250C";
        //    string gridLineH = "\u251C";
        //    string gridLineV = "\u2502";
        //    string bottomLine = "\u2514";

        //    for (int idx = 0; idx < m_colWidths.Length; idx++)
        //    {
        //        for(int j = 0; j<m_colWidths[idx];j++)
        //        {
        //            topLine += "\u2500";
        //            gridLineH += "\u2500";
        //            gridLineV += " ";
        //            bottomLine += "\u2500";
        //        }
        //        gridLineV += "\u2502";
        //        if (idx == m_colWidths.Length-1)
        //        {
        //            topLine += "\u2510";
        //            gridLineH += "\u2524";
        //            bottomLine += "\u2518";
        //        }
        //        else
        //        {
        //            topLine += "\u252C";
        //            gridLineH += "\u253C";
        //            bottomLine += "\u2534";
        //        }
        //    }

        //    int r = top;
        //    for (int idx = 0; idx < m_rowHeights.Length; idx++)
        //    {
        //        SConsole.SetCursorPosition(left, r++);
        //        if(idx == 0)
        //        {
        //            SConsole.Write(topLine);
        //        }
        //        else
        //        {
        //            SConsole.Write(gridLineH);
        //        }
        //        for (int j = 0; j < m_rowHeights[idx]; j++)
        //        {
        //            SConsole.SetCursorPosition(left, r++);
        //            SConsole.Write(gridLineV);
        //        }
        //    }
        //    SConsole.SetCursorPosition(left, r);
        //    SConsole.Write(bottomLine);

        //    if (ItemsSource != null)
        //    {
        //        for (int i = 0; i < ItemsSource.Count; i++)
        //        {
        //            var item = ItemsSource[i];
        //            int x = left + 1, y = top + 1;
        //            int col = m_getColumnFunc(item);
        //            int row = m_getRowFunc(item);
        //            if (col >= 0 && row >= 0)
        //            {
        //                int j = 0;
        //                for (j = 0; j < col; j++)
        //                {
        //                    x += m_colWidths[j] + 1;
        //                }
        //                x += (m_colWidths[j] - item.ActualWidth) / 2;
        //                for (j = 0; j < row; j++)
        //                {
        //                    y += m_rowHeights[j] + 1;
        //                }
        //                x += (m_rowHeights[j] - item.ActualHeight) / 2;
        //            }
        //            item.Render(x, y, null, (activeColumn == col && activeRow == row) ? (ConsoleColor?)SelectionColor : null);
        //        }
        //    }
        //}

        public override Size MeasureOverride(Size availableSize)
        {
            throw new NotImplementedException();
        }

        public override Size ArrangeOverride(Size availableSize)
        {
            throw new NotImplementedException();
        }

        public override void RenderOverride(IConsoleContext context)
        {
            throw new NotImplementedException();
        }
    }
}
