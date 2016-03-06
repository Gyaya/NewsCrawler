using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
 
namespace mui
{
    /// <summary>
    /// spider.xaml 的交互逻辑
    /// </summary>
    public partial class spider : Page
    {
        public spider()
        {
            InitializeComponent();          
        }

        //申明对象
        home spihome = new home();

        //申明一个专用来调用更改线程函数的委托
        private delegate void ThreadDelegate(); 

        //扫描的线程
        private void Scan(object sender, RoutedEventArgs e)
        { 
            listview.Items.Clear();
            Message.Items.Clear();
            spihome.Get_Value(year.Text, month.Text, day1.Text, day2.Text);
            ThreadStart ts = new ThreadStart(Thread_Scan);
            Thread thread = new Thread(ts);
            thread.IsBackground = true;
            thread.Start();
        }

        //Thread_Scan
        public void Thread_Scan()
        {
            int i,j;
            spihome.con = true;
            bool succ = true;
            spihome.scan_ok = false;
            spihome.list.Clear();
            spihome.list_m.Clear();
            List<string> temp = new List<string>();
            
            for (i = spihome.day1; i <= spihome.day2; i++)
            {
                if (!spihome.con)
                    break;
                //http://news.sina.com.cn/s/2013-09-04/052928125713.shtml
                string source_url = spihome.url + spihome.Get_Date(i, true) + ".shtml";
                string target_url = "http://news.sina.com.cn/s/" + spihome.Get_Date(i, false) + @"/\d{8,18}.shtml";
                temp = spihome.Match(source_url, target_url, ref succ);
                foreach (string str in temp)
                    if (!spihome.list.Contains(str))
                        spihome.list.Add(str);
                if (!succ)
                    spihome.list_m.Add(spihome.Get_Date(i, false) + " failed!");
                else
                    spihome.list_m.Add(spihome.Get_Date(i, false) + " complete!");
                Thread st_m = new Thread(new ThreadStart(delegate
                {
                    Message.Dispatcher.Invoke(new Action(delegate
                    {
                        Message.Items.Clear();
                        for ( j= 0; j < spihome.list_m.Count; j++)
                        {
                            Message.Items.Add(new { state = spihome.list_m[j] });
                        }
                    }), null);
                }));
                st_m.Start();
            }
            spihome.list_m.Add("Scan complete!");
            spihome.scan_ok = true;
            Thread t = new Thread(new ThreadStart(delegate
            {
                listview.Dispatcher.BeginInvoke(new Action(delegate
                {
                    listview.Items.Clear();
                    for (i = 0; i < spihome.list.Count; i++)
                    {
                        listview.Items.Add(new { Order=i, Url = spihome.list[i] });
                    }
                }), null);
            }));
            t.Start();
        }

        private void Download(object sender, RoutedEventArgs e)
        {
            //Message.Items.Clear();
            ThreadStart td = new ThreadStart(Thread_Download);
            Thread thread = new Thread(td);
            thread.IsBackground = true;
            thread.Start();
        }


        //下载网站
        public void Thread_Download()
        {
            bool succ = true;
            spihome.con = true;
            spihome.downlaod_ok = false;
            int i;
            foreach (string url in spihome.list)
            {
                if (!spihome.con)
                    break;
                Thread st = new Thread(new ThreadStart(delegate
                {
                    Message.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        Message.Items.Clear();
                        for (i = 0; i < spihome.list_m.Count; i++)
                        {
                            Message.Items.Add(new { state = spihome.list_m[i] });
                        }
                    }), null);
                }));
                st.Start();

                string url_info=url.Split('/')[4]+"     "+url.Split('/')[5];
                //time
                List<string> Time_List = spihome.Match(url, @"(?<=<span id=\u0022pub_date\u0022>).+?(?=</span>)", ref succ);
                if (!succ)
                {
                    spihome.list_m.Add("  reason: time not found!  " + url_info);
                    continue;
                }
                string Time_Str = Time_List[0];

                //title
                List<string> Title_List = spihome.Match(url, @"(?<=title\u0022 content=\u0022).+?(?=\u0022 />)", ref succ);
                if (!succ)
                {
                    spihome.list_m.Add("  reason: title not found!  " + url_info);
                    continue;
                }
                string Title_Str = Title_List[0];

                //publish
                List<string> Publish_List = spihome.Match(url, @"(?<=<p>).+?(?=</p>)", ref succ);
                if (!succ)
                {
                    spihome.list_m.Add("  reason: publish not found!  " + url_info);
                    continue;
                }
                string Publish_Str = "";
                foreach (string pub in Publish_List)
                {
                    if (!pub.Contains("<"))
                        Publish_Str += pub;
                }
                Publish_Str.Replace("&nbsp", " ");

