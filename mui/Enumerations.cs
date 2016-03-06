namespace mui
{
    public static class Enumerations
    {
        //directory
        public static string directory = ".\\Download\\";

        //wbsite
        public static string baseweb = "http://news.sina.com.cn/society/";
        public static string baseweb1 = "http://news.sina.com.cn/s/";
        public static string baseweb2= "http://news.sina.com.cn/s/p/";
        #region Regex infomation

        //img <IMG title=似虾又似鱼的怪物 alt=似虾又似鱼的怪物 src="http://i2.sinaimg.cn/dy/s/2014-06-01/U11252P1T1D30274011F21DT20140601105255.jpg">
        public static string EmotionImg1 = "(?<=<img ).+?(?=>)";
        public static string EmotionImg2 = "(?<=src=\u0022).+?(?=\u0022)";
        //<!-- publish_helper  <!-- publish_helper_end -->   <p>  </p>
        public static string EmotionPublish = "(?<=<!-- publish_helper).+?(?=<!-- publish_helper)";
        public static string EmotionPage = "(?<=<p>).+?(?=</p>)";
        //<meta name="comment" content="sh:1-1-30274011">
        public static string EmotionKeyword = "(?<=<meta name=\u0022comment\u0022 content=\u0022).+?(?=\u0022>)";
        //{"status":    }
        //(?<=title\u0022 content=\u0022).+?(?=\u0022 />)
        public static string EmotionTitle = "(?<=title\u0022 content=\u0022).+?(?=\u0022 />)";
        public static string EmotionComment = "(?<={\u0022status\u0022:).+?(?=})";
        //(?<=content\u0022: \u0022).+?(?=\u0022, \u0022)
        public static string EmotionComment2 = "(?<=content\u0022: \u0022).+?(?=\u0022, \u0022)";
        //(?<=agree\u0022: \u0022).+?(?=\u0022, \u0022)
        public static string EmotionComment3 = "(?<=agree\u0022: \u0022).+?(?=\u0022, \u0022)";
        //summary
        public static string EmotionSummary = "(?<=sum\u0022: \u0022).+?(?=\u0022)";
        //感动
        public static string EmotionMove = "(?<=1\u0022: \u0022).+?(?=\u0022)";
        //震惊
        public static string EmotionShock = "(?<=9\u0022: \u0022).+?(?=\u0022)";
        //搞笑
        public static string EmotionFunny = "(?<=5\u0022: \u0022).+?(?=\u0022)";
        //难过
        public static string EmotionSad = "(?<=6\u0022: \u0022).+?(?=\u0022)";
        //新奇
        public static string EmotionNovel = "(?<=7\u0022: \u0022).+?(?=\u0022)";
        //愤怒
        public static string EmotionAngry = "(?<=4\u0022: \u0022).+?(?=\u0022)";
        //all
        public static string[] Emotions = { EmotionSummary, EmotionMove, EmotionShock, EmotionFunny, EmotionSad, EmotionNovel, EmotionAngry };
        #endregion

        #region newsid infomation
        //time
        public static string newstime = "pub_date";
        //title
        public static string newstitle = "artibodyTitle";
        //img
        public static string newsimg = "img";
        //temp
        public static string temp = "div class=\"img_wrapper\"";
        #endregion

        #region Error infomation
        public static string NoMatchUrl = "Match-NO match urls";
        public static string NoMatchImg = "Match-NO match imgs urls: ";
        public static string NoMatchDate = "NO match Date eg: 2014 01 06-2014 01 07-100-20-20";
        public static string NoEnoughComment = "Error-Not Enough Comments + urls: ";
        public static string NoEnoughEmotion = "Error-Not Enough Emotions + urls: ";
        public static string MatchMessage = "Error-Match 0 item. url: ";
        #endregion

        #region Message infomation
        
        public static string ScanMessage = "Scan-Begin!";
        public static string ScanMessage2 = "Scan-Complete url: ";
        public static string ScanMessage3 = "Scan-Complete!";
        public static string DownloadMessage = "Download-Begin url: ";
        public static string DownloadMessage2 = "Download-Complete url: ";
        public static string DownloadMessage3 = "Download-Complete!";
        public static string DownloadMessage4 = "Download-To be downloaded urls: ";
        #endregion
    }
}
