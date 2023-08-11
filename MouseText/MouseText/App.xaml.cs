using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace MouseText
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
    }
    public class myIcon
    {
        //任务栏图标
        System.Windows.Forms.NotifyIcon notifyIcon = null;
        public void Icon()
        {
            //创建图标
            this.notifyIcon = new System.Windows.Forms.NotifyIcon();

            //程序打开时任务栏会有小弹窗
            //this.notifyIcon.BalloonTipText = "MouseText 已启动...";

            //鼠标放在图标上时显示的文字
            this.notifyIcon.Text = "MouseText";

            //图标图片的位置，注意这里要用绝对路径
            this.notifyIcon.Icon = new System.Drawing.Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("MouseText.MouseTextW.ico"));

            //显示图标
            this.notifyIcon.Visible = true;

            //右键菜单--退出菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            System.Windows.Forms.MenuItem interval = new System.Windows.Forms.MenuItem("-");
            System.Windows.Forms.MenuItem show = new System.Windows.Forms.MenuItem("显示/消失");
            System.Windows.Forms.MenuItem textset = new System.Windows.Forms.MenuItem("文本设置...");
            exit.Click += new EventHandler(CloseWindow);
            show.Click += new EventHandler(Display);
            textset.Click += new EventHandler(TextSetWindow_Show);

            //关联托盘控件
            System.Windows.Forms.MenuItem[] children = new System.Windows.Forms.MenuItem[] { textset, show, interval, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(children);

            //this.notifyIcon.ShowBalloonTip(1000);
        }

        //退出菜单项对应的处理方式
        public void CloseWindow(object sender, EventArgs e)
        {
            //Dispose()函数能够解决程序退出后图标还在，要鼠标划一下才消失的问题
            this.notifyIcon.Dispose();
            //关闭整个程序
            Application.Current.Shutdown();
        }

        TextSetWindow textSetWindow = new TextSetWindow();

        public void Display(object sender, EventArgs e)
        {
            if(Application.Current.MainWindow.Visibility == Visibility.Visible || textSetWindow.Visibility == Visibility.Visible)
                Application.Current.MainWindow.Hide();
            else
                Application.Current.MainWindow.Show();
        }

        public void TextSetWindow_Show(object sender, EventArgs e)
        {
            textSetWindow.Show();
            Application.Current.MainWindow.Hide();
        }
    }
}
