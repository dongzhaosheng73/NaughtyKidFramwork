using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using BarcodeLib;


namespace NaughtyKid.MyTools
{
    /// <summary>
    /// 条码生成帮助工具
    /// </summary>
    public class BarCodeHelper:Barcode
    {
      
        /// <summary>
        /// 获取条码
        /// </summary>
        /// <param name="str">条码内容</param>
        /// <returns></returns>
        public static Image GetBarCodeImage(string  str)
        {
            var b = new Barcode()
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                IncludeLabel = true,
                Alignment = AlignmentPositions.CENTER,
                LabelPosition = LabelPositions.BOTTOMCENTER,
                ImageFormat = ImageFormat.Jpeg,
                Width = 300,
                Height = 150
            };
            return b.Encode(TYPE.CODE128,str );
        }

        /// <summary>
        /// 获取条码
        /// </summary>
        /// <param name="str">条码内容</param>
        /// <param name="type">条码类型</param>
        /// <param name="white">条码宽度</param>
        /// <param name="height">条码高度</param>
        /// <returns></returns>
        public static Image GetBarCodeImage(string str,TYPE type,int white,int height)
        {
            var b = new Barcode()
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                IncludeLabel = true,
                Alignment = AlignmentPositions.CENTER,
                LabelPosition = LabelPositions.BOTTOMCENTER,
                ImageFormat = ImageFormat.Jpeg,
                Width = white,
                Height = height
            };
            return b.Encode(type, str);
        }

    }
}
