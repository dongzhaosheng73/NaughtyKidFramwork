using System;
using System.Globalization;
using NaughtyKid.MyTools;

namespace NaughtyKid.Extension
{
    public static class DecimalExtend
    {
        /// <summary>
        /// 小数化分数主函数
        /// </summary>
        /// <param name="xx"></param>
        /// <returns></returns>
        public static string[] XXtoBl(this decimal xx)
        {
            Int64 x1 = 1;
            //判断传入的数小数点后有几位小数
            int xxws = xx.ToString(CultureInfo.InvariantCulture).IndexOf(".", StringComparison.Ordinal);
            string xxbf = xx.ToString(CultureInfo.InvariantCulture).Substring(xxws + 1, xx.ToString(CultureInfo.InvariantCulture).Length - xxws - 1);
            for (int i = 0; i < xxbf.Length; i++)
            {
                x1 = x1 * 10;
            }
            Int64 x2 = (Int64)(xx * x1);
            //寻找公约数
            Int64 gys = MathHelper.Gcd(x1, x2);
            x1 = x1 / gys;
            x2 = x2 / gys;
            string[] bl = { x2.ToString(), x1.ToString() };
            return bl;
        }


    }
}
