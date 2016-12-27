using System;
using System.IO;

namespace NaughtyKid.MyTools
{
    public class LogHepler
    {
        private string Path { set; get; }

        public LogHepler(string path)
        {
            Path = path;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="error">错误</param>
        public void WriteError(Exception error)
        {
            FileStream errorlog = File.Open(Path + DateTime.Now.ToString("yyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            errorlog.Position = errorlog.Length;
            StreamWriter sw = new StreamWriter(errorlog);
            sw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
            sw.WriteLine(error.Message + error.Source + error.StackTrace);
            sw.WriteLine(DateTime.Now.ToString("------------------------"));
            sw.Close();
            sw.Dispose();
            errorlog.Close();
            errorlog.Dispose();
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log">日志内容</param>
        public void WriteLog(string log)
        {
            FileStream errorlog = File.Open(Path + DateTime.Now.ToString("yyMMdd") + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            errorlog.Position = errorlog.Length;
            StreamWriter sw = new StreamWriter(errorlog);
            sw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
            sw.WriteLine(log);
            sw.WriteLine(DateTime.Now.ToString("------------------------"));
            sw.Close();
            sw.Dispose();
            errorlog.Close();
            errorlog.Dispose();
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log">日志内容</param>
        /// <param name="fileFullName">日志完整路径</param>
        public static void WriteLog(string log,string fileFullName)
        {
            FileStream errorlog = File.Open(fileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            errorlog.Position = errorlog.Length;
            StreamWriter sw = new StreamWriter(errorlog);
            sw.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
            sw.WriteLine(log);
            sw.WriteLine(DateTime.Now.ToString("------------------------"));
            sw.Close();
            sw.Dispose();
            errorlog.Close();
            errorlog.Dispose();
        }
    }
}
