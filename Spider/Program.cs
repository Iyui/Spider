using System;
using System.Collections.Generic;
namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            AdressSpider Ads = new AdressSpider();
            Ads.Crawling("https://" + args[0], null,out List<string> AdressList);
            
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
