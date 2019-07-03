using System;
using SConsole = System.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GameBase.Console
{
    /// <summary>
    /// Create some rudimentary layering & heirarchy to generate the visuals in display
    /// Use some mechanism for selection & active item, etc.
    /// </summary>
    public class ConsoleWindow
    {
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
                var k = SConsole.ReadKey();
                if(k.Key == ConsoleKey.Enter)
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
            int width = -1, height = -1;
            while(true)
            {
                if (SConsole.WindowWidth != width || SConsole.WindowHeight != height)
                {
                    width = SConsole.WindowWidth;
                    height = SConsole.WindowHeight;
                    m_main.InvalidateLayout();
                }
                if (m_main.Layout(width, height))
                {
                    Debug.WriteLine("Render!");
                    m_main.Render(0, 0);
                }
                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
