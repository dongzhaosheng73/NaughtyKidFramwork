using System;
using System.Web.Script.Serialization;
using NaughtyKid.Error;
using NaughtyKid.Framework.HttpHelper;


namespace NaughtyKid.MyTools
{
    public class SerializationHelper
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static string GetSerialization(object ob)
        {
    
            if (ob == null)
                throw new ArgumentNullException();

           return JsonHelper.ObjectToJson(ob);

        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T ScriptDeserialize<T>(string strJson)
        {
            return (T) JsonHelper.JsonToObject<T>(strJson);

        }
    }
}
