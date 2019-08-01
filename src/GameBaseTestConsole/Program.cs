using GameBase.Console;
using System;

namespace GameBase
{
    class Program
    {
#pragma warning disable IDE1006 // Naming Styles
        static void Main(string[] args)
#pragma warning restore IDE1006 // Naming Styles
        {
            var cv = new ConsoleContainer("Test")
            {
                //BackgroundColor = ConsoleColor.Cyan,
//                Padding = new ConsoleSpacing(8, 2, 4, 1)
            };
            cv.Children.Add(new ConsoleBox("WhiteBox")
            {
                BorderStyle = ConsoleBorderStyle.Double,
            });
            var cfc = new ConsoleFlowContainer("FullFlow")
            {
                IsHorizontalLayout = false,
                Margin = 1
            };
            var ctr = new ConsoleContainer("Top");
            cfc.Children.Add(ctr);
            ctr.Children.Add(new ConsoleBox("RedBox")
            {
                ForegroundColor = ConsoleColor.Red,
            });

            var cfch = new ConsoleFlowContainer("TopFlow")
            {
                IsHorizontalLayout = true,
                Margin = new ConsoleSpacing(1)
            };
            ctr.Children.Add(cfch);
            cfch.Children.Add(new ConsoleTextBox("RedH")
            {
                ForegroundColor = ConsoleColor.Red,
                //ForegroundColor = null,
                BackgroundColor = null,
                Margin = 1,
                Text = "Red",
            });
            cfch.Children.Add(new ConsoleTextBox("WhiteH")
            {
                ForegroundColor = ConsoleColor.White,
                //ForegroundColor = null,
                BackgroundColor = null,
                Margin = 1,
                Text = "White",
            });
            cfch.Children.Add(new ConsoleTextBox("BlueH")
            {
                ForegroundColor = ConsoleColor.Blue,
                //ForegroundColor = null,
                BackgroundColor = null,
                Margin = 1,
                Text = "Blue",
            });
            ctr = new ConsoleContainer("Bottom");
            cfc.Children.Add(ctr);
            ctr.Children.Add(new ConsoleBox("BlueBox")
            {
                BorderStyle = ConsoleBorderStyle.Single,
                ForegroundColor = ConsoleColor.Blue,
            });
            var cfcv = new ConsoleFlowContainer("BottomFlow")
            {
                IsHorizontalLayout = false,
                Margin = 1,
            };
            ctr.Children.Add(cfcv);
            cfcv.Children.Add(new ConsoleTextBox("RedV")
            {
                ForegroundColor = ConsoleColor.Red,
                //ForegroundColor = null,
                BackgroundColor = null,
                Margin = 1,
                Text = "Red",
            });
            cfcv.Children.Add(new ConsoleTextBox("WhiteV")
            {
                ForegroundColor = ConsoleColor.White,
                //ForegroundColor = null,
                BackgroundColor = null,
                Margin = 1,
                Text = "White",
            });
            cfcv.Children.Add(new ConsoleTextBox("BlueV")
            {
                ForegroundColor = ConsoleColor.Blue,
                //ForegroundColor = null,
                BackgroundColor = null,
                Margin = 1,
                Text = "Blue",
            });
            cv.Children.Add(cfc);

            //cv.Children.Add(new ConsoleBox
            //{
            //    BackgroundColor = ConsoleColor.DarkCyan
            //    //,Margin = new ConsoleSpacing(4, 1, 8, 2)
            //});
            //cv.Children.Add(new ConsoleBox
            //{
            //    BackgroundColor = ConsoleColor.DarkBlue,
            //    ForegroundColor = ConsoleColor.Blue,
            //    Margin = new ConsoleSpacing(2)
            //});
            //cv.Children.Add(new ConsoleBox
            //{
            //    BackgroundColor = ConsoleColor.Black,
            //    ForegroundColor = null,
            //    Margin = new ConsoleSpacing(3)
            //});
            //cv.Children.Add(new ConsoleTextBox
            //{
            //    ForegroundColor = ConsoleColor.Green,
            //    //ForegroundColor = null,
            //    BackgroundColor = null,
            //    Text = "Hello World!",
            //    Margin = new ConsoleSpacing(0, 4)
            //});
            var cw = new ConsoleWindow(cv);
            cw.Run();
            while (true) System.Threading.Thread.Sleep(10000);
        }
    }
}
