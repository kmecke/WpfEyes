using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WPFeyes
{
    class EyeNotifyIcon
    {
        ToolStripMenuItem selItem;
        NotifyIcon nIcon = new NotifyIcon();
        MainWindow mw;

        public EyeNotifyIcon(MainWindow mw)
        {
            this.mw = mw;
            nIcon.Icon = new Icon(@"../../WpfEyesIcon\Rastergrafik.ico");
            // nIcon.ShowBalloonTip(5000, "Hi", "This is a BallonTip from Windows Notification", ToolTipIcon.Info);
            nIcon.ContextMenuStrip = ContextMenusCreate();
            nIcon.Visible = true;
        }

        private ContextMenuStrip ContextMenusCreate()
        {
            // Add the default menu options.
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem item;
            ToolStripSeparator sep;

            selItem = new ToolStripMenuItem();
            selItem.Text = "Restore Start Settings";
            selItem.Click += new EventHandler(Restore_Click);
            // selItem.Image = Resources.Explorer;
            menu.Items.Add(selItem);

            selItem = new ToolStripMenuItem();
            selItem.Text = "Hide";
            selItem.Click += new EventHandler(Hide_Click);
            // selItem.Image = Resources.Explorer;
            menu.Items.Add(selItem);

            // About.
            item = new ToolStripMenuItem();
            item.Text = "About";
            item.Click += new EventHandler(About_Click);
            // item.Image = Resources.About;
            menu.Items.Add(item);

            // Separator.
            sep = new ToolStripSeparator();
            menu.Items.Add(sep);

            // Exit.
            item = new ToolStripMenuItem();
            item.Text = "Exit";
            item.Click += new System.EventHandler(Exit_Click);
            // item.Image = Resources.Exit;
            menu.Items.Add(item);

            return menu;
        }

        private void Restore_Click(object sender, EventArgs e)
        {
            mw.restoreSettings();
        }

        private void Hide_Click(object sender, EventArgs e)
        {
            selItem.Text = "Show";
            selItem.Click += new EventHandler(Show_Click);
            mw.Hide();
        }

        private void Show_Click(object sender, EventArgs e)
        {
            selItem.Text = "Hide";
            selItem.Click += new EventHandler(Hide_Click);
            mw.Show();
        }

        private void About_Click(object sender, EventArgs e)
        {
            About aboutWindow = new About();
            aboutWindow.ShowDialog();
        }

        void Exit_Click(object sender, EventArgs e)
        {
            mw.exit();
        }

    }
}
