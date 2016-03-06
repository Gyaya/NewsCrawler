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

namespace mui
{
    /// <summary>
    /// spider4.xaml 的交互逻辑
    /// </summary>
    public partial class spider4 : Page
    {
        #region para
        //all urls
        private static List<string> urls = new List<string>();
        //private static List<string> imglisturls = new List<string>();
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
        public spider4()
        {
            InitializeComponent();
            DateRange.Text = "2014 01 01-2014 06 06-10-0-0";
            FileAddress.Text = Enumerationc.directory;
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
            Enumerationc.directory = FileAddress.Text;
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
                MessageBox.Show(Enumerationc.NoMatchDate);
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
                        WorkBox.Text = workpercent.ToString() + " % complete! + Totle: " + urls.Count + " VisitedDate: " + VisitedDate;
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
            ChangeList(Enumerationc.ScanMessage);
            for (DateTime someday = datebegin; !someday.Equals(dateend); someday = someday.AddDays(1))
            {
                if (!doflag)
                    return;
                //workpercent = i * 100 / summary;
                //http://www.chinanews.com/scroll-news/sh/2013/0605/news.shtml
                string month = someday.Month.ToString();
                if (month.Length == 1)
                    month = "0" + month;
                string day = someday.Day.ToString();
                if (day.Length == 1)
                    day = "0" + day;
                string DealDate = someday.Year.ToString() + "/" + month + day + "/";
                string DealDate2 = someday.Year.ToString() + "/" + month + "-" + day + "/";
                string DealDate3 = someday.Year.ToString() + "-" + month + "-" + day;
                VisitedDate = DealDate3;
                string BaseUrl = Enumerationc.baseweb + DealDate + "news.shtml";
                urls.Clear();
                MessageList.Clear();
                Scan_Worker(BaseUrl, DealDate2);

                //WriteLog("Log");
                //ChangeList(Enumerationc.ScanMessage3);

                workpercent = 0;
                ChangeList(Enumerationc.DownloadMessage + DealDate3);
                ChangeList(Enumerationc.DownloadMessage4 + urls.Count);
                for (int i = 0; i < urls.Count; i++)
                {
                    if (!doflag)
                        return;
                    workpercent = (i + 1) * 100 / urls.Count;
                    Download_Worker(urls[i]);
                }
                WriteLog("Log", DealDate3);
                ChangeList(Enumerationc.DownloadMessage3 + DealDate3);
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
                //http://www.chinanews.com/sh/2014/06-04/6243991.shtml
                string target_url = Enumerationc.baseweb5 + date + @"\d{4,10}.shtml";
                content = helper.HttpDownloads(url);
                if (helper.Errorflag)
                {
                    helper.Errorflag = false;
                    ChangeList("Error-" + content);
                    return;
                }
                match = new Regex(target_url, RegexOptions.Singleline).Matches(content);
                if (match.Count == 0)
                    ChangeList(Enumerationc.MatchMessage + date);
                else
                {
                    foreach (Match m in match)
                        if (!urls.Contains(m.ToString()))
                            urls.Add(m.ToString());
                    ChangeList(Enumerationc.ScanMessage2 + date);
                }
            }
            catch (Exception e)
            {
                ChangeList("Error-" + e.Message);
            }
        }
        private void Download_Worker(string url)
        {
            //url = "http://www.chinanews.com/sh/2014/03-07/5925787.shtml";
            try
            {
                content = helper.HttpDownloads(url);
                if (helper.Errorflag)
                {
                    helper.Errorflag = false;
                    ChangeList("Error-" + content);
                    return;
                }
                //http://www.chinanews.com/sh/2014/06-04/6244325.shtml
                string date = url.Split('/')[4] + "/" + url.Split('/')[5].Replace("-", "");
                string date2 = url.Split('/')[4] + "/" + url.Split('/')[5].Replace("-", "/");
                string date3 = url.Split('/')[4] + "-" + url.Split('/')[5];
                string newsid = url.Split('/')[6].Split('.')[0];
                #region img
                List<string> ImgList = new List<string>();
                match = new Regex(Enumerationc.EmotionPublish, RegexOptions.Singleline).Matches(content);
                string publishmatch = "";
                if (match.Count != 0)
                {
                    publishmatch = match[0].ToString();
                    match = new Regex(Enumerationc.EmotionImg, RegexOptions.Singleline).Matches(publishmatch);
                    foreach (Match m in match)
                    {
                        string ImgTemp = m.ToString();
                        if (ImgTemp.Contains("jpg") && !ImgList.Contains(ImgTemp))
                        {
                            if (ImgTemp.Contains("http"))
                                ImgList.Add(ImgTemp);
                            else if (ImgTemp.Contains("/"))
                                ImgList.Add(Enumerationc.baseweb2 + ImgTemp);
                            else
                                ImgList.Add(Enumerationc.baseweb2 + "/sh/" + url.Split('/')[4] + "/" + url.Split('/')[5] + "/" + ImgTemp);
                        }
                    }
                }
                match = new Regex(Enumerationc.EmotionImg1, RegexOptions.Singleline).Matches(content);
                if (match.Count != 0)
                {
                    match = new Regex(Enumerationc.EmotionImg, RegexOptions.Singleline).Matches(match[0].ToString());
                    foreach (Match m in match)
                    {
                        string ImgTemp = m.ToString();
                        if (ImgTemp.Contains("jpg") && !ImgList.Contains(ImgTemp))
                        {
                            if (ImgTemp.Contains("http"))
                                ImgList.Add(ImgTemp);
                            else if (ImgTemp.Contains("/"))
                                ImgList.Add(Enumerationc.baseweb2 + ImgTemp);
                            else
                                ImgList.Add(Enumerationc.baseweb2 + "/sh/" + url.Split('/')[4] + "/" + url.Split('/')[5] + "/" + ImgTemp);
                        }
                    }
                }
                if (ImgList.Count == 0)
                {
                    ChangeList(Enumerationc.NoMatchImg + newsid);
                    return;
                }
                #endregion
                #region title
                match = new Regex(Enumerationc.EmotionTitle, RegexOptions.Singleline).Matches(content);
                if (match.Count == 0)
                    return;
                string title = match[0].ToString();
                if (title.Contains("-"))
                    title = title.Replace("-", "");
                if (title.Contains("中新网"))
                    title = title.Replace("中新网", "");
                if (title.Contains("(图)"))
                    title = title.Replace("(图)", "");
                #endregion
                #region publish
                string publish = "";
                match = new Regex(Enumerationc.EmotionPage, RegexOptions.Singleline).Matches(publishmatch);
                publish = "";
                foreach (Match m in match)
                {
                    string temp = m.ToString().Replace("&nbsp", "").Replace("<strong>", "").Replace("</strong>", "");
                    if (!temp.Contains("href"))
                        publish += temp;
                }
                #endregion
                #region comments
                //http://comment.chinanews.com/ci/index.php/comment/news/more/6235622/1
                List<string> CommentList = new List<string>();
                for (int i = 1; i <= 10; i++)
                {
                    string CommentUrl = Enumerationc.baseweb4 + newsid.Split('.')[0] + "/" + i.ToString();
                    content = helper.HttpDownloads(CommentUrl, "utf-8");
                    if (helper.Errorflag)
                    {
                        helper.Errorflag = false;
                        ChangeList("Error-" + content);
                        return;
                    }
                    match = new Regex(Enumerationc.EmotionComment, RegexOptions.Singleline).Matches(content);
                    if (match.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        foreach (Match m in match)
                        {
                            string TempComment = new Regex(Enumerationc.EmotionComment2, RegexOptions.Singleline).Matches(m.ToString())[0].ToString();
                            if (!CommentList.Contains(TempComment) && TempComment.Length > MinCommentLength)
                                CommentList.Add(TempComment);
                            if (CommentList.Count >= 20)
                            {
                                i = 11;
                                break;
                            }
                        }
                    }
                }
                if (CommentList.Count < MinComments)
                {
                    ChangeList(Enumerationc.NoEnoughComment + newsid);
                    return;
                }
                #endregion
                #region emotions
                //http://mood.chinanews.com/data/2014/06/04/4_8_6240395.shtml
                List<string> EmotionList = new List<string>();
                string EmotionUrl = Enumerationc.baseweb3 + date2 + "/4_8_" + newsid + ".shtml";
                content = helper.HttpDownloads(EmotionUrl);
                if (helper.Errorflag)
                {
                    helper.Errorflag = false;
                    ChangeList("Error-" + content);
                    return;
                }
                int summary = 0;
                foreach (string emotionstr in Enumerationc.Emotions)
                {
                    match = new Regex(emotionstr, RegexOptions.Singleline).Matches(content);
                    if (match.Count == 0)
                        EmotionList.Add("0");
                    else
                    {
                        EmotionList.Add(match[0].ToString());
                        summary += int.Parse(match[0].ToString());
                    }
                }
                if (summary < MinEmotions)
                {
                    ChangeList(Enumerationc.NoEnoughEmotion + newsid);
                    return;
                }
                #endregion
                #region write
                if (string.IsNullOrWhiteSpace(Enumerationc.directory))
                    Enumerationc.directory = ".\\";
                string path = Enumerations.directory + date3 + "\\" + newsid + "\\";
                Directory.CreateDirectory(path);
                using (StreamWriter writer = new StreamWriter(path + "news" + ".txt"))
                {
                    writer.WriteLine("keyword:" + newsid);
                    writer.WriteLine("date:" + date);
                    writer.WriteLine("url:" + url);
                    writer.WriteLine("img:" + ImgList.Count);
                    int i = 1;
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
                    writer.WriteLine("title:" + title);
                    writer.WriteLine("publish:" + publish);
                    writer.WriteLine("order:" + "感动 同情 无聊 愤怒 搞笑 难过 高兴 路过");
                    //感动 同情 无聊 愤怒 搞笑 难过 高兴 路过
                    EmotionList.ForEach(x =>
                    {
                        writer.WriteLine("Emotion:" + x);
                    });
                    writer.WriteLine("Comments:" + CommentList.Count);
                    CommentList.ForEach(x =>
                    {
                        writer.WriteLine("Comment:" + x);
                    });
                }
                #endregion
                ChangeList(Enumerationc.DownloadMessage2 + newsid);
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