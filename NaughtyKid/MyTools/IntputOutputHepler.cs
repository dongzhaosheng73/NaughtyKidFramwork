using System;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Xml.Serialization;
using NaughtyKid.BaseClass;
using NaughtyKid.Error;

namespace NaughtyKid.MyTools
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    public delegate void DeleteIndex(int index);
    /// <summary>
    /// 回调委托
    /// </summary>
    /// <param name="mesg"></param>
    public delegate void MemoryCallBack(float mesg);

    /// <summary>
    /// 输入输出帮助类
    /// </summary>
    public class IntputOutputHepler :PropertyChangedBase
    {
        /// <summary>
        /// 是否启用监视内存回调
        /// </summary>
        private static bool _memoryCallBack;

        /// <summary>
        /// 内存占用变量
        /// </summary>
        private float _memoryBytes;
        private float MemoryBytes
        {
            get { return _memoryBytes; }
            set 
            {
                const double tolerance = 0;
                if (Math.Abs(value - _memoryBytes) < tolerance) return;
                _memoryBytes = value; OnPropertyChanged(() => MemoryBytes);
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public IntputOutputHepler()
        {
            PropertyChanged += IntputOutputHepler_PropertyChanged;
        }

        /// <summary>
        /// 属性变化事件
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">PropertyChangedEventArgs</param>
        private void IntputOutputHepler_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           if(e.PropertyName.Contains("MemoryBytes")) {_myCallBackData.MyCallBack(_memoryBytes);}
        }

        /// <summary>
        /// 删除计数
        /// </summary>
        public  event DeleteIndex EventDeletnIndex;
   
        /// <summary>
        /// 用于线程传递的回调类型
        /// </summary>
        struct MyCallBackStruct
        {
            public MemoryCallBack MyCallBack;

            public string Processname;
        }

        private MyCallBackStruct _myCallBackData;      
        /// <summary>
        /// 将数据转换成流数据
        /// </summary>
        /// <param name="dataOriginal">要压缩的内容</param>
        /// <returns>bytes</returns>
        public static byte[] CompressionObject<T>(T dataOriginal)where T:new ()
        {
            try
            {
                if (dataOriginal == null) return null;
                var bFormatter = new XmlSerializer(typeof(T));
                var mStream = new MemoryStream();
                bFormatter.Serialize(mStream, dataOriginal);
                var bytes = mStream.ToArray();
                var oStream = new MemoryStream();
                var zipStream = new DeflateStream(oStream, CompressionMode.Compress);
                zipStream.Write(bytes, 0, bytes.Length);
                zipStream.Flush();
                zipStream.Close();
                return oStream.ToArray();

            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
                return null;
            }
           
        }
        /// <summary>
        /// 将数据流转换成对应类型
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>解压缩的数据</returns>
        public static T DecompressionObject<T>(byte[] bytes)where T:new ()
        {
            try
            {
                if (bytes == null) return new T();
                var mStream = new MemoryStream(bytes);
                mStream.Seek(0, SeekOrigin.Begin);
                var unZipStream = new DeflateStream(mStream, CompressionMode.Decompress, true);
                var bFormatter = new XmlSerializer(typeof(T));
                var dsResult = (T)bFormatter.Deserialize(unZipStream);
                return dsResult;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
                return new T();
            }         
        }
        /// <summary>
        /// 删除指定目录下的所有目录
        /// </summary>
        /// <param name="path">目标目录</param>
        /// <returns></returns>
        public  bool DeleteDirectorys(string path)
        {
            var index = 0;
            foreach (
                var dir in Directory.GetDirectories(path))
            {
                foreach (var file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
                    }
                    index = index + 1;
                    if (EventDeletnIndex != null) EventDeletnIndex(index);
                }
                try
                {
                    if (!Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories).Any()) Directory.Delete(dir);
                }
                catch (Exception ex)
                {
                    ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
                }
            }

            return true;
        }    
        /// <summary>
        /// md5文件校验
        /// </summary>
        /// <param name="pathName">文件路径</param>
        /// <returns></returns>
        public static string GetFileMd5Hash(string pathName)
        {
            string strResult = "";

            MD5CryptoServiceProvider oMd5Hasher =
                new MD5CryptoServiceProvider();

            try
            {
                var oFileStream = new FileStream(pathName, FileMode.Open,
                    FileAccess.Read, FileShare.ReadWrite);

                var arrbytHashValue = oMd5Hasher.ComputeHash(oFileStream);

                oFileStream.Close();

                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”

                var strHashData = BitConverter.ToString(arrbytHashValue);

                //替换-
                strHashData = strHashData.Replace("-", "");

                strResult = strHashData;
            }

            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
            }
            return strResult;
        }
        /// <summary>
        /// 读取当前登陆到系统的用户名
        /// </summary>
        /// <returns>用户名</returns>
        public string GetUserName()
        {
            return Environment.UserName;
        }
        /// <summary>
        /// 根据主机名称获取当前用户在域中的IP地址
        /// </summary>
        /// <returns>IP地址</returns>
        public string GetIpAddress()
        {
            var localIp = "?";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() != "InterNetwork") continue;
                localIp = ip.ToString();
                break;
            }
            return localIp;
        }
        /// <summary>
        /// 读取当前电脑的计算机名称
        /// </summary>
        /// <returns>计算机名</returns>
        public string GetMachineName()
        {
            return Dns.GetHostName();
        }
        /// <summary>
        /// 读取电脑的MAC地址
        /// </summary>
        /// <returns></returns>
        public string GetMacAddress()
        {
            string mac = "";
            var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var moc = mc.GetInstances();
            foreach (var o in moc)
            {
                var mo = (ManagementObject) o;
                if (mo["IPEnabled"].ToString() == "True")
                {
                    mac = mo["MacAddress"].ToString();
                }
            }
            return mac;
        }
        /// <summary>
        /// 获取当前登录用户名匹配在域服务器上的信息。
        /// </summary>
        /// <returns>匹配登陆用户的个人姓名</returns>
        public string GetUserInDomainInfo()
        {
            try
            {
                var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                var domainName = ipGlobalProperties.DomainName;
                var principalContext = new PrincipalContext(ContextType.Domain, domainName);
                var up = UserPrincipal.FindByIdentity(principalContext, Environment.UserName);
                if (up != null) return up.Name;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, MethodBase.GetCurrentMethod().Name);
            }
            return null;
        }
        /// <summary>
        /// 获取正在运行程名字
        /// </summary>
        /// <returns></returns>
        public static string GetProcessName()
        {
            return Process.GetCurrentProcess().ProcessName;
        }
        /// <summary>
        /// 获得进程内存占用
        /// </summary>
        /// <param name="mCallBack">回调函数</param>
        /// <param name="processname">进程名字</param>
        public void GetMemory(MemoryCallBack mCallBack, string processname)
        {
            _memoryCallBack = true;
            var threadmycallback = new Thread(_ThreadMyBack) { IsBackground = true };
            threadmycallback.Start(new MyCallBackStruct() { MyCallBack = mCallBack, Processname = processname });
        }
        /// <summary>
        /// 回调查看内存占用线程
        /// </summary>
        /// <param name="myBackData"></param>
        protected virtual void _ThreadMyBack(object myBackData)
        {
            _myCallBackData = myBackData as MyCallBackStruct? ?? new MyCallBackStruct();

            while (_memoryCallBack)
            {
                using (var pc = new PerformanceCounter
                    ("Process", "Working Set - Private", _myCallBackData.Processname))
                {
                    MemoryBytes = pc.NextValue();              
                }

                Thread.Sleep(1000);
            }
        }
        /// <summary>
        /// 终止内存监视回调
        /// </summary>
        public static void StopMemoryCallBack()
        {
            _memoryCallBack = false;
        }

    }
}
