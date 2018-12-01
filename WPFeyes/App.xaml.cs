using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using SIO = System.IO;
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
            try
            {
                App app = new App();

                MainWindow window = new MainWindow();

                app.Run(window);

            }
            catch(Exception ex)
            {
                using (SIO.StreamWriter sw = SIO.File.AppendText(SIO.Path.Combine(SIO.Path.GetTempPath(), "wpfEyesLog.txt")))
                {
                    sw.WriteLine(DateTime.Now.ToString("s") + ex.Message);
                    sw.WriteLine(DateTime.Now.ToString("s") + ex.Source);
                    sw.WriteLine(DateTime.Now.ToString("s") + ex.Data);
                    sw.WriteLine(DateTime.Now.ToString("s") + ex.StackTrace);

                    sw.WriteLine(DateTime.Now.ToString("s") + "Inner: " + ex.InnerException.Message);
                    sw.WriteLine(DateTime.Now.ToString("s") + "Inner: " + ex.InnerException.StackTrace);
                }
                
                ;
            }
        }



    }
}
