
using System;
using System.Windows.Threading;
using NaughtyKid.Interface;

namespace NaughtyKid.Error
{
    /// <summary>
    /// dll内部错误捕获
    /// </summary>
    public class ErrorHelper
    {
        public delegate void ErrorInform(Exception error, string name);

        public static event ErrorInform EventDllErrorInform;
        /// <summary>
        /// 错误推送
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <param name="name">错误方法</param>
        public static void ErrorPutting(Exception error,string name)
        {
            if (EventDllErrorInform != null) EventDllErrorInform(error,name);
        }

      

       
    }
}
