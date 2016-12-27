using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using NaughtyKid.Error;
using NaughtyKid.Extension;

namespace NaughtyKid.WatchFilesServices
{
    public class WatchFilesServices : IWatchFilesServices
    {
        public string Path { set; get; }

        public int TimeSpan = 1000;

        public List<string> Searchpattern { set; get; }

        public string RegexStr { set; get; }

        public string NoMatchDirectory = string.Format("{0}\\RegexNoMatch\\", Directory.GetCurrentDirectory());

        public bool IsRunWatch { get; private set; }

        public SearchOption Option { set; get; }

        public WatchFilesServices(string path)
        {
            Path = path;
        }

        public WatchFilesServices(string path, SearchOption option, string searchpattern = "*.*")
        {
            Searchpattern = searchpattern.Split('|').ToList();
            Path = path;
            Option = option;
        }

        public WatchFilesServices(string path, int timespan, string regex, string searchpattern = "*.*")
        {
            Path = path;
            TimeSpan = timespan;
            Searchpattern = searchpattern.Split('|').ToList();
            RegexStr = regex;
        }

        public WatchFilesServices(string path, int timespan, string regex, SearchOption option,
            string searchpattern = "*.*")
        {
            Path = path;
            TimeSpan = timespan;
            Searchpattern = searchpattern.Split('|').ToList();
            RegexStr = regex;
            Option = option;
        }

        public event FindFilesEventHandler EventFindFilesEventHandler;
        public event WatchErrorEventHander EventWathcErrorEventHadler;

        /// <summary>
        /// 循环遍历
        /// </summary>
        public void RunWatch()
        {
            if (IsRunWatch) return;

            IsRunWatch = true;
            new Thread(x =>
            {
                while (IsRunWatch)
                {
                    try
                    {
                        var dates = GetFileFormats();
                        if (dates.Count != 0)
                        {
                            if (EventFindFilesEventHandler != null)
                            {
                                EventFindFilesEventHandler(this, dates);
                            };
                        }

                    }
                    catch(Exception ex)
                    {
                        if(EventWathcErrorEventHadler!=null)EventWathcErrorEventHadler(null, ex);
                    }

                    Thread.Sleep(TimeSpan);
                }
            })
            {
                IsBackground = true,
            }.Start();
        }

        public void StopWatch()
        {
            IsRunWatch = false;
        }

        public IList<string> GetFileFormats()
        {
            var filelist = new List<string>();
            try
            {
                var files = Searchpattern.SelectMany(end => Directory.GetFiles(Path, end, Option)).ToList();
       
                foreach (var file in files)
                {
                    if (!string.IsNullOrEmpty(RegexStr))
                    {
                        var regex = new Regex(RegexStr);

                        if (!regex.IsMatch(file))
                        {
                            if (Directory.Exists(NoMatchDirectory)) Directory.CreateDirectory(NoMatchDirectory);
                            file.CopyFile(string.Format("{0}\\{1}", NoMatchDirectory, System.IO.Path.GetFileName(file)), 1048576,
                                System.IO.Path.GetExtension(file), true);
                        }
                        else
                        {
                            filelist.Add(file);
                        }
                    }
                    else
                    {
                        filelist.Add(file);
                    }
                }
            }
            catch(Exception ex)
            {
                if (EventWathcErrorEventHadler != null) EventWathcErrorEventHadler(null, ex);
            }
          
            return filelist;
        }

    }
}
