using System;
using SConsole = System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Drawing;
//using GameBase.Model;

namespace GameBase.Console
{
    /// <summary>
    /// Create some rudimentary layering & heirarchy to generate the visuals in display
    /// Use some mechanism for selection & active item, etc.
    /// </summary>
    public class ConsoleWindow
    {
        //private ConsoleBuffer[] m_buffers = new ConsoleBuffer[] { new ConsoleBuffer(), new ConsoleBuffer() };
        //private int m_active = 0;
        //private int m_next => (m_active + 1) % m_buffers.Length;
        private readonly ConsoleContainer m_main;
        public ConsoleWindow(ConsoleContainer main)
        {
            m_main = main;
        }

        public void Run()
        {
            Task.Run((Action)watchKeys);
            Task.Run((Action)watchWindow);
            if (m_main is IRunnable r)
            {
                r.Run();
            }
        }

        private void watchKeys()
        {
            var line = new StringBuilder();
            while (true)
            {
                var k = SConsole.ReadKey(true);
                if (k.Key == ConsoleKey.Enter)
                {
                    m_main?.LineEntered(line.ToString());
                    line.Clear();
                }
                else
                {
                    line.Append(k.KeyChar);
                }
                m_main?.KeyPressed(k);
            }
        }

        private void watchWindow()
        {
            //var ctx = new DirectConsoleContext();
            var buf = new ConsoleBuffer();
            var ctx = new BufferConsoleContext(buf);
            Size available = new Size(-1, -1);
            SConsole.CursorVisible = false;
            while (true)
            {
                int cWidth = SConsole.WindowWidth;
                int cHeight = SConsole.WindowHeight;
                if (cWidth != available.Width || cHeight != available.Height)
                {
                    //SConsole.SetBufferSize(cWidth, cHeight);
                    //if(cWidth < available.Width || cHeight < available.Height)
                    //{
                    //    RenderTools.RenderBlank(ctx, 0, 0, available.Height, available.Width);
                    //}
                    available = new Size(cWidth, cHeight);
                    Debug.WriteLine($"Resize {available}");
                    buf.Resize(cWidth, cHeight);
                    //m_main.InvalidateMeasure();
                    m_main.Measure(available);
                    m_main.Arrange(new Rectangle(new Point(), available));
                    //RenderTools.RenderBlank(ctx, 0, 0, available.Width, available.Height);
                    m_main.Render(ctx);
                    ctx.Paint();
                }
                //if (m_main.Layout(width, height))
                //{
                //    m_main.Paint(ctx);
                //    Debug.WriteLine("Render!");
                //    m_main.Render(0, 0);
                //}
                // **** DOUBLE BUFFERED ****
                //if (SConsole.WindowWidth != m_buffers[m_next].Width || SConsole.WindowHeight != m_buffers[m_next].Height)
                //{
                //    m_buffers[m_next].Resize(SConsole.WindowWidth, SConsole.WindowHeight);
                //    m_main.InvalidateLayout();
                //}
                //if (m_main.Layout(m_buffers[m_next]))
                //{
                //    Debug.WriteLine("Render!");
                //    m_active = m_next;
                //    m_main.Render(m_buffers[m_active]);
                //}
                System.Threading.Thread.Sleep(25);
            }
        }
    }

    public interface IConsoleContext
    {
        void SetForegroundColor(ConsoleColor? color);
        void PopForegroundColor();
        void SetBackgroundColor(ConsoleColor? color);
        void PopBackgroundColor();
        void Write(char character, int left, int top);
        void Write(string text, int left, int top);
    }

    public class BufferConsoleContext : IConsoleContext
    {
        private readonly ConsoleBuffer m_buffer;
        private readonly Stack<ConsoleColor?> m_backgroundStack = new Stack<ConsoleColor?>();
        private readonly Stack<ConsoleColor?> m_foregroundStack = new Stack<ConsoleColor?>();
        private ConsoleColor? m_backgroundColor = ConsoleColor.Black;
        private ConsoleColor? m_foregroundColor = ConsoleColor.White;

        public BufferConsoleContext(ConsoleBuffer buffer)
        {
            m_buffer = buffer;
        }

        public void PopBackgroundColor()
        {
            if (m_backgroundStack.Count > 0)
            {
                m_backgroundColor = m_backgroundStack.Pop();
            }
        }

        public void PopForegroundColor()
        {
            if (m_foregroundStack.Count > 0)
            {
                m_foregroundColor = m_foregroundStack.Pop();
            }
        }

        public void SetBackgroundColor(ConsoleColor? color)
        {
            m_backgroundStack.Push(m_backgroundColor);
            m_backgroundColor = color;
        }

        public void SetForegroundColor(ConsoleColor? color)
        {
            m_foregroundStack.Push(m_foregroundColor);
            m_foregroundColor = color;
        }

        public void Write(char character, int left, int top)
        {
            if (left >= 0 && left < m_buffer.Width && top >= 0 && top < m_buffer.Height)
            {
                m_buffer.SetCharacter(left, top, character, m_foregroundColor, m_backgroundColor);
            }
        }

