/*using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Threading;

namespace mui
{
    /// <summary>
    /// Page2.xaml 的交互逻辑
    /// </summary>
    public partial class Page2 : Page
    {
        public Page2()
        {
            InitializeComponent();
        }
        home spihome = new home();

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            ThreadStart ts = new ThreadStart(spihome.Downloads);
            Thread thread = new Thread(ts);
            thread.IsBackground = true;
            thread.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            spihome.Stop();
            lv.Items.Clear();
        }

        private void Store_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog Dia = new System.Windows.Forms.FolderBrowserDialog();
            Dia.RootFolder = Environment.SpecialFolder.Desktop;
            Dia.Description = "Content Root Folder";
            var result = Dia.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Textbox_Path.Text = Dia.SelectedPath;
            }
        }

        private void Delete_click(object sender, RoutedEventArgs e)
        {
            spihome.Delete(Textbox_Num.Text);
            relist();
        }

        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            spihome.Scan(Textbox_Url.Text, Textbox_Depth.Text);
            relist();
        }

        private void relist()
        {
            int i;
            lv.Items.Clear();
            List<string> urls = spihome.Geturls();
            for (i = 0; i < urls.Count;i++ )
            {
                lv.Items.Add(new {Order=i,Url=urls[i]});
            }
        }
    }
}
*/