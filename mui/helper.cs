using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace mui
{
    public static class helper
    {
        public static bool Errorflag = false;
        public static string HttpDownloads(string url)
        {
            string content = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("gb2312")))
                    content = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                content = e.Message;
                Errorflag = true;
            }
            return content;
        }
        public static string HttpDownloads(string url,string Encode)
        {
            string content = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
                using (StreamReader reader = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding(Encode)))
                    content = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                content = e.Message;
                Errorflag = true;
            }
            return content;
        }
        public static string DownloadsImg(string url, string path)
        {
            string content = "";
            try
            {
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = request.GetResponse())
                using (Stream reader = response.GetResponseStream())
                using (FileStream writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buff = new byte[512];
                    int length = 0; //Bytes read
                    int i = 0;
                    while ((length = reader.Read(buff, 0, buff.Length)) > 0)
                    {
                        i++;
                        writer.Write(buff, 0, length);
                        if (i > 100)
                            content = "out of times!";
                    }
                }
            }
            catch (Exception e)
            {
                content = e.Message;
                Errorflag = true;
            }
            return content;
        }
    }
}
