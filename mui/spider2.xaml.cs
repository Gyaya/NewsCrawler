using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using System.Web;
namespace mui
{
    /// <summary>
    /// spider2.xaml 的交互逻辑
    /// </summary>
    public partial class spider2 : Page
    {
        #region para
        //all urls
        private static List<string> urls = new List<string>();
        //workers and thread
        BackgroundWorker WorkerBar;
        BackgroundWorker WorkerUrl;
        Thread MessageViewThread;
        Thread WorkBoxThread;
        //workpercent
        static int workpercent = 0;
        //datetime and para
        DateTime datebegin;
        DateTime dateend;
        int MinComments = 0;
        int MinEmotions = 20;
        int MinCommentLength = 0;
        int maxcomment = 99999999;
        //doflag
        bool doflag = true;
        //all message
        static List<string> MessageList = new List<string>();
        static int length = 0;
        //match
        MatchCollection match = null;
        string content = "";
        string VisitedDate = "";
        #endregion
        public spider2()
        {
            InitializeComponent();
            DateRange.Text = "2014 01 01-2015 11 11-0-0-0";
            FileAddress.Text = Enumerations.directory;
        }
        private void ChangeList(string url)
        {
            MessageList.Add(url);
        }
        #region WorkerBar
        void WorkerBar_DoWork(object sender, DoWorkEventArgs e)
        {
            while (doflag)
            {
                WorkerBar.ReportProgress(workpercent);
                Thread.Sleep(300);
            }
        }
        void WorkerBar_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        void WorkerBar_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visibility = Visibility.Hidden;
            MessageBox.Show("Completed or stopped");
        }
        #endregion
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                FileAddress.Text = folderBrowserDialog1.SelectedPath + "\\";
            Enumerations.directory = FileAddress.Text;
        }
        private void Go_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] tempdate = DateRange.Text.Split('-');
                datebegin = new DateTime(int.Parse(tempdate[0].Split(' ')[0]), int.Parse(tempdate[0].Split(' ')[1]), int.Parse(tempdate[0].Split(' ')[2]));
                dateend = new DateTime(int.Parse(tempdate[1].Split(' ')[0]), int.Parse(tempdate[1].Split(' ')[1]), int.Parse(tempdate[1].Split(' ')[2]));
                MinEmotions = int.Parse(tempdate[2]);
                MinComments = int.Parse(tempdate[3]);
                MinCommentLength = int.Parse(tempdate[4]);
            }
            catch
            {
                MessageBox.Show(Enumerations.NoMatchDate);
                return;
            }
            GoButton.Dispatcher.Invoke(new Action(delegate
            {
                GoButton.IsEnabled = false;
            }), null);

            doflag = true;
            MessageList.Clear();
            WorkerUrl = new BackgroundWorker();
            WorkerUrl.WorkerSupportsCancellation = true;
            WorkerUrl.DoWork += new DoWorkEventHandler(WorkerUrl_DoWork);
            WorkerUrl.RunWorkerAsync();

            WorkerBar = new BackgroundWorker();
            WorkerBar.WorkerReportsProgress = true;
            WorkerBar.WorkerSupportsCancellation = true;
            WorkerBar.DoWork += new DoWorkEventHandler(WorkerBar_DoWork);
            WorkerBar.ProgressChanged += new ProgressChangedEventHandler(WorkerBar_ProgressChanged);
            //WorkerBar.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerBar_RunWorkerCompleted);
            progressBar.Value = 0;
            WorkerBar.RunWorkerAsync();

            MessageViewThread = new Thread(new ThreadStart(delegate
            {
                while (doflag)
                {
                    Thread.Sleep(500);
                    MessageView.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        MessageView.Items.Clear();
                        if (MessageList.Count <= 1000)
                            for (int i = 0; i < MessageList.Count; i++)
                            {
                                string[] temp = MessageList[i].Split('-');
                                MessageView.Items.Add(new { InfoHeader = temp[0], MessageHeader = temp[1] });
                            }
                        else
                        {
                            for (int i = MessageList.Count - 1000; i < MessageList.Count; i++)
                            {
                                string[] temp = MessageList[i].Split('-');
                                MessageView.Items.Add(new { InfoHeader = temp[0], MessageHeader = temp[1] });
                            }
                        }
                        length = MessageList.Count;
                        if (MessageView.Items.Count > 0)
                            MessageView.ScrollIntoView(MessageView.Items[MessageView.Items.Count - 1]);
                    }), null);
                }
            }));
            MessageViewThread.Start();

            WorkBoxThread = new Thread(new ThreadStart(delegate
            {
                while (doflag)
                {
                    Thread.Sleep(500);
                    WorkBox.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        WorkBox.Text = workpercent.ToString() + " % Complete! Totle: " + urls.Count + " VisitedDate: " + VisitedDate;
                    }), null);
                }
            }));
            WorkBoxThread.Start();
        }
        private void WriteLog(string name, string date)
        {
            try
            {
                Directory.CreateDirectory(Enumerations.directory + date + "\\");
                using (StreamWriter writer = new StreamWriter(Enumerations.directory + date + "\\" + name + ".txt"))
                {
                    MessageList.ForEach(x =>
                    {
                        //if (x.Contains("Download") || x.Contains("Scan"))
                        writer.WriteLine(x);
                    });
                }
            }
            catch
            {

            }
        }
        private void WorkerUrl_DoWork(object sender, DoWorkEventArgs e)
        {
            workpercent = 0;
            dateend = dateend.AddDays(1);
            int summary = dateend.DayOfYear - datebegin.DayOfYear + 365 * (dateend.Year - datebegin.Year);
            //int i = 1;
            ChangeList(Enumerations.ScanMessage);
            for (DateTime someday = datebegin; !someday.Equals(dateend); someday = someday.AddDays(1))
            {
                if (!doflag)
                    return;
                //workpercent = i * 100 / summary;
                //http://news.sina.com.cn/society/20140508.shtml
                string month = someday.Month.ToString();
                if (month.Length == 1)
                    month = "0" + month;
                string day = someday.Day.ToString();
                if (day.Length == 1)
                    day = "0" + day;
                string DealDate = someday.Year.ToString() + month + day;
                string DealDate2 = someday.Year.ToString() + "-" + month + "-" + day;
                VisitedDate = DealDate2;
                string BaseUrl = Enumerations.baseweb + DealDate + ".shtml";
                urls.Clear();
                MessageList.Clear();
                Scan_Worker(BaseUrl, DealDate2);

                //WriteLog("Log");
                //ChangeList(Enumerations.ScanMessage3);

                workpercent = 0;
                ChangeList(Enumerations.DownloadMessage + DealDate);
                ChangeList(Enumerations.DownloadMessage4 + urls.Count);
                for (int i = 0; i < urls.Count; i++)
                {
                    if (!doflag)
                        return;
                    workpercent = (i + 1) * 100 / urls.Count;
                    Download_Worker(urls[i], DealDate2);
                }
                //urls.ForEach(x =>
                //{
                //    Download_Worker(x);
                //});
                WriteLog("Log", DealDate2);
                ChangeList(Enumerations.DownloadMessage3 + DealDate);
            }
            GoButton.Dispatcher.Invoke(new Action(delegate
            {
                GoButton.IsEnabled = true;
            }), null);
        }
        private void Scan_Worker(string url, string date)
        {
            try
            {
                string target_url = @"http://news.sina.com.cn.{0,6}?/" + date + "/.{9,20}?.shtml";// + date + @"/*?.shtml"
                //string target_url2 = Enumerations.baseweb2 + date + @"/\d{9,18}.shtml";
                content = helper.HttpDownloads(url);
                if (helper.Errorflag)
                {
                    helper.Errorflag = false;
                    ChangeList("Error-" + content);
                    return;
                }
                match = new Regex(target_url, RegexOptions.Singleline).Matches(content);
                foreach (Match m in match)
                    if (!urls.Contains(m.ToString()))
                        urls.Add(m.ToString());
                //match = new Regex(target_url2, RegexOptions.Singleline).Matches(content);
                //foreach (Match m in match)
                //    if (!urls.Contains(m.ToString()))
                //        urls.Add(m.ToString());
                if (urls.Count == 0)
                    ChangeList(Enumerations.MatchMessage + date);
                else
                    ChangeList(Enumerations.ScanMessage2 + date);
            }
            catch (Exception e)
            {
                ChangeList("Error-" + e.Message);
            }
        }
        private void Download_Worker(string url, string datatime)
        {
            try
            {
                content = helper.HttpDownloads(url);
                match = new Regex("(?<=charset=).+?(?=\")", RegexOptions.Singleline).Matches(content);
                if (match.Count != 0)
                {
                    string code = match[0].ToString().Trim('\"');
                    content = helper.HttpDownloads(url, code);
                }
                if (helper.Errorflag)
                {
                    helper.Errorflag = false;
                    ChangeList("Error-" + content);
                    return;
                }
                //http://news.sina.com.cn/s/2014-06-04/090430289752.shtml
                string date = datatime;
                string newsid = "";
                try
                {
                    newsid = url.Split(new string[1] { datatime }, StringSplitOptions.RemoveEmptyEntries)[1];
                    newsid = newsid.Trim('/').Split('.')[0];
                }
                catch (Exception)
                { }

                //if (url.Split('/')[4].Contains("p"))
                //{
                //    date = url.Split('/')[5];
                //    newsid = url.Split('/')[6].Split('.')[0];
                //}
                //else
                //{
                //    date = url.Split('/')[4];
                //    newsid = url.Split('/')[5].Split('.')[0];
                //}
                #region img
                List<string> ImgList = new List<string>();
                match = new Regex(Enumerations.EmotionImg1, RegexOptions.Singleline).Matches(content);
                if (match.Count == 0)
                {
                    ChangeList(Enumerations.NoMatchImg + newsid);
                    //return;
                }
                else
                {
                    foreach (Match m in match)
                    {
                        MatchCollection temp = new Regex(Enumerations.EmotionImg2, RegexOptions.Singleline).Matches(m.ToString());
                        if (temp.Count != 0)
                        {
                            string ImgTemp = temp[0].ToString();
                            if (ImgTemp != null && ImgTemp.Contains(date) && ImgTemp.Contains("sinaimg") && !ImgList.Contains(ImgTemp))
                                ImgList.Add(ImgTemp);
                        }
                    }
                    if (ImgList.Count == 0)
                    {
                        ChangeList(Enumerations.NoMatchImg + newsid);
                        //return;
                    }
                }
                #endregion
                #region title
                match = new Regex(Enumerations.EmotionTitle, RegexOptions.Singleline).Matches(content);
                string Title = "";
                if (match.Count != 0)
                {
                    Title = match[0].ToString();
                    if (Title.Contains("(图)"))
                        Title = Title.Replace("(图)", "");
                }
                #endregion
                #region publish
                string publish = "";
                match = new Regex(Enumerations.EmotionPublish, RegexOptions.Singleline).Matches(content);
                if (match.Count == 0)
                {
                    match = new Regex("(?<=<div class=\"article article).+?(?=<div class=\"news_weixin_ercode clearfix)", RegexOptions.Singleline).Matches(content);
                }
                publish = match[0].ToString();
                match = new Regex(Enumerations.EmotionPage, RegexOptions.Singleline).Matches(publish);
                publish = "";
                foreach (Match m in match)
                {
                    string temp = m.ToString().Replace("&nbsp", "").Replace("<strong>", "").Replace("</strong>", "");
                    if (!temp.Contains("href"))
                        publish += temp;
                }
                #endregion
                #region comments
                //http://comment5.news.sina.com.cn/page/info?format=js&channel=sh&newsid=1-1-30359924&group=0&compress=1&ie=gbk&oe=gbk&page=1&page_size=100
                //http://comment5.news.sina.com.cn/page/info?format=js&channel=sh&newsid=comos-fxkniup6306520" />< meta name = "sudameta" content = "comment_channel&group=0&compress=1&ie=gbk&oe=gbk&page=1&page_size=100
                //http://comment5.news.sina.com.cn/page/info?version=1&format=js&channel=sh&newsid=comos-fxksqis4728457&group=&compress=0&ie=utf-8&oe=utf-8&page=1&page_size=20&jsvar=loader_1447578670813_40693976
                List<string> CommentList = new List<string>();
                List<int> CommentAgree = new List<int>();
                int[] CommentIndex;
                //Dictionary<int, string> CommentDic = new Dictionary<int, string>();
                match = new Regex(Enumerations.EmotionKeyword, RegexOptions.Singleline).Matches(content);
                if (match.Count == 0)
                    return;
                string keyword = match[0].ToString().Split(':')[0];
                string id = match[0].ToString().Split(':')[1].Split('"')[0];
                string CommentUrl = "http://comment5.news.sina.com.cn/page/info?format=js&channel=" + keyword + "&newsid=" + id + "&group=0&compress=1&ie=gbk&oe=gbk&page=1&page_size=100";
                content = helper.HttpDownloads(CommentUrl);
                if (helper.Errorflag)
                {
                    helper.Errorflag = false;
                    ChangeList("Error-" + content);
                    return;
                }
                match = new Regex(Enumerations.EmotionComment, RegexOptions.Singleline).Matches(content);
                foreach (Match m in match)
                {
                    string TempComment = m.ToString();
                    match = new Regex(Enumerations.EmotionComment2, RegexOptions.Singleline).Matches(TempComment);
                    if (match.Count == 0)
                        continue;
                    string comment = match[0].ToString();
                    //Regex.Unescape = HttpUtility.UrlDecode(comment);
                    comment = Regex.Unescape(comment);
                    match = new Regex(Enumerations.EmotionComment3, RegexOptions.Singleline).Matches(TempComment);
                    if (match.Count == 0)
                        continue;
                    int agree = int.Parse(match[0].ToString());
                    if (!CommentList.Contains(comment) && comment.Length > MinCommentLength)
                    {
                        CommentList.Add(comment);
                        CommentAgree.Add(agree);
                    }
                }
                int j = 0;
                CommentIndex = new int[CommentAgree.Count];
                CommentAgree.ForEach(x => { CommentIndex[j] = j++; });
                Array.Sort(CommentAgree.ToArray(), CommentIndex);
                Array.Reverse(CommentIndex);
                if (CommentList.Count < MinComments)
                {
                    ChangeList(Enumerations.NoEnoughComment + newsid);
                    return;
                }
                #endregion
                #region emotions
                List<string> EmotionList = new List<string>();
                string EmotionUrl = "http://comment5.news.sina.com.cn/count/info?callback=moodObj.callback&key=" + id;
                content = helper.HttpDownloads(EmotionUrl);
                if (helper.Errorflag)
                {
                    helper.Errorflag = false;
                    ChangeList("Error-" + content);
                    return;
                }
                int summary = 0;
                int i = 0;
                foreach (string emotionstr in Enumerations.Emotions)
                {
                    i++;
                    match = new Regex(emotionstr, RegexOptions.Singleline).Matches(content);
                    if (match.Count == 0)
                        EmotionList.Add("0");
                    else
                    {
                        EmotionList.Add(match[0].ToString());
                        if (i != 1)
                            summary += int.Parse(match[0].ToString());
                    }
                }
                EmotionList[0] = summary.ToString();
                if (int.Parse(EmotionList[0]) < MinEmotions)
                {
                    ChangeList(Enumerations.NoEnoughEmotion + newsid);
                    return;
                }
                #endregion
                #region write
                if (string.IsNullOrWhiteSpace(Enumerations.directory))
                    Enumerations.directory = ".\\";
                string path = Enumerations.directory + date + "\\" + newsid + "\\";
                Directory.CreateDirectory(path);
                using (StreamWriter writer = new StreamWriter(path + "news" + ".txt"))
                {
                    writer.WriteLine("keyword:" + newsid);
                    writer.WriteLine("date:" + date);
                    writer.WriteLine("url:" + url);
                    writer.WriteLine("img:" + ImgList.Count);
                    i = 1;
                    ImgList.ForEach(x =>
                    {
                        writer.WriteLine("imgurl:" + x);
                        string temp = helper.DownloadsImg(x, path + "img" + (i++).ToString() + ".jpg");
                        if (helper.Errorflag)
                        {
                            helper.Errorflag = false;
                            ChangeList("Error-" + temp);
                        }
                    });
                    writer.WriteLine("title:" + Title);
                    writer.WriteLine("publish:" + publish);
                    writer.WriteLine("order:" + "总数 感动 震惊 搞笑 难过 新奇 愤怒");
                    //总数 感动 震惊 搞笑 难过 新奇 愤怒
                    EmotionList.ForEach(x =>
                    {
                        writer.WriteLine("Emotion:" + x);
                    });
                    int k = Math.Min(CommentList.Count, maxcomment);
                    writer.WriteLine("CommentCount:" + k);
                    for (i = 0; i < k; i++)
                    {
                        writer.WriteLine("Comment:" + CommentList[CommentIndex[i]]);
                        writer.WriteLine("Agree:" + CommentAgree[CommentIndex[i]].ToString());
                    }
                }
                #endregion
                ChangeList(Enumerations.DownloadMessage2 + newsid);
            }
            catch (Exception e)
            {
                ChangeList("Error-" + e.Message);
            }
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {

            //WorkerUrl
            WorkerUrl.CancelAsync();
            WorkerUrl.Dispose();
            MessageViewThread.Abort();
            WorkBoxThread.Abort();
            WorkBox.Text = "";
            GoButton.IsEnabled = true;
            MessageView.Items.Clear();
            MessageList.Clear();
            urls.Clear();
            length = 0;
            //WorkerBar
            doflag = false;
            workpercent = 0;
            WorkerBar.CancelAsync();
            WorkerBar.Dispose();
            progressBar.Value = 0;
        }
    }
}
