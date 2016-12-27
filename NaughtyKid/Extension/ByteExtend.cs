using System.Collections.Generic;

namespace NaughtyKid.Extension
{
    public  static  class ByteExtend
    {
        public static int Bytes2Int(this IList<byte> bin, int offset)
        {
            return bin[offset + 0] << 24 | bin[offset + 1] << 16 | bin[offset + 2] << 8 | bin[offset + 3];
        }
    }
}
