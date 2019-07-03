using GameBase.Model;
using System;
using SConsole = System.Console;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public abstract class AbstractConsoleComponent
    {
        protected AbstractConsoleComponent(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public event EventHandler<ChangedValueArgs<int>> ActualWidthChanged;
        private int m_actualWidth;
        public int ActualWidth
        {
            get => m_actualWidth;
            private set
            {
                var old = m_actualWidth;
                m_actualWidth = value;
                ActualWidthChanged?.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<int>> ActualHeightChanged;
        private int m_actualHeight;
        public int ActualHeight
        {
            get => m_actualHeight;
            set
            {
                var old = m_actualHeight;
                m_actualHeight = value;
                ActualHeightChanged?.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<int>> WidthChanged;
        private int m_width = -1;
        public int Width
        {
            get => m_width;
            set
            {
                var old = m_width;
                m_width = value;
                InvalidateLayout();
                WidthChanged?.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<int>> HeightChanged;
        private int m_height = -1;
        public int Height
        {
            get => m_height;
            set
            {
                var old = m_height;
                m_height = value;
                InvalidateLayout();
                HeightChanged?.Invoke(this, new ChangedValueArgs<int>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<ConsoleSpacing>> MarginChanged;
        private ConsoleSpacing m_margin;
        public ConsoleSpacing Margin
        {
            get => m_margin;
            set
            {
                var old = m_margin;
                m_margin = value;
                if (old != value)
                {
                    InvalidateLayout();
                    MarginChanged?.Invoke(this, new ChangedValueArgs<ConsoleSpacing>(old, value));
                }
            }
        }

        public event EventHandler<ChangedValueArgs<ConsoleColor>> BackgroundChanged;
        private ConsoleColor m_background = ConsoleColor.Black;
        public ConsoleColor BackgroundColor
        {
            get => m_background;
            set
            {
                var old = m_background;
                m_background = value;
                BackgroundChanged?.Invoke(this, new ChangedValueArgs<ConsoleColor>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<bool>> HasBackgroundChanged;
        private bool m_hasBackground = true;
        public bool HasBackground
        {
            get => m_hasBackground;
            set
            {
                var old = m_hasBackground;
                m_hasBackground = value;
                HasBackgroundChanged?.Invoke(this, new ChangedValueArgs<bool>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<ConsoleColor>> ForegroundChanged;
        private ConsoleColor m_foreground = ConsoleColor.White;
        public ConsoleColor ForegroundColor
        {
            get => m_foreground;
            set
            {
                var old = m_foreground;
                m_foreground = value;
                ForegroundChanged?.Invoke(this, new ChangedValueArgs<ConsoleColor>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<bool>> IsEnabledChanged;
        private bool m_isEnabled = true;
        public bool IsEnabled
        {
            get => m_isEnabled;
            set
            {
                var old = m_isEnabled;
                m_isEnabled = value;
                OnIsEnabledChanged(old, value);
                IsEnabledChanged?.Invoke(this, new ChangedValueArgs<bool>(old, value));
            }
        }

        protected virtual void OnIsEnabledChanged(bool oldVal, bool newVal) { }

        public event EventHandler<ChangedValueArgs<bool>> IsVisibleChanged;
        private bool m_isVisible = true;
        public bool IsVisible
        {
            get => m_isVisible;
            set
            {
                var old = m_isVisible;
                m_isVisible = value;
                IsVisibleChanged?.Invoke(this, new ChangedValueArgs<bool>(old, value));
            }
        }

        private bool m_layoutRequred = true;

        public event EventHandler Invalidated;
        public virtual void InvalidateLayout()
        {
            m_layoutRequred = true;
            Invalidated?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Prepare the layout of the control
        /// </summary>
        /// <param name="targetWidth">The target width for the control.</param>
        /// <param name="targetHeight">The target height for the control.</param>
        /// <returns>true if the layout has been updated.</returns>
        public bool Layout(int targetWidth, int targetHeight)
        {
            int layoutWidth = Width;
            int layoutHeight = Height;

            if(layoutWidth < 0)
            {
                layoutWidth = targetWidth;
            }
            if (layoutHeight < 0)
            {
                layoutHeight = targetHeight;
            }

            var layoutUpdated = m_layoutRequred;
            if(m_layoutRequred)
            {
                ComponentLayout(ref layoutWidth, ref layoutHeight);
                if(layoutWidth >= 0 && layoutHeight >=0)
                {
                    if (layoutHeight != Height)
                    {
                        layoutHeight += Margin.Top + Margin.Bottom;
                    }
                    ActualHeight = layoutHeight;
                    if (layoutWidth != Width)
                    {
                        layoutWidth += Margin.Left + Margin.Right;
                    }
                    ActualWidth = layoutWidth;
                }
                else
                {
                    layoutUpdated = false;
                }
                m_layoutRequred = false;
            }
            return layoutUpdated;
        }

        protected abstract void ComponentLayout(ref int layoutWidth, ref int layoutHeight);

        private int m_renderedTop = -1;
        private int m_renderedLeft = -1;
        private int m_renderedWidth = -1;
        private int m_renderedHeight = -1;
        /// <summary>
        /// Draw the control on the console using the left and top provided.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        public void Render(int left, int top, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (IsVisible && ActualHeight > 0 && ActualWidth > 0)
            {
                var bg = SConsole.BackgroundColor;
                var fg = SConsole.ForegroundColor;
                SConsole.ForegroundColor = foreground ?? ForegroundColor;
                SConsole.BackgroundColor = background ?? BackgroundColor;

                if (HasBackground && (m_renderedTop != top || m_renderedLeft != left || m_renderedHeight < ActualHeight))
                {
                    RenderBlank(left, top, ActualWidth, ActualHeight);
                }
                ComponentRender(left + Margin.Left, top + Margin.Top);
                if (m_renderedWidth > 0 && ActualWidth < m_renderedWidth)
                {
                    RenderBlank(left + Margin.Left + ActualWidth, top + Margin.Top, m_renderedWidth - ActualWidth, ActualHeight);
                }
                if (m_renderedHeight > 0 && ActualHeight < m_renderedHeight)
                {
                    RenderBlank(left + Margin.Left, top + Margin.Top + ActualHeight, Math.Max(m_renderedWidth, ActualWidth), m_renderedHeight - ActualHeight);
                }
                m_renderedTop = top;
                m_renderedLeft = left;
                m_renderedWidth = ActualWidth;
                m_renderedHeight = ActualHeight;
                SConsole.ForegroundColor = fg;
                SConsole.BackgroundColor = bg;
            }
        }

        protected abstract void ComponentRender(int left, int top);

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

        protected void RenderBlank(int left, int top, int width, int height, ConsoleColor? background = null)
        {
            if (width > 0)
            {
                var padding = string.Empty.PadLeft(width);
                SConsole.BackgroundColor = background ?? BackgroundColor;
                for (int y = 0; y < height; y++)
                {
                    SConsole.SetCursorPosition(left, top + y);
                    SConsole.Write(padding);
                }
                SConsole.BackgroundColor = BackgroundColor;
            }
        }
        protected void RenderBox(int left, int top, int width, int height, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (width > 0)
            {
                SConsole.ForegroundColor = foreground ?? ForegroundColor;
                SConsole.BackgroundColor = background ?? BackgroundColor;
                var right = left + width - 1;
                var bottom = top + height - 1;
                SConsole.SetCursorPosition(left, top);
                SConsole.Write("\u250C");
                SConsole.SetCursorPosition(right, top);
                SConsole.Write("\u2510");
                SConsole.SetCursorPosition(left, bottom);
                SConsole.Write("\u2514");
                SConsole.SetCursorPosition(right, bottom);
                SConsole.Write("\u2518");
                for (int i = 1; i < width - 1; i++)
                {
                    SConsole.SetCursorPosition(left + i, top);
                    SConsole.Write("\u2500");
                    SConsole.SetCursorPosition(left + i, bottom);
                    SConsole.Write("\u2500");
                }
                for (int i = 1; i < height - 1; i++)
                {
                    SConsole.SetCursorPosition(left, top + i);
                    SConsole.Write("\u2502");
                    SConsole.SetCursorPosition(right, top + i);
                    SConsole.Write("\u2502");
                }
                SConsole.BackgroundColor = ForegroundColor;
                SConsole.ForegroundColor = BackgroundColor;
            }
        }

        public override string ToString()
        {
            if (Name != null) return Name;
            return base.ToString();
        }
    }
}
