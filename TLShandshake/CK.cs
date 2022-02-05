using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace TLShandshake
{
    public class CK
    {
        public bool TryGetCookies(Uri url, out string[] cookies)
        {
            List<string> cooks = new List<string>();
            CookieContainer cooki = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cooki;
            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.99 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<Cookie> responseCookies = cooki.GetCookies(url).Cast<Cookie>();

            foreach (Cookie cookie in responseCookies)
            {
                cooks.Add( cookie.Name + ": " + cookie.Value);
            }
            cookies = cooks.ToArray();
            return true;
        }
    }
}
