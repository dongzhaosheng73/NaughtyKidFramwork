using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaughtyKid.BaseClass
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RestDataCommon<T> where T : new()
    {
        /// <summary>
        /// 
        /// </summary>
        public RestDataCommon()
        {

        }

        public int Success { set; get; }

        public int ErrType { set; get; }

        public string ErrMsg { set; get; }

        public T Data { set; get; }
    }
}
