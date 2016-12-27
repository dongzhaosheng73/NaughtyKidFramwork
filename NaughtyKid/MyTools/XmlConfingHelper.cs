using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NaughtyKid.Annotations;
using NaughtyKid.Error;
using NaughtyKid.Interface;

namespace NaughtyKid.MyTools
{
 
    public class XmlconfigHelper 
    {
        /// <summary>
        /// 生成xaml配置文件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dir">配置文件路径</param>
        /// <param name="obj">要序列化的类</param>
        /// <returns></returns>
        public static bool XmlWriter<T>(string dir, T obj) where T : class
        {
            try
            {

                using (var stringWriter = new StringWriter(new StringBuilder()))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stringWriter, obj);
                    var fs = new FileStream(dir,
                        FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                    fs.Seek(0, SeekOrigin.Begin);

                    fs.SetLength(0);

                    var sw = new StreamWriter(fs);

                    sw.Write(stringWriter.ToString());
                    sw.Close();
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }

        public static bool XmlWriter<T>(string dir, T obj,[NotNull]Encoding encoding) where T : class
        {
            try
            {

                using (var stringWriter = new StringWriter(new StringBuilder()))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(stringWriter, obj);
                    var fs = new FileStream(dir,
                        FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                    fs.Seek(0, SeekOrigin.Begin);

                    fs.SetLength(0);

                    var sw = new StreamWriter(fs, encoding);

                    sw.Write(stringWriter.ToString());
                    sw.Close();
                    fs.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }

        /// <summary>
        /// 读取xaml配置文件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dir">读取配置文件路径</param>
        /// <returns></returns>
        public static T XmlRead<T>(string dir) where T : new()
        {
            try
            {
                if (!File.Exists(dir))
                {
                    return new T();
                }

                var xmlSerializer = new XmlSerializer(typeof(T));

                FileStream fs = new FileStream(dir, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
                fs.Seek(0, SeekOrigin.Begin);

                StreamReader rw = new StreamReader(fs);

                var result = (T)xmlSerializer.Deserialize(rw);

                rw.Close();

                fs.Close();

                return result;
            }
            catch (Exception ex)
            {
                var result = new T();
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return result;
            }

        }
        /// <summary>
        /// 读取xaml配置文件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dir">读取配置文件路径</param>
        /// <returns></returns>
        public static T XmlRead<T>(string dir, [NotNull] Encoding encoding = null) where T : new()
        {
            try
            {
                if (!File.Exists(dir))
                {
                    return new T();
                }

                var xmlSerializer = new XmlSerializer(typeof(T));

                FileStream fs = new FileStream(dir, FileMode.Open,
                    FileAccess.Read, FileShare.Read);
                fs.Seek(0, SeekOrigin.Begin);

                StreamReader rw = new StreamReader(fs, encoding);

                var result = (T)xmlSerializer.Deserialize(rw);

                rw.Close();

                fs.Close();

                return result;
            }
            catch (Exception ex)
            {
                var result = new T();
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return result;
            }

        }

       
    }

}
