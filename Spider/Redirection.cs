using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using static Spider.Config;
using System.Net;
using System.Threading.Tasks;
namespace Spider
{
    /// <summary>
    /// 已访问的网址列表
    /// </summary>
    public class VisitedHelper
    {

        /// <summary>
        /// HTTP请求工具类
        /// </summary>
        public class HttpRequestUtil
        {
            /// <summary>
            /// 获取页面html
            /// </summary>
            public static string GetPageHtml(string url)
            {
                try
                {
                    // 设置参数
                    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                    request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0; Trident/4.0)";
                    //发送请求并获取相应回应数据
                    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                    //直到request.GetResponse()程序才开始向目标网页发送Post请求
                    Stream responseStream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                    //返回结果网页（html）代码
                    string content = sr.ReadToEnd();
                    return content;
                }
                catch
                { return ""; }

            }

        }
        public static List<string> VisitedList { get; } = new List<string>();

        #region 判断是否已访问
        /// <summary>
        /// 判断是否已访问
        /// </summary>
        public static bool IsVisited(string url)
        {
            if (VisitedList.Exists(a => a == url))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 添加已访问
        /// <summary>
        /// 添加已访问
        /// </summary>
        public static void Add(string url)
        {
            VisitedList.Add(url);
        }
        #endregion

    }

    public class AdressSpider
    {
        public List<string> VisitedList
        {
            get => VisitedHelper.VisitedList;
        }

        public AdressSpider()
        {
            urlOver += new UrlOverEventHandler(ShowMessage);
            urlError += new UrlErrorEventHandler(ShowMessage);
            
        }

        public event UrlOverEventHandler urlOver = null;
        public event UrlErrorEventHandler urlError = null;
       
        private static int m_CompletedCount = 0;
        /// <summary>
        /// 爬取
        /// </summary>
        public void Crawling(string url, string host)
        {
            if (!VisitedHelper.IsVisited(url))
            {
                VisitedHelper.Add(url);
                urlOver?.Invoke("访问地址:" + url);
                if (host == null)
                {
                    host = GetHost(url);
                }
            }

            string pageHtml = VisitedHelper.HttpRequestUtil.GetPageHtml(url);
            Regex regA = new Regex(@"<a[\s]+[^<>]*href=(?:""|')([^<>""']+)(?:""|')[^<>]*>[^<>]+</a>", RegexOptions.IgnoreCase);
            //递归遍历
            MatchCollection mcA = regA.Matches(pageHtml);

            foreach (Match mA in mcA)
            {
                try
                {
                    string nextUrl = mA.Groups[1].Value;
                    if (nextUrl.IndexOf("javascript") == -1)
                    {
                        if (nextUrl.IndexOf("http") == 0)
                        {
                            if (GetHost(url) == host)
                            {
                                //ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object obj)
                                //{
                                try
                                {
                                    Crawling(nextUrl, host);
                                    m_CompletedCount++;
                                }
                                catch { }
                                // }));
                            }
                        }
                        else
                        {
                            if (GetHost(url) == host)
                            {   
                                try
                                {
                                    Crawling(host + nextUrl, host);
                                    m_CompletedCount++;
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch { }
            }
        }



        public void Crawling(string url, string host, out List<string> AdressList)
        {
            AdressList = new List<string> { url };
            if (!VisitedHelper.IsVisited(url))
            {
                
                urlOver?.Invoke("访问地址:" + url);
                if (host == null)
                {
                    host = GetHost(url);
                }
            }
            string pageHtml = "";
            var iTimeStart = Environment.TickCount;
            Task.Factory.StartNew(() => { pageHtml = VisitedHelper.HttpRequestUtil.GetPageHtml(url); }).Wait(10 * 1000);
            if (Environment.TickCount - iTimeStart > 10000)
            {
                throw new Exception($"访问{url}超时!");//访问首页超时直接停止程序
            }
            
            Regex regA = new Regex(@"<a[\s]+[^<>]*href=(?:""|')([^<>""']+)(?:""|')[^<>]*>[^<>]+</a>", RegexOptions.IgnoreCase);
            MatchCollection mcA = regA.Matches(pageHtml);
            foreach (Match mA in mcA)
            {
                AdressList.Add(mA.Groups[1].Value);
            }
        }


        /// <summary>
        /// 获取主机
        /// </summary>
        private string GetHost(string url)
        {
            Regex regHost = new Regex(@"(?:http|https)://[a-z0-9\-\.:]+", RegexOptions.IgnoreCase);
            Match mHost = regHost.Match(url);
            return mHost.Value + "/";
        }


    }

}