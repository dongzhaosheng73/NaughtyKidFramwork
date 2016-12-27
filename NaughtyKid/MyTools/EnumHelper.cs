using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NaughtyKid.MyTools
{
    /// <summary>
    /// 枚举帮助类
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// 根据枚举获取描述
        /// </summary>
        /// <param name="enumName">枚举</param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumName)
        {
            var _description = string.Empty;
            var _fieldInfo = enumName.GetType().GetField(enumName.ToString());
            var _attributes = _fieldInfo.GetDescriptAttr();
            if (_attributes != null && _attributes.Length > 0)
                _description = _attributes[0].Description;
            else
                _description = enumName.ToString();
            return _description;    
        }

        /// <summary>
        /// 获取描述集合
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            return null;
        }
        /// <summary>
        /// 根据枚举描述获取枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetEnumName<T>(string description)
        {
            var _type = typeof(T);
            foreach (var field in _type.GetFields())
            {
                var curDesc = field.GetDescriptAttr();
                if (curDesc != null && curDesc.Length > 0)
                {
                    if (curDesc[0].Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
        }
    }
}