                //comment example:comment5.news.sina.com.cn/page/info?format=js&channel=gn&newsid=1-1-27801336&group=&compress=1&ie=gbk&oe=gbk&page=1&page_size=20
                List<string> Key_Word_List = spihome.Match(url, @"(?<=comment\u0022 content=\u0022).+?(?=\u0022>)", ref succ);
                if (!succ)
                {
                    spihome.list_m.Add("  reason: key_word not found!  " + url_info);
                    continue;
                }
                string Key_word = Key_Word_List[0];
                List<string> Comment_List = new List<string>();
                string new_url = "http://comment5.news.sina.com.cn/page/info?format=js&channel=" + Key_word.Split(':')[0] + "&newsid=" + Key_word.Split(':')[1] + "&group=&compress=1&ie=gbk&oe=gbk&page=1&page_size=100";
                Comment_List = spihome.Match(new_url, @"(?<=content\u0022: \u0022).+?(?=\u0022, \u0022)", ref succ);
                if (!succ)
                {
                    spihome.list_m.Add("  reason: comment not found!  " + url_info);
                    continue;
                }

                //mood example comment5.news.sina.com.cn/count/info?callback=moodObj.callback&key=1-1-27804741
                string new_mood_url = "http://comment5.news.sina.com.cn/count/info?callback=moodObj.callback&key=" + Key_word.Split(':')[1];

                //总数@"(?<=sum\u0022: \u0022).+?(?=\u0022)";                
                List<string> summary = spihome.Match(new_mood_url, @"(?<=sum\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    summary.Add("0");

                //感动@"(?<=1\u0022: \u0022).+?(?=\u0022)";
                List<string> move = spihome.Match(new_mood_url, @"(?<=1\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    move.Add("0");

                //震惊@"(?<=9\u0022: \u0022).+?(?=\u0022)";
                List<string> shock = spihome.Match(new_mood_url, @"(?<=9\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    shock.Add("0");

                //搞笑@"(?<=5\u0022: \u0022).+?(?=\u0022)"; 
                List<string> funny = spihome.Match(new_mood_url, @"(?<=5\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    funny.Add("0");

                //难过@"(?<=6\u0022: \u0022).+?(?=\u0022)";
                List<string> sad = spihome.Match(new_mood_url, @"(?<=6\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    sad.Add("0");

                //新奇@"(?<=7\u0022: \u0022).+?(?=\u0022)";
                List<string> novel = spihome.Match(new_mood_url, @"(?<=7\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    novel.Add("0");

                //愤怒@"(?<=4\u0022: \u0022).+?(?=\u0022)";
                List<string> angry = spihome.Match(new_mood_url, @"(?<=4\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    angry.Add("0");

                //存储
                if (int.Parse(summary[0]) < 20 || Comment_List.Count < 20)
                {
                    spihome.list_m.Add("Error: not enough news!  " + url_info);
                    continue;
                }
                string s = spihome.path + url.Split('/')[4] + "\\";
                DirectoryInfo di = Directory.CreateDirectory(s);
                StreamWriter writer = new StreamWriter(s + Key_word.Split(':')[1] + ".txt");
                writer.WriteLine("");
                writer.WriteLine("url:" + url);
                writer.WriteLine("");
                writer.WriteLine("time:" + Time_Str);
                writer.WriteLine("");
                writer.WriteLine("title:" + Title_Str);
                writer.WriteLine("");
                writer.WriteLine("publish:" + Publish_Str);
                for (i = 1; i <= Comment_List.Count & i <= 20; i++)
                {
                    writer.WriteLine("");
                    writer.WriteLine("comment" + i + ":" + Comment_List[i - 1]);
                }
                writer.WriteLine("");
                writer.WriteLine("summary:" + summary[0]);
                writer.WriteLine("");
                writer.WriteLine("move:" + move[0]);
                writer.WriteLine("");
                writer.WriteLine("shock:" + shock[0]);
                writer.WriteLine("");
                writer.WriteLine("funny:" + funny[0]);
                writer.WriteLine("");
                writer.WriteLine("sad:" + sad[0]);
                writer.WriteLine("");
                writer.WriteLine("novel:" + novel[0]);
                writer.WriteLine("");
                writer.WriteLine("angry:" + angry[0]);
                writer.Close();
                spihome.list_m.Add("Download complete!  " + url_info);
                
            }
            spihome.list_m.Add("Download task finished!");
            Thread st2 = new Thread(new ThreadStart(delegate
            {
                Message.Dispatcher.BeginInvoke(new Action(delegate
                {
                    Message.Items.Clear();
                    for (i = 0; i < spihome.list_m.Count; i++)
                    {
                        Message.Items.Add(new { state = spihome.list_m[i] });
                    }
                }), null);
            }));
            st2.Start();
            spihome.downlaod_ok = true;
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            ThreadStart tsp = new ThreadStart(Thread_Stop);
            Thread thread = new Thread(tsp);
            thread.IsBackground = true;
            thread.Start();
        }

        //停止
        public void Thread_Stop()
        {
            spihome.con = false;
            spihome.list.Clear();
            spihome.list_m.Clear();
        }
    }
}
