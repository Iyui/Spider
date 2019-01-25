using System;

namespace Spider
{
    public class Config
    {
        public static void ShowMessage(string s)
        {
            Console.WriteLine(s = s ?? "");
        }

        public delegate void UrlOverEventHandler(string msg);//处理完成
        //public event UrlOverEventHandler urlOver = null;

        public delegate void UrlErrorEventHandler(string errmsg);//发送错误
        //public event UrlErrorEventHandler urlError = null;
    }
}
