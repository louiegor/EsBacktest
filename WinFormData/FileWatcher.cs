using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace WinFormData
{
    public interface IFileWatcher
    {
        string GetEsignalPath();
        void Watch();
        void StopWatch();
        string FileReader(string fileName);
    }

    public class FileWatcher:IFileWatcher
    {
        private readonly IMainModel model;
        private readonly Dictionary<string, DateTime> lastExecute;
        private readonly Log log;
        

        public FileWatcher(IMainModel model)
        {
            this.model = model;
            log = new Log();
            lastExecute = new Dictionary<string, DateTime>();
            
        }
        
        private static FileSystemWatcher watcher;

        public string GetEsignalPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Interactive Data\FormulaOutput\"; 
        }

        public void Watch()
        {
            watcher = new FileSystemWatcher
            {
                Path = Global.EsignalPath,
                NotifyFilter = NotifyFilters.LastWrite
                               | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Filter = "*.*",
                InternalBufferSize = 65536
            };

            watcher.Changed += OnChanged;
            //watcher.Created += OnChangedDirectory;
            watcher.EnableRaisingEvents = true;
        }

        public void StopWatch()
        {
            watcher.Changed -= OnChanged;
            watcher.Dispose();
        }

        private static DateTime lastRead = DateTime.MinValue;

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //Workaround for firing twice: http://stackoverflow.com/a/3042963
            var lastWriteTime = File.GetLastWriteTime(e.FullPath);
            log.Updatelog(String.Format("Change Detected {0}",e.FullPath));
            
            DateTime lastExecTime;
            
            //Cannot execute the same stock in less than 20 seconds
            if (lastExecute.TryGetValue(e.FullPath, out lastExecTime))
            {
                log.Updatelog(String.Format("Cannot update in less than 20 second - {0} , now is {1}, last execute is {2}", e.FullPath, DateTime.Now, lastExecTime));
                var diff = DateTime.Now - lastExecTime;
                if (diff.TotalSeconds < 10) { return; }
            }
            
            if (lastWriteTime != lastRead)
            {
                var sh = new SettingHelper();
                var setting = sh.GetSetting();
                lastExecute[e.FullPath] = DateTime.Now;
                
                
                var fullPath = e.FullPath;
                var name = e.Name.Replace(".txt", "");

                //FormHelper.FormSetText(String.Format("File Change detected for {0}...", name));

                
                //Non American Stocks
                if (name.Contains("-TSE") || name.Contains("-TC")  || name.Contains(".JP"))
                {
                    name = name.Replace("-TSE", ".JP");
                    name = name.Replace("-TC", ".TO");
                }
                
                else if (name.Contains("total"))
                {
                   CsvParser.UpdateDictionary(e.FullPath);
                }

                //Futures 
                else if (name.ToLowerInvariant() == "japanfuture")
                {
                    name = setting.JpFuture;
                }
                else if (name.ToLowerInvariant() == "hongkongfuture")
                {
                    name = setting.HkFuture;
                }
                else if (name.ToLowerInvariant() == "brazilfuture")
                {
                    name = setting.BraFuture;
                }
                else if (name.ToLowerInvariant() == "esfuture")
                {
                    name = setting.EsFuture;
                }
                else if (name.ToLowerInvariant() == "eurusd")
                {
                }
                //American Stocks - could be amex nasdaq or nyse
                else
                {
                    var amList = Global.AmExceptionList;
                    var nqList = Global.NqExceptionList;
                    if (amList.Contains(name))
                    {
                        name = name + ".AM";
                    }
                    else if(nqList.Contains(name))
                    {
                        name = name + ".NQ";
                    }
                    else
                    {
                        name = name + ".NY";
                    }
                }
                FormHelper.FormSetText(String.Format("File Change detected for {0}...", name));

                //
                // For debug
                //
                if (name.ToLowerInvariant().Contains("test") || name.ToLowerInvariant().Contains("debug"))return;
                var line = FileReader(fullPath).Split(',');
                log.Updatelog(String.Format ("executing side {0} - {1}", line[0], line[1]));
            }
            lastRead = lastWriteTime;
        }

        public string FileReader(string fileName)
        {
            var locked = IsFileLocked(new FileInfo(fileName));
            log.Updatelog("check if file is lock");
            while (locked)
            {
                Thread.Sleep(100);
            }
            log.Updatelog("file is not locked");
            // Read the file and display it line by line.
            string line;

            using (var sr = new StreamReader(fileName))
            {
                line = sr.ReadToEnd();
            }

            log.Updatelog(String.Format("file is {0}",line));
            return line;
        }

        protected bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
    }
}
