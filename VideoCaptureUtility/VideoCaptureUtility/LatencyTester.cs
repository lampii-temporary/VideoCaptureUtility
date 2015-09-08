using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace OculusFPV.Launcher
{
    class LatencyTesterWindow
    {
        System.Windows.Media.Brush blackBrush = System.Windows.Media.Brushes.Black;
        System.Windows.Media.Brush redBrush = System.Windows.Media.Brushes.Red;
        System.Windows.Media.Brush whiteBrush = System.Windows.Media.Brushes.White;

        Window testWindow;
        public LatencyTesterWindow()
        {
            testWindow = new Window();
            testWindow.Width = 640;
            testWindow.Height = 480;
            testWindow.Background = whiteBrush;
            testWindow.Show();
        }
        public enum BColor
        {
            Red, Black, White
        }
        public void ChangeBrush(BColor color)
        {
            switch(color)
            {
                case BColor.Red:
                    testWindow.Background = redBrush;
                    break;
                case BColor.Black:
                       testWindow.Background = blackBrush;
                    break;
                case BColor.White:
                    testWindow.Background = whiteBrush;
                    break;
                default:
                       testWindow.Background = blackBrush;
                    break;
            }
            
        }
    }
}
