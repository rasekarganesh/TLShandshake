using System;
using TLShandshake;

namespace Test.TLShandshake
{
    class Program
    {
        static void Main(string[] args)
        {
            CK cK = new CK();
            string[] cookies;
            cK.TryGetCookies(new Uri("https://www.google.com/"), out cookies);
            foreach (var item in cookies)
            {
                Console.WriteLine(item);
            }
            if (cookies.Length == 0)
            {
                Console.WriteLine("No Cookies found.");
            }
            Console.ReadKey();
        }
    }
}