        public void Write(string text, int left, int top)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Write(text[i], left + i, top);
            }
        }

        public void Paint()
        {
            ConsoleColor fg = SConsole.ForegroundColor;
            ConsoleColor bg = SConsole.BackgroundColor;
            SConsole.SetCursorPosition(0, 0);
            for (int row = 0; row < m_buffer.Height && row < SConsole.WindowHeight; row++)
            {
                for (int col = 0; col < m_buffer.Width && col < SConsole.WindowWidth; col++)
                {
                    var cc = m_buffer[col, row];
                    if (cc.ForegroundColor != fg)
                    {
                        SConsole.ForegroundColor = fg = cc.ForegroundColor;
                    }
                    if (cc.BackgroundColor != bg)
                    {
                        SConsole.BackgroundColor = bg = cc.BackgroundColor;
                    }
                    SConsole.Write(cc.Char);
                }
            }
            SConsole.WindowTop = 0;
        }
    }

    public class DirectConsoleContext : IConsoleContext
    {
        private readonly Stack<ConsoleColor> m_backgroundStack = new Stack<ConsoleColor>();
        private readonly Stack<ConsoleColor> m_foregroundStack = new Stack<ConsoleColor>();

        public void SetForegroundColor(ConsoleColor? color)
        {
            m_foregroundStack.Push(SConsole.ForegroundColor);
            if (color != null)
            {
                SConsole.ForegroundColor = (ConsoleColor)color;
            }
        }

        public void PopForegroundColor()
        {
            if (m_foregroundStack.Count > 0)
            {
                SConsole.ForegroundColor = m_foregroundStack.Pop();
            }
        }

        public void SetBackgroundColor(ConsoleColor? color)
        {
            m_backgroundStack.Push(SConsole.BackgroundColor);
            if (color != null)
            {
                SConsole.BackgroundColor = (ConsoleColor)color;
            }
        }

        public void PopBackgroundColor()
        {
            if (m_backgroundStack.Count > 0)
            {
                SConsole.BackgroundColor = m_backgroundStack.Pop();
            }
        }

        public void Write(char character, int left, int top)
        {
            SConsole.SetCursorPosition(left, top);
            SConsole.Write(character);
        }
        public void Write(string text, int left, int top)
        {
            SConsole.SetCursorPosition(left, top);
            SConsole.Write(text);
        }
    }

    public class ConsoleCharacter
    {
        public ConsoleCharacter(char c, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
        {
            Char = c;
            ForegroundColor = foreground;
            BackgroundColor = background;
        }
        public char Char;
        public ConsoleColor ForegroundColor;
        public ConsoleColor BackgroundColor;

        public override string ToString()
        {
            return $"{Char}:{toAbbreviation(ForegroundColor)}/{toAbbreviation(BackgroundColor)}";
        }

        private string toAbbreviation(ConsoleColor? color)
        {
            return m_abbreviationForColor.ContainsKey(color) ? m_abbreviationForColor[color] : "--";
        }

        private Dictionary<ConsoleColor?, string> m_abbreviationForColor = new Dictionary<ConsoleColor?, string>()
        {
            { ConsoleColor.Black," K" },
            { ConsoleColor.Blue," B" },
            {ConsoleColor.Cyan," C" },
            {ConsoleColor.DarkBlue,"dB" },
            {ConsoleColor.DarkCyan,"dC" },
            {ConsoleColor.DarkGray,"dG" },
            {ConsoleColor.DarkGreen,"dN" },
            {ConsoleColor.DarkMagenta,"dM" },
            {ConsoleColor.DarkRed,"dR" },
            {ConsoleColor.DarkYellow,"dY" },
            {ConsoleColor.Gray," G" },
            {ConsoleColor.Green," N" },
            {ConsoleColor.Magenta," M" },
            {ConsoleColor.Red," R" },
            {ConsoleColor.White," W" },
            {ConsoleColor.Yellow," Y" },
        };
    }

    public class ConsoleBuffer
    {
        private ConsoleCharacter[,] m_buffer = new ConsoleCharacter[0, 0];

        public int Width => m_buffer.GetLength(0);
        public int Height => m_buffer.GetLength(1);

        public void Resize(int width, int height)
        {
            if (width != Width || height != Height)
            {
                var resized = new ConsoleCharacter[width, height];
                for (int y = 0; y < Height && y < height; y++)
                {
                    for (int x = 0; x < Width && x < width; x++)
                    {
                        resized[x, y] = m_buffer[x, y];
                    }
                    for (int x = Width; x < width; x++)
                    {
                        resized[x, y] = new ConsoleCharacter(' ');
                    }
                }
                for (int y = Height; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        resized[x, y] = new ConsoleCharacter(' ');
                    }
                }
                m_buffer = resized;
            }
        }

        public ConsoleCharacter this[int column, int row]
        {
            get { return m_buffer[column, row]; }
        }

        public void SetCharacter(int column, int row, char character, ConsoleColor? foreground, ConsoleColor? background)
        {
            m_buffer[column, row].Char = character;
            m_buffer[column, row].BackgroundColor = background ?? m_buffer[column, row].BackgroundColor;
            m_buffer[column, row].ForegroundColor = foreground ?? m_buffer[column, row].BackgroundColor;
        }
    }
}
