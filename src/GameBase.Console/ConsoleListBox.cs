using GameBase.Model;
using System;
using SConsole = System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public class ConsoleListBox<CC> : AbstractConsoleItemsContainer<CC> where CC:AbstractConsoleComponent
    {
        public ConsoleListBox(string name = null) : base(name) { }

        public event EventHandler<ChangedValueArgs<bool>> IsHorizontalLayoutChanged;
        private bool m_isHorizontalLayout;
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

        protected override void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldStartingIndex == m_selectedIndex)
            {
                m_selectedIndex = -1;
            }
            base.ItemsSource_CollectionChanged(sender, e);
        }

        protected override bool ComponentKeyPressed(ConsoleKeyInfo key)
        {
            var dec = false;
            var inc = false;
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    dec = IsHorizontalLayout;
                    break;
                case ConsoleKey.RightArrow:
                    inc = IsHorizontalLayout;
                    break;
                case ConsoleKey.UpArrow:
                    dec = !IsHorizontalLayout;
                    break;
                case ConsoleKey.DownArrow:
                    inc = !IsHorizontalLayout;
                    break;
                case ConsoleKey.Enter:
                    SelectedIndex = overIndex;
                    break;
            }
            if (dec)
            {
                if (overIndex > 0)
                {
                    overIndex--;
                }
            }
            if (inc)
            {
                if (overIndex < ((ItemsSource?.Count - 1) ?? 0))
                {
                    overIndex++;
                }
            }
            return inc || dec;
        }

        private int m_overIndex = -1;
        private int overIndex
        {
            get => m_overIndex;
            set
            {
                var old = OverItem;
                m_overIndex = value;
                OverItem = overIndex >= 0 ? ItemsSource?[overIndex] : null;
            }
        }

        private int m_selectedIndex = -1;
        public int SelectedIndex
        {
            get => m_selectedIndex;
            set
            {
                var old = SelectedItem;
                m_selectedIndex = value;
                SelectedItem = SelectedIndex >= 0 ? ItemsSource?[SelectedIndex] : null;
            }
        }

        protected override void OnSelectionChanged(CC old, CC value)
        {
            m_selectedIndex = ItemsSource?.IndexOf(value) ?? -1;
            base.OnSelectionChanged(old, value);
        }

        private int m_itemWidth;
        private int m_itemHeight;

        protected override void ComponentLayout(ref int layoutWidth, ref int layoutHeight)
        {
            m_itemWidth = m_itemHeight = 0;
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                {
                    item.Layout(layoutWidth, layoutHeight);
                    m_itemWidth = Math.Max(m_itemWidth, item.ActualWidth);
                    m_itemHeight = Math.Max(m_itemHeight, item.ActualHeight);
                }
                if (IsHorizontalLayout)
                {
                    layoutHeight = m_itemHeight + 2;
                    layoutWidth = (m_itemWidth + 1) * ItemsSource.Count + 1;
                }
                else
                {
                    layoutWidth = m_itemWidth + 2;
                    layoutHeight = (m_itemHeight + 1) * ItemsSource.Count + 1;
                }
            }
            else
            {
                layoutWidth = layoutHeight = 0;
            }
        }

        protected override void OnIsEnabledChanged(bool oldVal, bool newVal)
        {
            if (newVal)
            {
                overIndex = SelectedIndex;
            }
            else
            {
                overIndex = -1;
            }
        }

        private int m_lastActiveIndex = -1;
        protected override void ComponentRender(int left, int top)
        {
            if (m_lastActiveIndex >= 0 && m_lastActiveIndex != overIndex)
            {
                if (IsHorizontalLayout)
                {
                    RenderBlank(left + m_lastActiveIndex * (m_itemWidth + 1), top, m_itemWidth * 2, m_itemHeight * 2);
                }
                else
                {
                    RenderBlank(left, top + m_lastActiveIndex * (m_itemHeight + 1), m_itemWidth * 2, m_itemHeight * 2);
                }
            }
            if (ItemsSource != null)
            {
                for (int i=0; i < ItemsSource.Count;i++)
                {
                    var item = ItemsSource[i];
                    int x = left + 1, y = top + 1;
                    if(IsHorizontalLayout)
                    {
                        x += (m_itemWidth + 1) * i;
                    }
                    else
                    {
                        y += (m_itemHeight + 1) * i;
                    }
                    item.Render(x, y, null, (SelectedIndex == i ? (ConsoleColor?)SelectionColor:null));
                }
            }
            if (overIndex >= 0)
            {
                if (IsHorizontalLayout)
                {
                    RenderBox(left + overIndex * (m_itemWidth + 1), top, m_itemWidth * 2, m_itemHeight * 2);
                }
                else
                {
                    RenderBox(left, top + overIndex * (m_itemHeight + 1), m_itemWidth * 2, m_itemHeight * 2);
                }
            }
            m_lastActiveIndex = overIndex;
        }
    }
}
