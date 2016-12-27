using System;
using System.Collections.Generic;
using NaughtyKid.Model;

namespace NaughtyKid.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="printcontent">Bitmap</param>
    public delegate void PrintEndDelegate(object printcontent);

    internal interface IPrint:IDisposable
    {     
        event PrintEndDelegate EventPrintend;
    }
}
