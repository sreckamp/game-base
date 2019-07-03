using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBase.Console
{
    public class ConsoleBox:AbstractConsoleComponent
    {
        public ConsoleBox(string name = null) : base(null) { }

        protected override void ComponentLayout(ref int layoutWidth, ref int layoutHeight)
        {
        }

        protected override void ComponentRender(int left, int top)
        {
            if(ActualWidth > 0 && ActualHeight > 0)
            {
                RenderBox(left, top, ActualWidth - Margin.Left - Margin.Right, ActualHeight - Margin.Top - Margin.Bottom);
            }
        }
    }
}
