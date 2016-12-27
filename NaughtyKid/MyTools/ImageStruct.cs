using System.Drawing;

namespace NaughtyKid.MyTools
{
    public class ImageStruct
    {
        /// <summary>
        /// 适配结构
        /// </summary>
        public struct FitSizeTable 
        {
            public long Fitw;
            public long Fith;
            public float Fitsize;
        }
        /// <summary>
        /// 适配结构双精度
        /// </summary>
        public struct FitSizeTableDouble 
        {
            public double Dfitw;
            public double Dfith;
            public double Dfitsize;
        }
        /// <summary>
        /// 适配后的坐标及大小
        /// </summary>
        public struct AdaptationSize
        {
            public FitSizeTableDouble Size;
            public PointF Point;
        }
    }
}
