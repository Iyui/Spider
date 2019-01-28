using System;
using System.Collections.Generic;
namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            AdressSpider Ads = new AdressSpider();
            List<string> AdressList = new List<string>();
            try
            {
                Ads.Crawling("https://www.btpian.com", null, out AdressList);
            }
            catch
            {
                Console.WriteLine($"访问https://{args[0]}超时");
            }
            ImageSpider imgspider = new ImageSpider();
            foreach (var vs in AdressList)
            {
                imgspider.AddUrl(vs, "image");
                imgspider.StartGetImage();
            }
            Console.ReadKey();
        }
    }
}
