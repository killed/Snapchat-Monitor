using System.IO;
using System.Net;
using System.Text;

namespace Snapchat_Monitor.Classes
{
    class HTTP
    {
        public static void Request(string url, string data = null)
        {
            string Body = string.Empty;

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);
            Request.UserAgent = "Snapchat Monitor/v1";
            Request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            try
            {
                if (data != null)
                {
                    byte[] PostData = Encoding.ASCII.GetBytes(data);

                    Request.Method = WebRequestMethods.Http.Post;
                    Request.ContentType = "application/json";

                    Request.ContentLength = PostData.Length;
                    Request.GetRequestStream().Write(PostData, 0, PostData.Length);
                }

                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                Body = new StreamReader(Response.GetResponseStream()).ReadToEnd();

                Response.Close();
            }
            catch (WebException Ex)
            {
                if (Ex.Response != null)
                {
                    Body = new StreamReader(Ex.Response.GetResponseStream()).ReadToEnd();
                    Ex.Response.Close();
                }
            }
        }
    }
}
