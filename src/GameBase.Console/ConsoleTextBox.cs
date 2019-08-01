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
    public class ConsoleTextBox : AbstractConsoleElement
    {
        public ConsoleTextBox(string name = null) : base(name) { }

        public event EventHandler<ChangedValueArgs<bool>> IsWrapTextChanged;
        private bool m_isWrapText;
        public bool IsWrapText
        {
            get => m_isWrapText;
            set
            {
                var old = m_isWrapText;
                m_isWrapText = value;
                InvalidateLayout();
                IsWrapTextChanged?.Invoke(this, new ChangedValueArgs<bool>(old, value));
            }
        }

        public event EventHandler<ChangedValueArgs<string>> TextChanged;
        private string m_text;
        public string Text
        {
            get => m_text;
            set
            {
                var old = m_text;
                m_text = value;
                if(m_text != old)
                {
                    InvalidateLayout();
                }
                TextChanged?.Invoke(this, new ChangedValueArgs<string>(old, value));
            }
        }

        private List<string> m_lines = new List<string>();
        //protected override void ComponentLayout(ref int layoutWidth, ref int layoutHeight)
        //{
        //    var maxWidth = 0;
        //    var temp = Text ?? string.Empty;
        //    m_lines.Clear();
        //    while (temp.Length > 0)
        //    {
        //        var line = getNextString(layoutWidth, ref temp);
        //        var len = line?.Length ?? 0;
        //        if (line != null && (layoutHeight <= 0 || m_lines.Count < layoutHeight))
        //        {
        //            maxWidth = Math.Max(maxWidth, len);
        //            m_lines.Add(line);
        //        }
        //    }
        //    layoutHeight = m_lines.Count;
        //    layoutWidth = maxWidth;
        //}

        private string getNextString(int actualWidth, ref string text)
        {
            string line = text;
            if (actualWidth > 0)
            {
                line = text.Substring(0, Math.Min(text.Length, actualWidth));
                text = IsWrapText ? text.Substring(actualWidth) : string.Empty;
            }
            else
            {
                var idx = text.IndexOf('\n');
                if(idx >=0)
                {
                    line = text.Substring(0, idx);
                    text = text.Substring(idx + 1);
                    line = line.Replace("\r", "");
                    while (text.StartsWith("\r")) text = text.Substring(1);
                }
                else
                {
                    text = string.Empty;
                }
            }
            return line;
        }

        //protected override void ComponentRender(int left, int top)
        //{
        //    for (int i = 0; i < m_lines.Count; i++)
        //    {
        //        SConsole.SetCursorPosition(left, top + i);
        //        SConsole.Write(m_lines[i]);
        //    }
        //}

        private int m_maxWidth = 0;
        public override Size MeasureOverride(Size availableSize)
        {
            m_maxWidth = 0;
            var temp = Text ?? string.Empty;
            m_lines.Clear();
            while (temp.Length > 0)
            {
                var line = getNextString(availableSize.Width, ref temp);
                var len = line?.Length ?? 0;
                if (line != null && m_lines.Count < availableSize.Height)
                {
                    m_maxWidth = Math.Max(m_maxWidth, len);
                    m_lines.Add(line);
                }
            }
            return new Size(m_maxWidth, m_lines.Count);
        }

        public override Size ArrangeOverride(Size availableSize)
        {
            return new Size(Math.Min(availableSize.Width, DesiredSize.Width - Margin.TotalSize.Width), Math.Min(availableSize.Height, DesiredSize.Height - Margin.TotalSize.Height));
        }

        public override void RenderOverride(IConsoleContext context)
        {
            for (int i = 0; i < m_lines.Count; i++)
            {
                context.Write(m_lines[i], RenderLocation.X, RenderLocation.Y + i);
            }
        }
    }
}
