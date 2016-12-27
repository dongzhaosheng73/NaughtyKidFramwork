using System.IO;
using System.Text;
using NaughtyKid.WinAPI;

namespace NaughtyKid.MyTools
{
    public class InIConfingHelper
    {
        #region 变量
        /// <summary>
        /// INI路径
        /// </summary>
        private static string InIsPath { set; get; }
        #endregion
        #region INI
        /// <summary>
        /// 设置INI路径
        /// </summary>
        /// <param name="path"></param>
        public   InIConfingHelper(string path)
        {
            InIsPath = path;
        }
        /// <summary>
        /// 设置INI路径
        /// </summary>
        public InIConfingHelper()
        {
            InIsPath = Directory.GetCurrentDirectory() + "\\Set.ini";
        }
        /// <summary>
        /// INI写入 使用前先调用void Ini 赋值路径
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="value">值</param>
        public  void Write(string section, string key, string value)
        {

            // section=配置节，key=键名，value=键值，path=路径

           WinApiHepler.WritePrivateProfileString(section, key, value, InIsPath);

        }
        /// <summary>
        /// INI读取 使用前先调用void Ini 赋值路径
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <returns>返回读取值</returns>
        public  string ReadValue(string section, string key)
        {

            // 每次从ini中读取多少字节

            StringBuilder temp = new StringBuilder(255);

            // section=配置节，key=键名，temp=上面，path=路径

            WinApiHepler.GetPrivateProfileString(section, key, "", temp, 255, InIsPath);

            return temp.ToString();

        }
        /// <summary>
        /// INI读取预设值比较返回true false
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="comparedvalue">预设值</param>
        /// <returns>返回bool</returns>
        public  bool ReadValue(string section, string key, string comparedvalue)
        {

            // 每次从ini中读取多少字节

            StringBuilder temp = new StringBuilder(255);

            // section=配置节，key=键名，temp=上面，path=路径

            WinApiHepler.GetPrivateProfileString(section, key, "", temp, 255, InIsPath);

            if (temp.ToString() != comparedvalue) return false;
            return true;

        }
        #endregion
    }
}
