using System;

namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageSpider imgspider = new ImageSpider();
            imgspider.AddUrl("https://", "image");
            imgspider.StartGetImage();
            Console.ReadKey();
        }
    }
}
