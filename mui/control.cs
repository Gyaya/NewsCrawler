using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Threading;

namespace mui
{
    public static class control
    {
        public static string path = "C:\\News\\Sina\\";
        public static string url = "http://news.sina.com.cn/society/";
        public static List<string> list = new List<string>();
        public static List<string> list_m = new List<string>();
        public static int year, month, day1, day2;
        const int pages = 20;
        public static bool con = true;
        public static bool scan_ok = false;
        public static bool downlaod_ok = false;


        //返回网址
        public static List<string> Get_Url()
        {
            return list;
        }

        //传参
        public static void Get_Value(string y, string m, string d1, string d2)
        {
            year=int.Parse(y);
            month=int.Parse(m);
            day1=int.Parse(d1);
            day2=int.Parse(d2);
        } 

        //得到固定格式的日期
        public static string Get_Date(int day,bool y)
        {
            string s_month, s_day;
            if(month<10)
                s_month="0"+month.ToString();
            else
                s_month=month.ToString();
            if(day<10)
                s_day="0"+day.ToString();
            else
                s_day=day.ToString();
            if(y)
                return year.ToString() + s_month + s_day;
            return year.ToString()+"-"+s_month+"-"+s_day;
        }

        //扫描网址
        /*public void Scan()
        {
            int i;
            con = true;
            bool succ = true;
            scan_ok = false;
            list.Clear();
            list_m.Clear();
            List<string> temp = new List<string>();
            for(i=day1;i<=day2;i++)
            {
                if (!con)
                    break;
                //http://news.sina.com.cn/s/2013-09-04/052928125713.shtml
                string source_url = url + Get_Date(i,true) + ".shtml";
                string target_url = "http://news.sina.com.cn/s/" + Get_Date(i,false) + @"/\d{8,15}.shtml";
                temp = Match(source_url, target_url, ref succ);
                foreach (string str in temp)
                    if (!list.Contains(str))
                        list.Add(str);
                if (!succ)
                    list_m.Add(Get_Date(i, false) + " failed!");
                else
                    list_m.Add(Get_Date(i, false) + " complete!");
            }
            list_m.Add("Scan complete!");
            scan_ok = true;
            MessageBox.Show("ok");
        }*/

        //匹配url
        public static List<string> Match(string url, string regex, ref bool success)
        {
            int i;
            string content = "";
            List<string> lis = new List<string>();
            Regex reg = new Regex(regex, RegexOptions.Singleline);
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("gb2312"));
                content = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
                res.Close();
            }
            catch (WebException we)
            {
                list_m.Add("Error:" + we.Message + we.Status);
                success = false;
                return lis;
            }
            MatchCollection match = reg.Matches(content);
            if (match.Count == 0)
            {
                list_m.Add("Error: Nothing Found！");
                success = false;
                return lis;
            }
            string[] links = new string[match.Count];
            for (i = 0; i < match.Count; i++)
            {
                links[i] = match[i].ToString();
            }
            foreach (string eachstr in links)
            {
                if (!lis.Contains(eachstr))
                {
                    lis.Add(eachstr);
                }
            }
            success = true;
            return lis;
        }

        //下载网站
        /*public void Download()
        {
            bool succ=true;
            con = true;
            downlaod_ok = false;
            int i;
            foreach (string url in list)
            {
                if (!con)
                    break;
                //time
                List<string> Time_List = Match(url, @"(?<=<span id=\u0022pub_date\u0022>).+?(?=</span>)", ref succ);
                if (!succ)
                {
                    list_m.Add("  error: time not found!" + url);
                    return;
                }
                string Time_Str = Time_List[0];

                //title
                List<string> Title_List = Match(url, @"(?<=title\u0022 content=\u0022).+?(?=\u0022 />)", ref succ);
                if (!succ)
                { 
                    list_m.Add("  error: title not found!" + url);
                    return;
                }
                string Title_Str = Title_List[0];

                //publish
                List<string> Publish_List = Match(url, @"(?<=<p>).+?(?=</p>)", ref succ);
                if (!succ)
                {
                    list_m.Add("    error: publish not found!" + url);
                    return;
                }
                string Publish_Str = "";
                foreach (string pub in Publish_List)
                {
                    if (!pub.Contains("<"))
                        Publish_Str += pub;
                }

                //comment example:comment5.news.sina.com.cn/page/info?format=js&channel=gn&newsid=1-1-27801336&group=&compress=1&ie=gbk&oe=gbk&page=1&page_size=20
                List<string> Key_Word_List = Match(url, @"(?<=comment\u0022 content=\u0022).+?(?=\u0022>)", ref succ);
                if (!succ)
                {
                    list_m.Add(url + "error: key_word not found!");
                    return;
                }
                string Key_word = Key_Word_List[0];
                List<string> Comment_List = new List<string>();
                string new_url = "http://comment5.news.sina.com.cn/page/info?format=js&channel=" + Key_word.Split(':')[0] + "&newsid=" + Key_word.Split(':')[1] + "&group=&compress=1&ie=gbk&oe=gbk&page=1&page_size=100";
                Comment_List = Match(new_url, @"(?<=content\u0022: \u0022).+?(?=\u0022, \u0022)", ref succ);
                if (!succ)
                {
                    list_m.Add("  error: comment not found!" + url);
                    return;
                }

                //mood example comment5.news.sina.com.cn/count/info?callback=moodObj.callback&key=1-1-27804741
                string new_mood_url = "http://comment5.news.sina.com.cn/count/info?callback=moodObj.callback&key=" + Key_word.Split(':')[1];

                //总数@"(?<=sum\u0022: \u0022).+?(?=\u0022)";                
                List<string> summary = Match(new_mood_url, @"(?<=sum\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    summary.Add("0");

                //感动@"(?<=1\u0022: \u0022).+?(?=\u0022)";
                List<string> move = Match(new_mood_url, @"(?<=1\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    move.Add("0");

                //震惊@"(?<=9\u0022: \u0022).+?(?=\u0022)";
                List<string> shock = Match(new_mood_url, @"(?<=9\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    shock.Add("0");

                //搞笑@"(?<=5\u0022: \u0022).+?(?=\u0022)"; 
                List<string> funny = Match(new_mood_url, @"(?<=5\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    funny.Add("0");

                //难过@"(?<=6\u0022: \u0022).+?(?=\u0022)";
                List<string> sad = Match(new_mood_url, @"(?<=6\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    sad.Add("0");

                //新奇@"(?<=7\u0022: \u0022).+?(?=\u0022)";
                List<string> novel = Match(new_mood_url, @"(?<=7\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    novel.Add("0");

                //愤怒@"(?<=4\u0022: \u0022).+?(?=\u0022)";
                List<string> angry = Match(new_mood_url, @"(?<=4\u0022: \u0022).+?(?=\u0022)", ref succ);
                if (!succ)
                    angry.Add("0");

                //存储  
                string s = path + url.Split('/')[4] + "\\";
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
                for (i = 1; i <= Comment_List.Count & i<= 20; i++)
                {
                    writer.WriteLine("");
                    writer.WriteLine("comment" + i + ":" + Comment_List[i-1]);
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
                list_m.Add("    Download complete!" + url);
                
            }
            downlaod_ok = true;
        }*/

        
    }
}
