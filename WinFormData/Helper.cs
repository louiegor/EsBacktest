using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace WinFormData
{
    public static class FLog
    {
        private static string dateFileName = DateTime.Now.Date.ToString("yyyy-MM-dd");

        // This text is added only once to the file. 
        public static void Updatelog(string text)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + dateFileName +
                          "_ProgramLog2.log";
            if (!File.Exists(path))
            {
                // Create a file to write to. 
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("----------- Program Log ---------");
                }
            }

            // This text is always added, making the file longer over time 
            // if it is not deleted. 

            while (true)
            {
                var tryCount = 0;
                try
                {
                    if (tryCount > 4)
                    {
                        break;
                    }
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        var enterString = String.Format("{0} {1}", DateTime.Now, text);
                        sw.WriteLine(enterString);
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                    tryCount++;
                }
            }
        }

        public static void AddFormLogMessage(string text)
        {
            FormHelper.FormSetText(text);
            Updatelog(text);
        }
    }

    public class Log
    {
        private string dateFileName = DateTime.Now.Date.ToString("yyyy-MM-dd");

        // This text is added only once to the file. 
        public void Updatelog(string text)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + dateFileName +
                          "_BackTestProgramLog.log";
            if (!File.Exists(path))
            {
                // Create a file to write to. 
                try
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine("----------- Program Log ---------");
                    }
                }
                catch{}
            }

            // This text is always added, making the file longer over time 
            // if it is not deleted. 

            while (true)
            {
                var tryCount = 0;
                try
                {
                    if (tryCount >4)
                    {
                        break;
                    }
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        var enterString = String.Format("{0} {1}", DateTime.Now, text);
                        sw.WriteLine(enterString);
                        break;
                    }
                }
                catch
                {
                    Thread.Sleep(500);
                    tryCount++;
                }
            }
        }


    }

    public class Setting
    {
        public string UserId { get; set; }
        public string AmList { get; set; }
        public string NqList { get; set; }
        public string HkFuture { get; set; }
        public string JpFuture { get; set; }
        public string EuFuture { get; set; }
        public string EurUsd { get; set; }
        public string BraFuture { get; set; }
        public string EsFuture { get; set; }
    }

    public class SettingHelper
    {
        public static readonly string SettingPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "setting.xml";

        public Setting GetSetting()
        {
            if (!File.Exists(SettingPath))
            {
                var s = new Setting {UserId = "SCOTHOLL", AmList = "SPY", NqList = "QQQ"};
                SaveSetting(s);
            }

            //var xmlSetting = SettingPath.GetData();
            var xmlSetting = XDocument.Load(SettingPath);
            var userId = xmlSetting.XPathSelectElements("//UserId").Single().Value;
            var amList = xmlSetting.XPathSelectElements("//AmList").SingleOrDefault();
            var nqList = xmlSetting.XPathSelectElements("//NqList").SingleOrDefault();
            var hkFuture = xmlSetting.XPathSelectElements("//HkFuture").SingleOrDefault();
            var jpFuture = xmlSetting.XPathSelectElements("//JpFuture").SingleOrDefault();
            var euFuture = xmlSetting.XPathSelectElements("//EuFuture").SingleOrDefault();
            var braFuture = xmlSetting.XPathSelectElements("//BraFuture").SingleOrDefault();
            var eurUsd = xmlSetting.XPathSelectElements("//EurUsd").SingleOrDefault();
            var esFuture = xmlSetting.XPathSelectElements("//EsFuture").SingleOrDefault();

            if (amList == null || nqList == null)
            {
                var s = new Setting {UserId = userId, AmList = "SPY", NqList = "QQQ"};
                SaveSetting(s);
            }

            return new Setting
                {
                    UserId = userId,
                    AmList = amList == null ? "" : amList.Value,
                    NqList = nqList == null ? "" : nqList.Value,
                    HkFuture = hkFuture == null ? "" : hkFuture.Value,
                    JpFuture = jpFuture == null ? "" : jpFuture.Value,
                    EuFuture = euFuture == null ? "" : euFuture.Value,
                    EurUsd = eurUsd == null ? "" : eurUsd.Value,
                    BraFuture = braFuture == null ? "" : braFuture.Value,
                    EsFuture = esFuture == null ? "" : esFuture.Value,
                };
        }

        public void SaveSetting(Setting setting)
        {
            var writer = new XmlSerializer(typeof (Setting));
            var file = new StreamWriter((SettingPath));
            writer.Serialize(file, setting);
            file.Close();
        }

        public List<string> GetAmList()
        {
            var setting = GetSetting();
            var amList = setting.AmList.Replace(" ","").ToUpper().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return amList;
        }

        public List<string> GetNqList()
        {
            var setting = GetSetting();
            var nqList = setting.NqList.Replace(" ", "").ToUpper().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return nqList;
        }

        public string AmListStringList()
        {
            var setting = GetSetting();
            return setting.AmList;
        }

        public string NqListStringList()
        {
            var setting = GetSetting();
            return setting.NqList;
        }
    }

    public static class TextHelper
    {
        public static string AddTimeStamp(this string text)
        {
            var timeStamp = DateTime.Now.ToString("h:mm:ss tt");
            return timeStamp + " " + text;
        }

        public static string OrganizeList(this string text)
        {
            var tempAmListObj = text.Replace(" ", "").ToUpper().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            tempAmListObj.Sort();
            var temp = string.Join(",", tempAmListObj);
            return temp;
        }
    }

    public static class Helper
    {
        public static double RoundToNearest100 (this double input)
        {
            var result = Math.Floor(input / 100.0) * 100;
            return result;
        }

        public static void AddFormLogMessage(this Log log, string text)
        {
            FormHelper.FormSetText(text);
            log.Updatelog(text);
        }


    }
}
