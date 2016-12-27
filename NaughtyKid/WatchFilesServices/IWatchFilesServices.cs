using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaughtyKid.WatchFilesServices
{
    public delegate void FindFilesEventHandler(object sender, IList<string> e);

    public delegate void WatchErrorEventHander(object sender, Exception error);

    public interface IWatchFilesServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IList<string> GetFileFormats();

        event FindFilesEventHandler EventFindFilesEventHandler;

        event WatchErrorEventHander EventWathcErrorEventHadler;

        void RunWatch();

        void StopWatch();
    }
}
