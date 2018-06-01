using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace J.H_D
{
    public class Askers
    {
        /// <summary>
        /// Send a json string to the given url with the WebRequest client
        /// </summary>
        /// <param name="client">The client to use for make the request, send null to make a single-use request</param>
        /// <param name="url">The url where to set the json</param>
        /// <param name="json">The json to send</param>
        /// <returns></returns>
        public string SendJsonToAPI(HttpWebRequest client, string url, string json)
        {
            string webresponse = null;
            if (client == null)
            {
                var http = (HttpWebRequest)WebRequest.Create(new Uri(url));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";

                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] Bytes = encoding.GetBytes(json);

                Stream sendstream = http.GetRequestStream();
                sendstream.Write(Bytes, 0, Bytes.Length);
                sendstream.Close();

                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);

                webresponse = sr.ReadToEnd();
            }
            else
            {
                ASCIIEncoding encoding = new ASCIIEncoding();
                Byte[] Bytes = encoding.GetBytes(json);

                Stream sendstream = client.GetRequestStream();
                sendstream.Write(Bytes, 0, Bytes.Length);
                sendstream.Close();

                var response = client.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);

                webresponse = sr.ReadToEnd();
            }
            return (webresponse);
        }

        public string SimpleAsk(string url)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Encoding = Encoding.UTF8;
                    return (wc.DownloadString(url));
                }
            }
            catch(WebException e)
            {
                Console.WriteLine(e.Message);
                return (null);
            }
        }

        public string AskwithCredentials(string url, string id, string pass)
        {
            WebClient wc = new WebClient
            {
                Encoding = Encoding.UTF8,
                Credentials = new NetworkCredential(id, pass),
            };
            wc.Headers.Add("User-agent: JH");
            string result = wc.DownloadString(url);
            return (result);
        }

        public void DownloadRessource(string url, string path)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile(url, path);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        public string AskRequest(string url)
        {
            try
            {
                string webresponse = null;

                var http = (HttpWebRequest)WebRequest.Create(new Uri(url));
                http.Accept = "application/json";
                http.Method = "POST";

                Stream sendstream = http.GetRequestStream();
                sendstream.Close();

                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);

                webresponse = sr.ReadToEnd();
                return (webresponse);
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return (null);
            }
        }
        
        public void DownloadRessource_fromstream(string url, string filename)
        {
            Console.WriteLine(url);
            try
            {
                var http = (HttpWebRequest)WebRequest.Create(new Uri(url));
                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);

                MemoryStream mem = new MemoryStream();
                stream.CopyTo(mem);

                byte[] imageData = mem.ToArray();
                File.WriteAllBytes(filename, imageData);
                stream.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void DownloadRessource_fromSankakuComplex(string url, string path)
        {
            try
            {
                WebClient skweb = new WebClient();
                skweb.Headers.Add("User-Agent: JH");

                skweb.DownloadFile(url, path);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public string GetSakuraComplexResponse(string url)
        {
            try
            {
                string final = null;
                var http = (HttpWebRequest)WebRequest.Create(new Uri(url));

                http.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                http.Headers.Add("Accept-Language:fr,fr-FR;q=0.8,en-US;q=0.5,en;q=0.3");
                http.Headers.Add("login:" + File.ReadAllLines("Logers/sancomplexlogins.txt")[0]);
                http.Headers.Add("password_hash:" + File.ReadAllLines("Logers/sancomplexlogins.txt")[1]);
                http.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:62.0) Gecko/20100101 Firefox/62.0";
                http.Host = "capi-beta.sankakucomplex.com";
                http.Method = "GET";

                var response = http.GetResponse();
                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);

                final = sr.ReadToEnd();
                return final;
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                return (null);
            }
        }

        public WebClient init_malCaller(WebClient w)
        {
            if (w == null)
            {
                WebClient malClient = new WebClient
                {
                    Credentials = new NetworkCredential(File.ReadAllLines("Logers/malcredentials.txt")[0], File.ReadAllLines("Logers/malcredentials.txt")[1])
                };
                return (malClient);
            }
            else
            {
                w.Credentials = new NetworkCredential(File.ReadAllLines("Logers/malcredentials.txt")[0], File.ReadAllLines("Logers/malcredentials.txt")[1]);
                return (w);
            }
        }
    }
}
