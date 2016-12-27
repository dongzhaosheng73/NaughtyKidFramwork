using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaughtyKid.Extension
{
    /// <summary>
    /// 类型扩展类
    /// </summary>
    public static class TypeExtend
    {
        /// <summary>
        /// 将字符串转成相应类型
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetStringToType(this Type obj, string value)
        {
            if (obj == typeof(bool))
            {
                return bool.Parse(value);
            }

            if (obj == typeof(int))
            {
                return int.Parse(value);
            }

            if (obj == typeof(DateTime))
            {
               return  DateTime.Parse(value);
            }

            return obj == typeof(string) ? value : null;
        }
    }
}
