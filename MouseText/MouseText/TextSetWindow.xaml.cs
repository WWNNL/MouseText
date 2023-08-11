using ApeFree.DataStore;
using ApeFree.DataStore.Core;
using ApeFree.DataStore.Local;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace MouseText
{
    /// <summary>
    /// TextSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TextSetWindow : Window
    {
        ObservableCollection<TextData> textdata = new ObservableCollection<TextData>();
        private LocalStore<ObservableCollection<TextData>> store;
        private int Id = 0;

        public TextSetWindow()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            store = StoreFactory.Factory.CreateLocalStore<ObservableCollection<TextData>>(new LocalStoreAccessSettings("./TextSet.txt"));
            this.ShowInTaskbar = false;
            TextSet.ItemsSource = textdata;
            Renew.IsEnabled = false;
        }

        private void OnPropertyChanged()
        {
            TextSet.ItemsSource = null;
            TextSet.ItemsSource = textdata;
        }
        private void CellDouble(object sender, MouseButtonEventArgs e)
        {
            TextData cell_Info = TextSet.SelectedCells[0].Item as TextData; //选中行数据
            string key = TextSet.SelectedCells[0].Column.Header.ToString(); //选中项表头
            string text = cell_Info.DataGet(key).ToString(); //选中项内容
            TextShow.Text = text;
            Renew.IsEnabled = true;
            if (text == "ID" || text == "FontColor" || text== "Interval")
                Renew.IsEnabled = false;
        }

        private void Botton_Renew(object sender, RoutedEventArgs e)
        {
            object i = TextSet.SelectedCells;
            TextData cell_Info = TextSet.SelectedCells[0].Item as TextData; //选中行数据
            string key = TextSet.SelectedCells[0].Column.Header.ToString(); //选中项表头
            string text = cell_Info.DataGet(key).ToString(); //选中项内容
            if (text != TextShow.Text)
            {
                int num = cell_Info.ID;
                textdata[num].DataSet(key, TextShow.Text);
                OnPropertyChanged();
            }
            Renew.IsEnabled = false;
        }

        private void Button_Confirm(object sender, RoutedEventArgs e)
        {
            this.Hide();
            store.Value.Clear();
            for (int i = 0; i < textdata.Count; i++)
                store.Value.Add(textdata[i]);
            store.Save();
            Process.Start("MouseText.exe");
            System.Windows.Application.Current.Shutdown();
        }

        private void Botton_Cancel(object sender, RoutedEventArgs e)
        {
            this.Hide();
            textdata.Clear();
            for (int i = 0; i < store.Value.Count; i++)
                textdata.Add(store.Value[i]);
            OnPropertyChanged();
            System.Windows.Application.Current.MainWindow.Show();
        }

        private void Botton_Add(object sender, RoutedEventArgs e)
        {
            Id++;
            if (TextSet.SelectedCells.Any())
            {
                TextData cell_Info = TextSet.SelectedCells[0].Item as TextData; //选中行数据
                int num = cell_Info.ID + 1;
                textdata.Insert(num,new TextData { ID = Id, Text = "NULL", FontSize = 56, FontColor = Brushes.White, Loop = 1, DwellTime = 500 });
                for (int i = 0; i < textdata.Count; i++)
                    textdata[i].ID = i;
            }
            else
                textdata.Add(new TextData { ID = Id, Text = "NULL", FontSize = 56, FontColor = Brushes.White, Loop = 1, DwellTime = 500 });
            OnPropertyChanged();
        }

        private void Botton_Delete(object sender, RoutedEventArgs e)
        {
            if (TextSet.SelectedCells.Any())
            {
                TextData cell_Info = TextSet.SelectedCells[0].Item as TextData; //选中行数据
                int num = cell_Info.ID;
                textdata.RemoveAt(num);
                for (int i = 0; i < textdata.Count; i++)
                    textdata[i].ID = i;
                OnPropertyChanged();
            }
            else
                textdata.RemoveAt(textdata.Count-1);
        }

        private void Color_Selection(object sender, RoutedEventArgs e)
        {
            TextData cell_Info = TextSet.SelectedCells[0].Item as TextData; //选中行数据
            int num = cell_Info.ID;
            ColorDialog colorDialog = new ColorDialog();
            //允许使用该对话框的自定义颜色
            colorDialog.AllowFullOpen = true;
            colorDialog.FullOpen = true;
            colorDialog.ShowHelp = true;
            colorDialog.Color = System.Drawing.Color.White;
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textdata[num].FontColor = new SolidColorBrush(Color.FromRgb(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
            OnPropertyChanged();
        }

        private void TextSetWindow_Loaded(object sender, RoutedEventArgs e)
        {
            store.Load();
            if (store.Value.Count == 0)
                store.Value.Add(new TextData { ID = 0, Text = "フォントテスト", FontSize = 56, FontColor = Brushes.White, Loop = 1, DwellTime = 500 });
            for (int i = 0; i < store.Value.Count; i++)
                textdata.Add(store.Value[i]);
            Id = store.Value.Count-1;
        }
    }
    public class TextData
    {
        public int ID { get; set; }

        public string Text { get; set; }

        public int FontSize { get; set; }

        public Brush FontColor { get; set; }

        public int Loop { get; set; }

        public long DwellTime { get; set; }

        public Boolean DataSet(string set, string var)
        {
            var = var.PadLeft(1, '0');
            switch (set)
            {
                case "Text":
                    Text = var;
                    break;
                case "FontSize":
                    FontSize = int.Parse(Regex.Replace(var, @"[^0-9]+", ""));
                    break;
                case "Loop":
                    Loop = int.Parse(Regex.Replace(var, @"[^0-9]+", ""));
                    break;
                case "DwellTime":
                    DwellTime = long.Parse(Regex.Replace(var, @"[^0-9]+", ""));
                    break;
                default:
                    return false;
            }
            return true;
        }

        public object DataGet(string set)
        {
            switch (set)
            {
                case "ID":
                    return ID;
                case "Text":
                    return Text;
                case "FontSize":
                    return FontSize;
                case "FontColor":
                    return FontColor;
                case "Loop":
                    return Loop;
                case "DwellTime":
                    return DwellTime;
                default:
                    return false;
            }
        }
    }
}
