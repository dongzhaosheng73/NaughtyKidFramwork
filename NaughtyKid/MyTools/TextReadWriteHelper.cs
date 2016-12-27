using System;
using System.Collections.Generic;
using System.IO;
using NaughtyKid.Interface;
using System.Text;
using NaughtyKid.Annotations;
using NaughtyKid.Error;

namespace NaughtyKid.MyTools
{
    public class TextReadWriteHelper
    {
        /// <summary>
        /// 按行写入text
        /// </summary>
        /// <param name="path"></param>
        /// <param name="meg"></param>
        /// <returns></returns>
        public static bool WriteTextLine(string path, string meg)
        {
            FileStream log = null;
            StreamWriter sw = null;
            try
            {
                log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = log.Length;
                sw = new StreamWriter(log, Encoding.UTF8);
                sw.WriteLine(meg);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
                if (log != null)
                {
                    log.Close();
                    log.Dispose();
                }

            }
        }

        /// <summary>
        /// 按行写入text
        /// </summary>
        /// <param name="path"></param>
        /// <param name="meg"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static bool WriteTextLine(string path, string meg, [NotNull] Encoding encoding = null)
        {
            FileStream log = null;
            StreamWriter sw = null;
            try
            {
                log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = log.Length;
                sw = new StreamWriter(log, encoding);
                sw.WriteLine(meg);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
                if (log != null)
                {
                    log.Close();
                    log.Dispose();
                }

            }
        }

        /// <summary>
        /// 写入text
        /// </summary>
        /// <param name="path"></param>
        /// <param name="meg"></param>
        /// <returns></returns>
        public static bool WriteText(string path, string meg)
        {
            FileStream log = null;
            StreamWriter sw = null;

            try
            {
                log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = log.Length;
                sw = new StreamWriter(log, Encoding.UTF8);
                sw.Write(meg);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }

                if (log != null)
                {
                    log.Close();
                    log.Dispose();
                }
            }
        }

        /// <summary>
        /// 写入text
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="meg">消息</param>
        /// <param name="encoding">编码格式</param>
        /// <returns></returns>
        public static bool WriteText(string path, string meg, [NotNull] Encoding encoding = null)
        {
            FileStream log = null;
            StreamWriter sw = null;

            try
            {
                log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = log.Length;
                sw = new StreamWriter(log, encoding);
                sw.Write(meg);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }

                if (log != null)
                {
                    log.Close();
                    log.Dispose();
                }
            }
        }

        /// <summary>
        /// 按行读取text
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] ReadTextLine(string path)
        {
            FileStream log = null;
            StreamReader sw = null;
            try
            {
                log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = 0;
                sw = new StreamReader(log, Encoding.UTF8);

                var tempList = new List<string>();

                while (sw.Peek() > 0)
                {
                    tempList.Add(sw.ReadLine());
                }
                return tempList.ToArray();
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
                if (log != null)
                {
                    log.Close();
                    log.Dispose();
                }

            }

        }

        /// <summary>
        /// 读取text
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ReadText(string path)
        {
            FileStream log = null;
            StreamReader sw = null;
            try
            {
                log = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                log.Position = 0;
                sw = new StreamReader(log, Encoding.UTF8);
                var readchar = new char[log.Length];
                sw.Read(readchar, 0, readchar.Length);
                return new string(readchar);
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return "";
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw.Dispose();
                }
                if (log != null)
                {
                    log.Close();
                    log.Dispose();
                }

            }
        }

    }

}

