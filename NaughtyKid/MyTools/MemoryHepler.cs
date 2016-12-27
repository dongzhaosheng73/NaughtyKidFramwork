using System;
using System.Diagnostics;
using NaughtyKid.WinAPI;

namespace NaughtyKid.MyTools
{
    public class MemoryHepler
    {
        /// <summary>
        /// 内存释放
        /// </summary>
        public static void FlushMemory()
        {         
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                WinApiHepler.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
    }
}
