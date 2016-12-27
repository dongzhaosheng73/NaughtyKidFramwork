
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;
using NaughtyKid.Error;

namespace NaughtyKid.Framework.HttpHelper
{
    
    /// <summary>
    /// 文件下载类
    /// </summary>
    public class FileDownHelper
    {
        public delegate void GetDownloadBytes(int byteleght,object data);

        public bool FileDownload(string url,string filepath,GetDownloadBytes getDownloadBytes,int byteleght=1024)
        {
            try
            {
                //得到客户端请求的对象
                System.Net.HttpWebRequest myrq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                //得到浏览器响应的对象
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)myrq.GetResponse();
                Stream st = myrp.GetResponseStream();
                Stream so = new FileStream(filepath, FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[byteleght];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    so.Write(by, 0, osize);
                    osize = st.Read(by, 0, (int)by.Length);
                    getDownloadBytes((int)totalDownloadedByte,filepath);
                }
                so.Close();
                st.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }        
        }
    }
}