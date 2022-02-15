using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TLShandshake
{
    public class CK
    {

        public bool TryGetCookies(Uri url, out string[] cookies)
        {
            List<string> cooks = new List<string>();
            //TcpClient client = new TcpClient();
            //client.Connect(url.Host, url.Port);
            var strData = HttpRequestAsync(url).Result;
            cookies = strData.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            //NetworkStream networkStream = client.GetStream();
            //if (networkStream.CanWrite && networkStream.CanRead)
            //{


            //    Byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there");
            //    networkStream.Write(sendBytes, 0, sendBytes.Length);

            //    // Reads the NetworkStream into a byte buffer.
            //    byte[] bytes = new byte[client.ReceiveBufferSize];
            //    networkStream.Read(bytes, 0, (int)client.ReceiveBufferSize);

            //    // Returns the data received from the host to the console.
            //    string returndata = Encoding.ASCII.GetString(bytes);
            //    Console.WriteLine("host returned: " + returndata);

            //}


            //CookieContainer cooki = new CookieContainer();
            //HttpClientHandler handler = new HttpClientHandler();
            //handler.CookieContainer = cooki;
            //HttpClient client = new HttpClient(handler);
            //client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/97.0.4692.99 Safari/537.36");
            //client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            //HttpResponseMessage response = client.GetAsync(url).Result;
            //IEnumerable<Cookie> responseCookies = cooki.GetCookies(url).Cast<Cookie>();

            //foreach (Cookie cookie in responseCookies)
            //{
            //    cooks.Add( cookie.Name + ": " + cookie.Value);
            //}
           // cookies = cooks.ToArray();
            return true;
        }


        private static async Task<string> HttpRequestAsync(Uri uri)
        {
            string result = string.Empty;

            using (var tcp = new TcpClient(uri.Host, 80))
            using (var stream = tcp.GetStream())
            {
                tcp.SendTimeout = 500;
                tcp.ReceiveTimeout = 1000;
                // Send request headers
                var builder = new StringBuilder();
                builder.AppendLine("GET / HTTP/1.1");
                builder.AppendLine("Host: "+ uri.Host);
               // builder.AppendLine("User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/98.0.4758.82 Safari/537.36");
                //builder.AppendLine("Content-Length: " + data.Length);   // only for POST request
                builder.AppendLine("Connection: close");
                builder.AppendLine();
                var header = Encoding.ASCII.GetBytes(builder.ToString());
                await stream.WriteAsync(header, 0, header.Length);

                // Send payload data if you are POST request
                //await stream.WriteAsync(data, 0, data.Length);

                // receive data
                using (var memory = new MemoryStream())
                {
                    await stream.CopyToAsync(memory);
                    memory.Position = 0;
                    var data = memory.ToArray();

                    var index = BinaryMatch(data, Encoding.ASCII.GetBytes("\r\n\r\n")) + 4;
                    string headers = Encoding.ASCII.GetString(data, 0, index);
                    memory.Position = index;

                    if (headers.IndexOf("Content-Encoding: gzip") > 0)
                    {
                        using (GZipStream decompressionStream = new GZipStream(memory, CompressionMode.Decompress))
                        using (var decompressedMemory = new MemoryStream())
                        {
                            decompressionStream.CopyTo(decompressedMemory);
                            decompressedMemory.Position = 0;
                            result = Encoding.UTF8.GetString(decompressedMemory.ToArray());
                        }
                    }
                    else
                    {
                        result = Encoding.UTF8.GetString(data, index, data.Length - index);
                        //result = Encoding.GetEncoding("gbk").GetString(data, index, data.Length - index);
                    }
                    result = headers;
                }

                //Debug.WriteLine(result);
                return result;
            }
        }
        private static int BinaryMatch(byte[] input, byte[] pattern)
        {
            int sLen = input.Length - pattern.Length + 1;
            for (int i = 0; i < sLen; ++i)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; ++j)
                {
                    if (input[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    return i;
                }
            }
            return -1;
        }
    }


}
