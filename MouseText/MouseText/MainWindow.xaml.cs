using ApeFree.DataStore.Core;
using ApeFree.DataStore.Local;
using ApeFree.DataStore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Globalization;
using System.Threading;

namespace MouseText
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<TextData> textdata = new ObservableCollection<TextData>();
        private LocalStore<ObservableCollection<TextData>> store;
        int Id = 0;
        myIcon ic;

        public MainWindow()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            store = StoreFactory.Factory.CreateLocalStore<ObservableCollection<TextData>>(new LocalStoreAccessSettings("./TextSet.txt"));
            this.ShowInTaskbar = false;
            ic = new myIcon();
            ic.Icon();
        }

        int c = 1;
        private void Display_Update(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                if (Id == textdata.Count)
                    Id = 0;
                if (c != textdata[Id].Loop && c == 1)
                    c = textdata[Id].Loop;
                else if(c==1)
                    c++;
                LText.FontSize = textdata[Id].FontSize;
                LText.Foreground = textdata[Id].FontColor;
                Font f = new Font();
                var o = f.GetFontWidthHeight(this, textdata[Id].FontSize, textdata[Id].Text, "丁卯点阵体 9px");
                this.Width = o.width + 10;
                this.Height = o.height + 10;
                LText.Content = textdata[Id].Text;
                c--;
                if (c == 1)
                {
                    c = 1;
                    Id++;
                }
            }
        }

        private void Showtimer_Tick(object sender, EventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                var p = System.Windows.Forms.Control.MousePosition;
                this.Left = p.X + 1;
                this.Top = p.Y + 1;
                this.Show();
            }
        }

        public void Window_Closed(object sender, EventArgs e)
        {
            ic.CloseWindow(null, null);
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            store.Load();
            if (store.Value.Count == 0)
                store.Value.Add(new TextData { ID = 0, Text = "フォントテスト", FontSize = 56, FontColor = Brushes.White, Loop = 1, DwellTime = 500 });
            for (int i = 0; i < store.Value.Count; i++)
                textdata.Add(store.Value[i]);
            DispatcherTimer showtimer = new DispatcherTimer();
            showtimer.Tick += Showtimer_Tick;
            showtimer.Interval = new TimeSpan(1);
            showtimer.Start();

            DispatcherTimer update = new DispatcherTimer();
            update.Tick += Display_Update;
            update.Interval = new TimeSpan(5000000);
            update.Start();
        }
    }
    class Font
    {
        public (double width, double height) GetFontWidthHeight(Visual visual, double fontSize, string Text, string fontFamily)
        {
            var pixelsPerDip = VisualTreeHelper.GetDpi(visual).PixelsPerDip;
            FormattedText ft = new FormattedText(
                Text,
                CultureInfo.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(fontFamily),
                fontSize,
                System.Windows.Media.Brushes.Black,
                pixelsPerDip
            );
            return (ft.Width, ft.Height);
        }
    }
}
