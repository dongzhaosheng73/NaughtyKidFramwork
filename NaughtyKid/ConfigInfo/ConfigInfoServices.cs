using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using NaughtyKid.Model;
using NaughtyKid.MyTools;

namespace NaughtyKid.ConfigInfo
{
    /// <summary>
    /// 配置文件读取生成类
    /// </summary>
    public class ConfigInfoServices : IConfigInfoServices
    {
        public string SettingPath { set; get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="settingPath">配置文件路径</param>
        public ConfigInfoServices(string settingPath)
        {
            SettingPath = settingPath;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public ConfigInfoServices()
        {
            SettingPath = string.Format("{0}\\Set.ini", System.IO.Directory.GetCurrentDirectory());
        }


        /// <summary>
        /// 读取xml格式的配置文件
        /// </summary>
        /// <typeparam name="T">读取类型</typeparam>
        /// <returns></returns>
        public IConfigInfo ReadXmlConfigInfos<T>() where T : IConfigInfo, new()
        {
            return XmlconfigHelper.XmlRead<T>(SettingPath);
        }
        /// <summary>
        /// 写入xml格式的配置文件
        /// </summary>
        /// <param name="info">写入配置实体</param>
        /// <returns></returns>
        public bool WriteXmlConfigInfos(IConfigInfo info)
        {
            return XmlconfigHelper.XmlWriter(SettingPath, info);
        }
        /// <summary>
        /// 读取INI格式的配置文件
        /// </summary>
        /// <typeparam name="T">读取类型</typeparam>
        /// <returns></returns>
        public IConfigInfo ReadIniConfigInfos<T>() where T : IConfigInfo, new()
        {
            var inIConfing = new InIConfingHelper(SettingPath);

            var retIni = new T();

            var properties = retIni.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                var description =
                    ((DescriptionAttribute)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute)))
                        .Description;

                var getvalue =
                      inIConfing.ReadValue(description, property.Name);

                if (string.IsNullOrWhiteSpace(getvalue)) continue;


                var obj = Convert.ChangeType(getvalue, property.PropertyType);

                property.SetValue(retIni, obj, null);
            }

            return retIni;
        }
        /// <summary>
        /// 写入INI格式的配置文件
        /// </summary>
        /// <param name="obj">写入实体</param>
        /// <returns></returns>
        public bool WriteIniConfigInfos(IConfigInfo obj)
        {
            var inIConfing = new InIConfingHelper(SettingPath);

            var properties = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                var description =
                    ((DescriptionAttribute)Attribute.GetCustomAttribute(property, typeof(DescriptionAttribute)))
                        .Description;

                if (property.GetValue(obj, null) != null)
                    inIConfing.Write(description, property.Name, property.GetValue(obj,null).ToString());
            }

            return true;
        }
        /// <summary>
        /// 写入INI格式的配置文件
        /// </summary>
        /// <param name="setting">标题</param>
        /// <param name="name">属性</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool WriteIniConfigInfos(string setting, string name, string value)
        {
            var inIConfing = new InIConfingHelper(SettingPath);

            inIConfing.Write(setting, name, value);

            return true;
        }

    }
}
