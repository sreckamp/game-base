using GameBase.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public abstract class AbstractConsoleItemsContainer<CC> : AbstractConsoleElement where CC : AbstractConsoleElement
    {
        public AbstractConsoleItemsContainer(string name) : base(name) { }

        public event EventHandler<ChangedValueArgs<ConsoleSpacing>> PaddingChanged;
        private ConsoleSpacing m_padding;
        public ConsoleSpacing Padding
        {
            get => m_padding;
            set
            {
                var old = m_padding;
                m_padding = value;
                PaddingChanged?.Invoke(this, new ChangedValueArgs<ConsoleSpacing>(old, value));
                InvalidateLayout();
            }
        }

        private IObservableList<CC> m_itemsSource;
        public IObservableList<CC> ItemsSource
        {
            get => m_itemsSource;
            set
            {
                if (m_itemsSource != null)
                {
                    m_itemsSource.CollectionChanged -= ItemsSource_CollectionChanged;
                }
                m_itemsSource = value;
                if (m_itemsSource != null)
                {
                    m_itemsSource.CollectionChanged += ItemsSource_CollectionChanged;
                }
            }
        }

        protected virtual void ItemsSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InvalidateLayout();
        }

        public event EventHandler<ChangedValueArgs<ConsoleColor>> SelectionColorChanged;
        private ConsoleColor m_selectionColor = ConsoleColor.Blue;
        public ConsoleColor SelectionColor
        {
            get => m_selectionColor;
            set
            {
                var old = m_selectionColor;
                m_selectionColor = value;
                SelectionColorChanged?.Invoke(this, new ChangedValueArgs<ConsoleColor>(old, value));
            }
        }

        public EventHandler<ChangedValueArgs<CC>> OverItemChanged;

        private CC m_overItem;
        public CC OverItem
        {
            get => m_overItem;
            protected set
            {
                var old = OverItem;
                m_overItem = value;
                OverItemChanged?.Invoke(this, new ChangedValueArgs<CC>(old, value));
                InvalidateLayout();
            }
        }

        public EventHandler<ChangedValueArgs<CC>> SelectionChanged;

        private CC m_selectedItem;
        public CC SelectedItem
        {
            get => m_selectedItem;
            set
            {
                var old = OverItem;
                m_selectedItem = value;
                OnSelectionChanged(old, value);
            }
        }

        protected virtual void OnSelectionChanged(CC old, CC value)
        {
            SelectionChanged?.Invoke(this, new ChangedValueArgs<CC>(old, value));
            InvalidateLayout();
        }
    }
}
