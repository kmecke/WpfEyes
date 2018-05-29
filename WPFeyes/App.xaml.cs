using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace WPFeyes
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        App()
        {
        }

        [STAThread]
        static void Main()
        {
            App app = new App();

            MainWindow window = new MainWindow();

            app.Run(window);
        }



    }
}
