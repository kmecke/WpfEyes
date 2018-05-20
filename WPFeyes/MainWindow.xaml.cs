using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

namespace WPFeyes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        ContextMenu cm;
        Point newP, oldP;
        System.Timers.Timer myTimer;
        string path = "exe path n/a";

        const float frame = 10;
        private bool dragging = true;
        private float opacity = 0.3f;
        String col;
        Settings settings;

        static double cWidth, cHeight;
        static double cPosX, cPosY;
        static double unitX, unitY;

        InterceptMouse mouseCom;

        public MainWindow()
        {
            mouseCom = new InterceptMouse();

            path = System.Reflection.Assembly.GetEntryAssembly().Location;
            path = System.IO.Path.Combine(System.IO.Directory.GetParent(path).FullName, "EyesSettings.xml");

            if (System.IO.File.Exists(path))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    settings = serializer.Deserialize(stream) as Settings;
                }
            }
            else
            {
                settings = new Settings("default");
            }

            InitializeComponent();

            applysettings(settings);

            newP = mouseCom.getPooint();
            oldP = mouseCom.getPooint();

        }

        private void applysettings(Settings settings)
        {

            this.Width = settings.eyeSize.Width;
            this.Height = settings.eyeSize.Height;
            this.Left = settings.eyePos.x;
            this.Top = settings.eyePos.y;

            canvas1.Margin = new Thickness(frame);

            col = "LightGray";

            opacity = settings.Opacity / 100;
            mw.Background.Opacity = opacity;

            SolidColorBrush brush = (SolidColorBrush)new BrushConverter().ConvertFromString(settings.Color);
            brush.Opacity = settings.Opacity/100;
            mw.Background = brush;

            showGrip(settings.ShowResizeGrip);

            if (settings.MostTop) mw.Topmost = true;
            else mw.Topmost = false;

            showXY(settings.ShowXYPosition);

            applyTimer(settings);
            changeHz(settings.RefreshRate); 
            dragging = settings.DragMove;
        }

        private void applyTimer(Settings settings)
        {

            if (myTimer == null)
            {
                myTimer = new System.Timers.Timer();
                myTimer.Elapsed += new ElapsedEventHandler(this.moveMouse);
                myTimer.Interval = 1000 / settings.RefreshRate;
                myTimer.Enabled = true;
                myTimer.Start();
            }
            else
            {
                myTimer.Stop();
                myTimer.Interval = 1000 / settings.RefreshRate;
                myTimer.Start();
            }
        }

        #region Eventhanlders EyePos
        private void moveMouse(object sender, ElapsedEventArgs ea)
        {
            newP = mouseCom.getPooint();
            if ( ! (newP.X == oldP.X && newP.Y == oldP.Y))
            {
                // MouseEventArgs ea = new MouseEventArgs();
                // newPos_Handler.Invoke(newP, ea);
                Dispatcher.Invoke(() =>
                {
                    posLabel.Content = newP.X + " " + newP.Y;
                });
                calcPos();
            }
        }

        private void calcPos()
        {
            double Mx = newP.X;
            double My = newP.Y;

            // Eye 1
            double WE1x = 2 * unitX; // Mittelpunkt Auge
            double WE1y = 2 * unitY;
            double OE1x = cPosX + WE1x;
            double OE1y = cPosY  + WE1y;

            double E1Mx = Mx - OE1x;
            double E1My = My - OE1y;

            double lenghtE1M = Math.Sqrt(Math.Pow(E1Mx, 2) + Math.Pow(E1My, 2));

            double WP1x = (E1Mx / lenghtE1M * unitX) + WE1x;
            if (Math.Abs(E1Mx) < unitX)
            {
                WP1x = E1Mx + WE1x; // Mittelpunkt Pupille
            }

            double WP1y = (E1My / lenghtE1M * unitY) + WE1y;
            if (Math.Abs(E1My) < unitY)
            {
                WP1y = E1My + WE1y; // Mittelpunkt Pupille
            }

            double canPos1x = WP1x - unitX / 2;
            double canPos1y = WP1y - unitY / 2;

            // Eye 2
            double WE2x = 7 * unitX;
            double WE2y = 2 * unitY;
            double OE2x = cPosX  + WE2x;
            double OE2y = cPosY + WE2y;

            double E2Mx = Mx - OE2x;
            double E2My = My - OE2y;

            double lenghtE2M = Math.Sqrt(Math.Pow(E2Mx, 2) + Math.Pow(E2My, 2));

            double WP2x = (E2Mx / lenghtE2M * unitX) + WE2x;
            if (Math.Abs( E2Mx ) < unitX)
            {
                WP2x = E2Mx + WE2x; // Mittelpunkt Pupille
            }

            double WP2y = (E2My / lenghtE2M * unitY) + WE2y;
            if (Math.Abs( E2My ) < unitY)
            {
                WP2y = E2My + WE2y; // Mittelpunkt Pupille
            }

            double canPos2x = WP2x - unitX / 2;
            double canPos2y = WP2y - unitY / 2;

            Dispatcher.Invoke(() => {
                Canvas.SetLeft(P1, canPos1x);
                Canvas.SetTop(P1, canPos1y);
                Canvas.SetLeft(P2, canPos2x);
                Canvas.SetTop(P2, canPos2y);
            });
        }
        #endregion

        #region Resize and Move
        private void canvas1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cWidth = canvas1.Width;
            cHeight = canvas1.Height;
            cPosX = this.Left + frame;
            cPosY = this.Top + frame;

            unitX = (cWidth) / 9;
            unitY = (cHeight) / 4;

            E1.Width = unitX * 4;
            E1.Height = unitY * 4;
            Canvas.SetLeft(E1, unitX * 0);
            Canvas.SetTop(E1, unitY * 0);

            E2.Width = unitX * 4;
            E2.Height = unitY * 4;
            Canvas.SetLeft(E2, unitX * 5);
            Canvas.SetTop(E2, unitY * 0);

            P1.Width = unitX;
            P1.Height = unitY;
            P2.Width = unitX;
            P2.Height = unitY;

            calcPos();
        }

        private void DockPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas1.Height = e.NewSize.Height - 2 * frame;
            canvas1.Width = e.NewSize.Width - 2 * frame;
            settings.eyeSize.Width = mw.Width;
            settings.eyeSize.Height = mw.Height;
        }

        private void moveWin(object sender, MouseButtonEventArgs e)
        {
            if (dragging && e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
                settings.eyePos.x = mw.Left;
                settings.eyePos.y = mw.Top;
            }
        }

        #endregion

        #region Menu
        private void menu(object sender, MouseButtonEventArgs e)
        {
            cm = this.FindResource("cm") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }

        public void save(object sender, RoutedEventArgs e)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Settings));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(stream, settings);
                }

                MessageBox.Show("Sucessfully saved your current settings here" + Environment.NewLine +
                    path,
                    "Saving Settings",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong during saving process" + Environment.NewLine +
                    "Writing a xml file to the following location failed: " + Environment.NewLine +
                    path + Environment.NewLine +
                    ex.Message,
                    "Saving Settings",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void changeHz_cb(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            float hz = 1000 / Convert.ToSingle(mi.Tag);
            changeHz(hz);
            settings.RefreshRate = hz;
        }

        private void changeHz(float hz)
        {
            myTimer.Stop();
            myTimer.Interval = hz;
            settings.RefreshRate = hz;
            myTimer.Start();
        }

        private void dragMove(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            dragging = mi.IsChecked;
            settings.DragMove = mi.IsChecked;
        }

        private void about(object sender, RoutedEventArgs e)
        {
            About aboutWindow = new About();
            aboutWindow.ShowDialog();
        }

        private void browse(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            String url = mi.Tag.ToString();
            if (! String.IsNullOrEmpty(url))
            {
                Process proc = new Process();
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.FileName = url;
                proc.Start();
            }
        }

        private void setColor(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            col = mi.Tag.ToString();
            SolidColorBrush brush = (SolidColorBrush) new BrushConverter().ConvertFromString(col);
            brush.Opacity = opacity;
            mw.Background = brush;
            settings.Color = brush.Color.ToString();
        }

        private void alwaysOnTop_cb(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            alwaysOnTop(mi.IsChecked);
            settings.MostTop = mi.IsChecked;
        }

        private void alwaysOnTop(bool isChecked)
        {
            if (isChecked)
            {
                mw.Topmost = false;
            }
            else
            {
                mw.Topmost = true;
            }
        }

        private void changeOpacity(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            if (mi != null)
            {
                opacity = Convert.ToSingle(mi.Tag) / 100;
                mw.Background.Opacity = opacity;
                settings.Opacity = opacity;
            }
        }

        private void showGrip_cb(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            showGrip(mi.IsChecked);
            settings.ShowResizeGrip = mi.IsChecked;
        }

        private void showGrip(bool isChecked)
        {
            if (!(isChecked))
            {
                mw.ResizeMode = ResizeMode.CanResize;
            }
            else
            {
                mw.ResizeMode = ResizeMode.CanResizeWithGrip;
            }
        }

        private void showXY_cb(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            showXY(mi.IsChecked);
            settings.ShowXYPosition = mi.IsChecked;
        }

        private void showXY(bool isChecked)
        {
            if (isChecked)
            {
                this.posLabel.Visibility = Visibility.Visible;
            }
            else
            {
                this.posLabel.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        private void exit(object sender, RoutedEventArgs e)
        {
            mouseCom.exitCall();
            System.Windows.Application.Current.Shutdown();
        }

    }

}
