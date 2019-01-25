using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using static Spider.Config;

namespace Spider
{
    public class ImageSpider
    {
   

        public ImageSpider()
        {
            urlOver += new UrlOverEventHandler(ShowMessage);
            urlError += new UrlErrorEventHandler(ShowMessage);
        }
        public struct ImageInfo
        {
            public string Path { get; set; }
            public string Url { get; set; }
        }
        public string path { get; set; } = "Images";//图片目录

        //public delegate void UrlOverEventHandler(string msg);//处理完成
        public event UrlOverEventHandler urlOver = null;

        //public delegate void UrlErrorEventHandler(string errmsg);//发送错误
        public event UrlErrorEventHandler urlError = null;

        public List<ImageInfo> Urls { get; } = new List<ImageInfo>();

        public void AddUrl(string url, string path)
        {
            Urls.Add(new ImageInfo() { Url = url, Path = path });
        }

        public void AddUrl(string url)
        {
            Urls.Add(new ImageInfo() { Url = url });
        }

        public void StartGetImage()//调用此方法开始抓取图片
        {
            if (Urls?.Count <= 0)
            {
                urlError?.Invoke("Url集合为空");
            }
            else
            {
                urlOver?.Invoke("开始抓取图片...");
                foreach (ImageInfo url in Urls)
                {
                    string html = GetHtml(url.Url);
                    List<string> list = GetImgUrlList(html);
                    urlOver?.Invoke("url:{url.Url}" + SaveImg(list, url.Path));
                    urlOver?.Invoke("全部操作完成！");
                }
            }
        }

        string GetHtml(string uri)//请求指定url取得返回的html数据
        {
            Stream rsp = null;
            StreamReader sr = null;

            try
            {
                WebRequest http = WebRequest.Create(uri);
                rsp = http.GetResponse().GetResponseStream();
                sr = new StreamReader(rsp, Encoding.UTF8);
                return "成功:" + sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return "失败：" + ex.Message;
            }
            finally
            {
                sr?.Close();
                rsp.Close();
            }
        }

        List<string> GetImgUrlList(string html)
        {
            if (html?.Substring(0, 2) != "成功")
            {
                return null;
            }
            List<string> list = new List<string>();

            Regex regImg = new Regex(@"src[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            MatchCollection mc = Regex.Matches(html, regImg.ToString(), RegexOptions.Multiline);
            foreach (Match m in mc)
            {
                list.Add(m.Groups["imgUrl"].Value);
            }

            return list;
        }

        string SaveImg(List<string> list, string savepath)
        {
            if (list?.Count <= 0)
            {
                return "未解析到图片地址";
            }
            string dic = path + "\\" + savepath;
            if (!Directory.Exists(dic))
                Directory.CreateDirectory(dic);
            int s = 0, f = 0;
            foreach (string url in list)
            {
                string name = "图片名获取失败";
                //取文件名
                try
                {
                    name = url.Substring(url.LastIndexOf('/') + 1, url.Length - url.LastIndexOf('/') - 5);
                }
                catch
                { continue; }
                WebClient wc = new WebClient();
                try
                {
                    wc.DownloadFile(url, dic + "\\" + name + ".jpg");
                    s++;
                    urlOver?.Invoke($"从{url}抓取图片{ name + ".jpg"}成功！");
                }
                catch (Exception ex)
                {
                    f++; urlOver?.Invoke($"从{url}抓取图片{name + ".jpg"}失败！" /*+ ex.ToString()*/);
                }
                finally { wc.Dispose(); }
            }
            string msg = $"一共抓到{list.Count}个图片地址,成功下载{s}张图片,下载失败{f}张,图片保存路径{dic}";
            return msg;
        }
    }
}
