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

            cm = this.FindResource("cm") as ContextMenu;

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

            BrushConverter conv = new BrushConverter();
            SolidColorBrush brush = conv.ConvertFromString("#0000FF") as SolidColorBrush;
            // SolidColorBrush brush = new SolidColorBrush(settings.Color);
            mw.Background = brush;

            opacity = settings.Opacity;
            mw.Background.Opacity = opacity;

            showGrip(settings.ShowResizeGrip);
            checkIsChecked("resize", settings.ShowResizeGrip);
            alwaysOnTop(settings.MostTop);
            checkIsChecked("onTop", settings.MostTop);
            showXY(settings.ShowXYPosition);
            checkIsChecked("showXY", settings.ShowXYPosition);
            dragging = settings.DragMove;
            checkIsChecked("move", settings.DragMove);

            applyTimer(settings);
            changeHz(settings.RefreshRate); 
        }

        private void checkIsChecked(string menuItemStr, bool checkState)
        {
            MenuItem item = LogicalTreeHelper.FindLogicalNode(cm, menuItemStr) as MenuItem;
            // MenuItem item = cm.FindResource(menuItemStr) as MenuItem;
            item.IsChecked = checkState;
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

            Point P1v = calcEye(Mx, My, 2f, 2f);
            Point P2v = calcEye(Mx, My, 7f, 2f);

            Dispatcher.Invoke(() => {
                Canvas.SetLeft(P1, P1v.X);
                Canvas.SetTop(P1, P1v.Y);
                Canvas.SetLeft(P2, P2v.X);
                Canvas.SetTop(P2, P2v.Y);
            });
        }

        private Point calcEye(double Mx, double My, double Eposx, double Eposy)
        {
            // Punkt E:     Mittelpunkt Auge
            // Punkt M:     Mauscursor
            // Punkt O:     Ursprung Origin von Bildschirm
            // Punkt P:     Pupille

            double WEx = Eposx * unitX; // Mittelpunkt Auge auf Canvas
            double WEy = Eposy * unitY;

            double OEx = cPosX + WEx; // Origin => Mittelpunkt Auge = Canvas Pos (cPos) + Mittelpunkt Auge (E)
            double OEy = cPosY + WEy;

            double EMx = Mx - OEx; // Auge => MausCursor
            double EMy = My - OEy;

            double lenghtEM = Math.Sqrt(Math.Pow(EMx, 2) + Math.Pow(EMy, 2)); // Betrag von E => M 

            double EPx = EMx / lenghtEM * unitX; // E => P = Normierung und Skalierung 
            double EPy = EMy / lenghtEM * unitY;

            if (Math.Abs(EMx) < unitX)
            {
                if ( Math.Abs(EMy) < Math.Abs(unitY / unitX * Math.Sqrt(Math.Pow(unitX, 2) - EMx )) )
                {
                    EPx = EMx;
                    EPy = EMy;
                }
            }

            double WPx = EPx + WEx;
            double WPy = EPy + WEy;

            double canPosx = WPx - unitX / 2;
            double canPosy = WPy - unitY / 2;

            return new Point(canPosx, canPosy);
        }

        #endregion

        #region Resize and Move Window
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
                cPosX = this.Left + frame;
                cPosY = this.Top + frame;
                settings.eyePos.x = mw.Left;
                settings.eyePos.y = mw.Top;

                calcPos();
            }
        }

        #endregion

        #region Menu
        private void menu(object sender, MouseButtonEventArgs e)
        {
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
            // SolidColorBrush brush = (SolidColorBrush) new BrushConverter().ConvertFromString(col);
            SolidColorBrush brush = null;
            switch (col)
            {
                case "Gray": brush = new SolidColorBrush(Brushes.Gray.Color); break;
                case "LightGray": brush = new SolidColorBrush(Brushes.LightGray.Color); break;
                case "Beige": brush = new SolidColorBrush(Brushes.Beige.Color); break;
                case "Red": brush = new SolidColorBrush(Brushes.Red.Color); break;
                case "DarkBlue": brush = new SolidColorBrush(Brushes.DarkBlue.Color); break;
                case "Black": brush = new SolidColorBrush(Brushes.Black.Color); break;
                default:
                    brush = new SolidColorBrush(Brushes.Gray.Color);
                    break;
            }
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
            if (!isChecked)
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

        private void mw_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // this prevents win7 aerosnap
            if (this.ResizeMode != System.Windows.ResizeMode.NoResize)
            {
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
                this.UpdateLayout();
            }

            // DragMove();
        }

        private void mw_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.ResizeMode == System.Windows.ResizeMode.NoResize)
            {
                // restore resize grips
                this.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                this.UpdateLayout();
            }
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
