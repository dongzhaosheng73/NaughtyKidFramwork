using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NaughtyKid.MyEnum
{
    internal enum SendMessageTimeoutFlags : uint
    {
        SmtoNormal = 0x0000,
        SmtoBlock = 0x0001,
        SmtoAbortifhung = 0x0002,
        SmtoNotimeoutifnothung = 0x0008
    }
}
