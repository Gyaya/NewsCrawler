namespace mui
{
    public static class Enumerationc
    {

        //directory
        public static string directory = ".\\Download\\";

        //wbsite
        
        public static string baseweb = "http://www.chinanews.com/scroll-news/sh/";
        public static string baseweb2 = "http://www.chinanews.com";
        public static string baseweb3 = "http://mood.chinanews.com/data/";
        public static string baseweb4 = "http://comment.chinanews.com/ci/index.php/comment/news/more/";
        public static string baseweb5 = "http://www.chinanews.com/sh/";
        #region Regex infomation
        //<title>邻居出门忘拔下房门钥匙 男子留纸条替其保管1天-中新网</title>
        public static string EmotionTitle = "(?<=<title>).+?(?=</title>)";
        //正文
        public static string EmotionPublish = "(?<=<!--正文start-->).+?(?=<!--正文start-->)";
        //img
        public static string EmotionImg = "(?<=src=\u0022).+?(?=\u0022)";
        public static string EmotionImg1 = "(?<=<!--图片start-->).+?(?=<!--图片end-->)";

        //comment  <div class='comment_p' style='font-size:14px;line-height:22px;text-indent:24px;word-break: break-all;overflow:hidden;word-wrap:break-word;'>逗比，年收入100万的家庭，一个月就给600块生活费？小编，扯尼玛蛋</div>
        public static string EmotionComment = "(?<=<div class='comment_p).+?(?=</div>)";
        public static string EmotionComment2 = "(?<=>).*";

        //img <IMG title=似虾又似鱼的怪物 alt=似虾又似鱼的怪物 src="http://i2.sinaimg.cn/dy/s/2014-06-01/U11252P1T1D30274011F21DT20140601105255.jpg">
        //public static string EmotionImg1 = "(?<=<img ).+?(?=>)";
        //public static string EmotionImg2 = "(?<=src=\u0022).+?(?=\u0022)";
        //<!-- publish_helper  <!-- publish_helper_end -->   <p>  </p>
        //public static string EmotionPublish = "(?<=<!-- publish_helper).+?(?=<!-- publish_helper)";
        public static string EmotionPage = "(?<=<p>).+?(?=</p>)";
        //<meta name="comment" content="sh:1-1-30274011">
        public static string EmotionKeyword = "(?<=<meta name=\u0022comment\u0022 content=\u0022).+?(?=\u0022>)";
        //感动 同情 无聊 愤怒 搞笑 难过 高兴 路过
        //感动
        public static string EmotionMove = "(?<=mood1\u0022:).+?(?=,)";
        //同情
        public static string EmotionSympathy = "(?<=mood2\u0022:).+?(?=,)";
        //无聊
        public static string EmotionBored = "(?<=mood3\u0022:).+?(?=,)";
        //愤怒
        public static string EmotionAngry = "(?<=mood4\u0022:).+?(?=,)";
        //搞笑
        public static string EmotionFunny = "(?<=mood5\u0022:).+?(?=,)";
        //难过
        public static string EmotionSad = "(?<=mood6\u0022:).+?(?=,)";
        //高兴
        public static string EmotionHappy = "(?<=mood7\u0022:).+?(?=,)";
        //路过
        public static string EmotionPass = "(?<=mood8\u0022:).+?(?=,)";
        //all
        public static string[] Emotions = { EmotionMove, EmotionSympathy, EmotionBored, EmotionAngry, EmotionFunny, EmotionSad, EmotionHappy, EmotionPass };
        #endregion

        #region Error infomation
        public static string NoMatchUrl = "Match-NO match urls";
        public static string NoMatchImg = "Match-NO match imgs urls: ";
        public static string NoMatchDate = "NO match Date eg: 2014 01 06-2014 01 07-100-20-20";
        public static string NoEnoughComment = "Error-Not Enough Comments urls: ";
        public static string NoEnoughEmotion = "Error-Not Enough Emotions urls: ";
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
