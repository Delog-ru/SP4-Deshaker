using Microsoft.Win32;
using SP4_Deshaker.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SP4_Deshaker
{
    class SP4Deshaker : ApplicationContext
    {
        readonly NotifyIcon notifyIcon;
        readonly Timer timer = new Timer();
        readonly Icon[] icons;
        readonly int iconsLength;
        readonly MenuItem startupMenuItem;
        const string startupKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        int currentIcon;
        public SP4Deshaker()
        {
            icons = new Icon[] { Resources.Icon1, Resources.Icon2, Resources.Icon3, Resources.Icon4, Resources.Icon5 };
            iconsLength = icons.Length;
            startupMenuItem = new MenuItem("Startup", Startup)
            {
                Checked = IsStartupEnabled()
            };
            notifyIcon = new NotifyIcon()
            {
                Icon = icons[currentIcon],
                ContextMenu = new ContextMenu(new MenuItem[] { startupMenuItem, new MenuItem("Exit", Exit) { } }),
                Visible = true,
                Text = "SP4 Deshaker\r\nMore info at github.com/Delog-ru"
            };
            timer.Tick += Timer_Tick;
            timer.Interval = 500;
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (++currentIcon >= iconsLength) currentIcon = 0;
            notifyIcon.Icon = icons[currentIcon];
        }

        bool IsStartupEnabled()
        {
            return Registry.GetValue("HKey_Current_User\\" + startupKey, nameof(SP4Deshaker), null) != null;
        }

        void Startup(object sender, EventArgs e)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(startupKey, true);
            startupMenuItem.Checked = !IsStartupEnabled();
            if (startupMenuItem.Checked)
                registryKey.SetValue(nameof(SP4Deshaker), Application.ExecutablePath);
            else
                registryKey.DeleteValue(nameof(SP4Deshaker), false);
        }

        void Exit(object sender, EventArgs e)
        {
            timer.Stop();
            notifyIcon.Visible = false;
            Application.Exit();
        }
    }
}
