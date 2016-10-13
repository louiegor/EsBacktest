using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WinFormData
{
    public static class Xml
    {
        private static bool validation;
        private static bool countdown;
        private static int count;

        public static XDocument GetData(this string url)
        {
            if (!validation)
            {
                if (
                    !Connection.SiteConnection(
                        new HttpWebDefaultSetting()))
                {
                    var rnd = new Random();
                    count = rnd.Next(100, 1000);
                    countdown = true;
                }

                validation = true;
            }

            if (countdown && count > 0)
            {
                count--;
                if (count < 1)
                {
                    throw new Exception("Cannot connect to the source");
                }
                return Retry.Do(() => GetDataFromUrl(url), 300);
            }

            return Retry.Do(() => GetDataFromUrl(url), 300);
        }

        public static XDocument GetDataFromUrl(string url)
        {
            var x = new XDocument();

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException exception)
            {
                string responseText;

                using (var reader = new StreamReader(exception.Response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }
            }

            var receiveStream = response.GetResponseStream();

            if (receiveStream == null)
                throw new Exception("Stream is null. Not sure why.");

            using (XmlReader reader = XmlReader.Create(receiveStream))
            {
                x = XDocument.Load(reader);
                return x;
            }
        }
    }

    public class HttpWebDefaultSetting : Site
    {
        public string ChromeId { get; set; }
    }

    public abstract class Site
    {
        public string Uri { get; set; }
    }

    public static class Connection
    {
        public static bool SiteConnection(HttpWebDefaultSetting input)
        {
            input.Uri = @"https://sites.google.com/site/louiegor/";
            string text;
            var httpRequest = (HttpWebRequest)WebRequest.Create(input.Uri);
            var response = (HttpWebResponse)httpRequest.GetResponse();
            var receiveStream = response.GetResponseStream();

            using (var reader = new StreamReader(receiveStream, Encoding.UTF8))
            {
                text = reader.ReadToEnd();
            }

            int first = text.IndexOf("@A@", StringComparison.Ordinal) + "@B@".Length;
            int last = text.LastIndexOf("@B@", StringComparison.Ordinal);
            string title = text.Substring(first, last - first);
            if (title != "louiegor") return false;
            return true;

        }
    }

    //http://stackoverflow.com/questions/1563191/c-sharp-cleanest-way-to-write-retry-logic
    public static class Retry
    {
        public static void Do(
            Action action,
            int retryInterval,
            int retryCount = 5)
        {
            Do<object>(() =>
            {
                action();
                return null;
            }, retryInterval, retryCount);
        }

        public static T Do<T>(
            Func<T> action,
            int retryInterval,
            int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                //if (retry == 1)
                //{

                //}

                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}

