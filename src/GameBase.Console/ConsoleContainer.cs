using GameBase.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public class ConsoleContainer:AbstractConsoleComponent
    {
        protected Dictionary<AbstractConsoleComponent, RenderState> m_renderStates = new Dictionary<AbstractConsoleComponent, RenderState>();
        public ConsoleContainer(string name = null) : base(name)
        {
            Children = new ObservableList<AbstractConsoleComponent>();
            Children.CollectionChanged += childrenChanged;
        }

        private void childrenChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.NewItems != null)
            {
                foreach (AbstractConsoleComponent cc in e.NewItems)
                {
                    cc.Invalidated += childLayoutInvalidated;
                    m_renderStates.Add(cc, new RenderState());
                }
            }
            if (e.OldItems != null)
            {
                foreach (AbstractConsoleComponent cc in e.OldItems)
                {
                    cc.Invalidated -= childLayoutInvalidated;
                    m_renderStates.Remove(cc);
                }
            }
            InvalidateLayout();
        }

        private void childLayoutInvalidated(object sender, EventArgs e)
        {
            Debug.WriteLine(string.Format("{0} Invalidated", sender));
            InvalidateLayout();
        }

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

        public ObservableList<AbstractConsoleComponent> Children { get; private set; }

        protected override void ComponentLayout(ref int layoutWidth, ref int layoutHeight)
        {
            int width = 0;
            int height = 0;
            bool layoutChanged = false;
            foreach (var c in Children)
            {
                if(c.Layout(-1, -1))
                {
                    layoutChanged = true;
                }
                UpdateBounds(c.ActualWidth + (Padding.Left + Padding.Right), c.ActualHeight + (Padding.Top + Padding.Bottom), ref width, ref height);
                m_renderStates[c].IsRenderRequired = layoutChanged;
            }
            if (width > 0 && height > 0)
            {
                layoutWidth = width;
                layoutHeight = height;
            }
        }

        protected virtual void UpdateBounds(int layoutWidth, int layoutHeight, ref int width, ref int height)
        {
            width = Math.Max(width, layoutWidth);
            height = Math.Max(height, layoutHeight);
        }

        protected override void ComponentRender(int left, int top)
        {
            foreach (var c in Children)
            {
                if (m_renderStates[c].IsRenderRequired)
                {
                    m_renderStates[c].IsRenderRequired = false;
                    c.Render(left + Padding.Left, top + Padding.Top);
                }
                PostRender(c, ref left, ref top);
            }
        }

        protected virtual void PostRender(AbstractConsoleComponent component, ref int left, ref int top)
        {
        }

        protected override bool ComponentLineEntered(string line)
        {
            foreach (var c in Children)
            {
                var used = c.LineEntered(line);
                if (used) return used;
            }
            return false;
        }

        protected override bool ComponentKeyPressed(ConsoleKeyInfo key)
        {
            foreach (var c in Children)
            {
                var used = c.KeyPressed(key);
                if (used) return used;
            }
            return false;
        }

        protected class RenderState
        {
            public bool IsRenderRequired = true;
            public int LastHeight = -1;
            public int LastWidth = -1;
        }
    }
}
