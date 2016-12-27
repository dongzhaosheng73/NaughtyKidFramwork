using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaughtyKid.Extension
{
    public static class DirectoryInfoExtend
    {
        public static List<FileInfo> GetFilesInfos(this DirectoryInfo dirInfo, SearchOption option, string searchpattern = "*.*",
            char split = '|')
        {
          
            var Searchpattern = searchpattern.Split(split);

            var files = Searchpattern.SelectMany(end => dirInfo.GetFiles(end, option)).ToList();

            return files;
        }
    }
}
