using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NaughtyKid.Extension;
using NaughtyKid.Error;
using NaughtyKid.Model;
using NaughtyKid.MyEnum;
using NaughtyKid.WinAPI;
using Color = System.Drawing.Color;
using Encoder = System.Drawing.Imaging.Encoder;
using FontFamily = System.Drawing.FontFamily;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using ExifLib;
using ExifTags = NaughtyKid.MyEnum.ExifTags;

namespace NaughtyKid.MyTools
{
  
    /// <summary>
    /// 图像相关帮助类
    /// </summary>
    public class ImageHepler 
    {      
        /// <summary>
        /// 旋转图像
        /// </summary>
        /// <param name="temp">要旋转的位图</param>
        /// <param name="angle">要旋转的角度</param>
        /// <returns>Bitmap</returns>
        public static Bitmap RotationImage(Bitmap temp, float angle) //位图旋转
        {

            angle =  angle % 360; //弧度转换

            var radian = angle * Math.PI / 180.0;
            var cos = Math.Cos(radian);
            var sin = Math.Sin(radian);

            var w = temp.Width;
            var h = temp.Height;
            var rw = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            var rh = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));


            var tempp = new Bitmap(rw, rh);

            var g = Graphics.FromImage(tempp);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            var offset = new Point((rw - w) / 2, (rh - h) / 2);

            var rect = new Rectangle(offset.X, offset.Y, w, h);

            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 + angle);
            g.TranslateTransform(-center.X, -center.Y);

            g.DrawImage(temp, rect);
            g.ResetTransform();
            g.Dispose();
            return tempp;

        }
        /// <summary>
        /// 附加初始值旋转
        /// </summary>
        /// <param name="file">图像文件</param>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        public static Bitmap AddIniValuesRotation(string file, int angle)
        {
            using (
                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite,
                        FileShare.ReadWrite))
            {
                var bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);

                var tempShowbit = (Bitmap)Image.FromStream(new MemoryStream(bytes));
                fileStream.Seek(0, SeekOrigin.Begin);
                try
                {
                    var exifread = new ExifReader(fileStream);

                    object angleType;

                    exifread.GetTagValue(ExifLib.ExifTags.Orientation, out angleType);

                    exifread.Dispose();

                    if (angleType != null)
                    {
                        switch (angleType.ToString())
                        {
                            case "6":
                                tempShowbit = RotationImage((Bitmap)tempShowbit.Clone(), 90);
                                break;
                            case "8":
                                tempShowbit = RotationImage((Bitmap)tempShowbit.Clone(), -90);
                                break;
                        }
                    }
                }
                catch
                {
                    
                }
                                      
                if (angle != 0) tempShowbit = RotationImage((Bitmap)tempShowbit.Clone(), angle);

                return tempShowbit;
            }
        }
       
        /// <summary>
        /// 在位图上添加文字
        /// </summary>
        /// <param name="bmp">原图</param>
        /// <param name="textdata">字符数据</param>
        /// <returns></returns>
        public static Bitmap BitmapAddText(Bitmap bmp, List<TextProperty> textdata)
        {
            var rBitmap = (Bitmap)bmp.Clone();

            var g = Graphics.FromImage(rBitmap);

            try
            {
                foreach (var text in textdata)
                {
                    var font = new Font(new FontFamily(text.FontName), text.FontSize);

                    g.DrawString(text.Text, font, new SolidBrush(Color.Black), text.DrawPoint);

                }
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;

            }
            finally
            {
                g.Dispose();
            }

            return rBitmap;
        }

        /// <summary>
        /// BitmapImage TO Bitmap
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource BitmapToBitmapImage(Bitmap bmp)
        {
            using (var source = bmp)
            {
                var ptr = source.GetHbitmap(); 

                var bs = Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

                    WinApiHepler.DeleteObject(ptr); 

                return bs;
            }
        }

        /// <summary>
        /// BitmapSource To Bitmap
        /// </summary>
        /// <param name="bs">BitmapSource</param>
        /// <returns>Bitmap</returns>
        public static Bitmap BitmapSourceToBitmap(BitmapSource bs)
        {
            var bmp = new Bitmap(bs.PixelWidth, bs.PixelHeight,PixelFormat.Format32bppPArgb);
            var data = bmp.LockBits(new Rectangle(Point.Empty, bmp.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            bs.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
            bmp.UnlockBits(data);
            return bmp;
        }

        /// <summary>
        /// BitmapImage To Bitmap
        /// </summary>
        /// <param name="bi">BitmapImage</param>
        /// <returns>Bitmap</returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bi)
        {
          
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bi));
                enc.Save(outStream);
                var bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        /// <summary>
        /// 蓝背景抠像
        /// </summary>
        /// <param name="low">最小值</param>
        /// <param name="hig">最大值</param>
        /// <param name="similarity">相似度</param>
        /// <param name="bmp">位图</param>
        /// <returns>bmp</returns>
        public static Bitmap BlueScreenMatting(int low, int hig, int similarity, Bitmap bmp)
        {
            bmp.MakeTransparent(Color.Blue);

            try
            {
                Rectangle ret = new Rectangle(0, 0, bmp.Width, bmp.Height);
                var vHeight = bmp.Height;
                var vWidth = bmp.Width;
                var bitmapData = bmp.LockBits(ret, ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                unsafe
                {
                    byte* pixels = (byte*)bitmapData.Scan0;

                    int nOffset = bitmapData.Stride - vWidth * 4;

                    for (int row = 0; row < vHeight; ++row)
                    {
                        for (int col = 0; col < vWidth; ++col)
                        {
                            var b = pixels[0];
                            var g = pixels[1];
                            var r = pixels[2];

                            int vAlpha = b - Math.Max(r, g);

                            if (vAlpha < 0) vAlpha = 0;

                            if ((vAlpha <= hig) && (vAlpha >= low))
                            {
                                vAlpha = 255;
                            }

                            if (vAlpha < similarity)
                                vAlpha = 0;

                            vAlpha = 255 - vAlpha;

                            if (b > Math.Max(r, g))
                            {
                                pixels[1] = Math.Max(r, g);

                            }

                            pixels[3] = (byte)vAlpha;

                            pixels += 4;
                        }
                        pixels += nOffset;
                    }
                }

                bmp.UnlockBits(bitmapData);

            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }

            return bmp;

        }

        /// <summary>
        /// 屏幕截图
        /// </summary>
        /// <param name="rect">截图范围</param>
        /// <returns>bitmap</returns>
        public static Bitmap CopyScreen(Rectangle rect)
        {
            var bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size);
                g.Dispose();
            }
            GC.Collect();
            return bitmap;
        }

        /// <summary>
        /// 无损压缩图像带图像旋转识别
        /// </summary>
        /// <param name="sFile">原图</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="tWidth">竖向压缩宽度</param>
        /// <param name="vWidth">竖向压缩宽度</param>
        /// <param name="flag">压缩质量</param>
        /// <param name="isDelect">是否删除</param>
        /// <returns></returns>
        public static bool CompressImageRotation(string sFile, string dFile, int tWidth, int vWidth, int flag,
            bool isDelect)
        {
            Bitmap ob;

            var iSource = AddIniValuesRotation(sFile, 0);

            Point tempXy;

            ImageStruct.FitSizeTable picsize;

            if (iSource.Width > iSource.Height)
            {
                double p = (double)iSource.Width / tWidth;
                int newtHeight = (int)Math.Round((iSource.Height / p), MidpointRounding.ToEven);
                picsize = FitSize(iSource.Width, iSource.Height, tWidth, newtHeight);
                tempXy = PointXy(picsize, tWidth, newtHeight);
                ob = new Bitmap(tWidth, newtHeight);
            }
            else
            {
                var p = (double)iSource.Width / vWidth;
                var newtHeight = (int)Math.Round((iSource.Height / p), MidpointRounding.ToEven);
                picsize = FitSize(iSource.Width, iSource.Height, vWidth, newtHeight);
                tempXy = PointXy(picsize, vWidth, newtHeight);
                ob = new Bitmap(vWidth, newtHeight);
            }

            var tFormat = iSource.RawFormat;

            //按比例缩放            
            var g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, tempXy.X, tempXy.Y,
                (int)Math.Round((iSource.Width * picsize.Fitsize), MidpointRounding.AwayFromZero),
                (int)Math.Round((iSource.Height * picsize.Fitsize), MidpointRounding.ToEven));
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            var ep = new EncoderParameters();
            var qy = new long[1];
            qy[0] = flag; //设置压缩的比例1-100
            var eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                var arrayIci = ImageCodecInfo.GetImageEncoders();
                var jpegIcIinfo = arrayIci.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
                if (jpegIcIinfo != null)
                {
                    ob.Save(dFile, jpegIcIinfo, ep);
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }

                try
                {
                    if (isDelect) File.Delete(sFile);
                }
                catch (Exception ex)
                {
                    ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
      
            }

            return false;
        }
        /// <summary>;
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="tWidth">横图宽度</param>
        /// <param name="vWidth">竖图宽度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="isDelect">是否删除原始照片</param>
        /// <returns>bool</returns>
        public static bool CompressImage(string sFile, string dFile, int tWidth, int vWidth, int flag, bool isDelect)
        {

            Bitmap ob;

            var fstream = new FileStream(sFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            var bytes = new byte[fstream.Length];

            fstream.Read(bytes, 0, bytes.Length);

            fstream.Close();

            var ms = new MemoryStream(bytes);

            var iSource = (Bitmap)Image.FromStream(ms, false, false);

            Point tempXy;

            ImageStruct.FitSizeTable picsize;

            if (iSource.Width > iSource.Height)
            {
                double p = (double)iSource.Width / tWidth;
                int newtHeight = (int)Math.Round((iSource.Height / p), MidpointRounding.ToEven);
                picsize = FitSize(iSource.Width, iSource.Height, tWidth, newtHeight);
                tempXy = PointXy(picsize, tWidth, newtHeight);
                ob = new Bitmap(tWidth, newtHeight);
            }
            else
            {
                var p = (double)iSource.Width / vWidth;
                var newtHeight = (int)Math.Round((iSource.Height / p), MidpointRounding.ToEven);
                picsize = FitSize(iSource.Width, iSource.Height, vWidth, newtHeight);
                tempXy = PointXy(picsize, vWidth, newtHeight);
                ob = new Bitmap(vWidth, newtHeight);
            }

            var tFormat = iSource.RawFormat;

            //按比例缩放            
            var g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, tempXy.X, tempXy.Y,
                (int)Math.Round((iSource.Width * picsize.Fitsize), MidpointRounding.AwayFromZero),
                (int)Math.Round((iSource.Height * picsize.Fitsize), MidpointRounding.ToEven));
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            var ep = new EncoderParameters();
            var qy = new long[1];
            qy[0] = flag; //设置压缩的比例1-100
            var eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                var arrayIci = ImageCodecInfo.GetImageEncoders();
                var jpegIcIinfo = arrayIci.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
                if (jpegIcIinfo != null)
                {
                    ob.Save(dFile, jpegIcIinfo, ep); 
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }

                try
                {
                    if (isDelect) File.Delete(sFile);
                }
                catch (Exception ex)
                {
                    ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
                ms.Dispose();
            }

        }

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">原图片</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="isDelect">是否删除原始照片</param>
        /// <returns></returns>
        public static bool CompressImage(string sFile, string dFile, int flag, bool isDelect)
        {
            var fstream = new FileStream(sFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

            var bytes = new byte[fstream.Length];

            fstream.Read(bytes, 0, bytes.Length);

            fstream.Close();

            var ms = new MemoryStream(bytes);

            var iSource = (Bitmap)Image.FromStream(ms, false, false);

            var tFormat = iSource.RawFormat;

            //以下代码为保存图片时，设置压缩质量
            var ep = new EncoderParameters();
            var qy = new long[1];
            qy[0] = flag; //设置压缩的比例1-100
            var eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                var arrayIci = ImageCodecInfo.GetImageEncoders();
                var jpegIcIinfo = arrayIci.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));
                if (jpegIcIinfo != null)
                {
                    iSource.Save(dFile, jpegIcIinfo, ep); 
                }
                else
                {
                    iSource.Save(dFile, tFormat);
                }

                try
                {
                    if (isDelect) File.Delete(sFile);
                }
                catch (Exception ex)
                {
                    ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }

                return true;
            }
            catch(Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }
            finally
            {
                iSource.Dispose();
                ms.Dispose();
            }

        }

        /// <summary>
        /// 获取适配比例
        /// </summary>
        /// <param name="srw">原图宽度</param>
        /// <param name="srh">原图高度</param>
        /// <param name="dsw">容器宽度</param>
        /// <param name="dsh">容器宽度</param>
        /// <returns>FitSizeTable</returns>
        public static ImageStruct.FitSizeTable FitSize(long srw, long srh, long dsw, long dsh) 
        {
            var srBl = (float)srh / srw;
            var dsBl = (float)dsh / dsw;

            ImageStruct.FitSizeTable dsetFitsize;

            if (srBl > dsBl)
            {
                dsetFitsize.Fith = dsh;


                dsetFitsize.Fitw = ((long)Math.Round(srw / (float)srh * dsh, MidpointRounding.AwayFromZero));
            }
            else
            {
                if (srBl < dsBl)
                {
                    dsetFitsize.Fitw = dsw;
                    dsetFitsize.Fith = ((long)Math.Round(srh / (float)srw * dsw, MidpointRounding.AwayFromZero));
                }
                else
                {
                    dsetFitsize.Fith = dsh;
                    dsetFitsize.Fitw = dsw;
                }
            }

            dsetFitsize.Fitsize = (float)Math.Round((float)dsetFitsize.Fith / srh, 5, MidpointRounding.AwayFromZero);

            return dsetFitsize;
        }

        /// <summary>
        /// 获取适配比例
        /// </summary>
        /// <param name="srw">原图宽度</param>
        /// <param name="srh">原图高度</param>
        /// <param name="dsw">容器宽度</param>
        /// <param name="dsh">容器宽度</param>
        /// <returns>FitSizeTable</returns>
        public static ImageStruct.FitSizeTableDouble FitSize(double srw, double srh, double dsw, double dsh) //适配原图像与容器
        {
            double srBl = srh / srw;
            double dsBl = dsh / dsw;

            ImageStruct.FitSizeTableDouble dsetFitsize;

            if (srBl > dsBl)
            {
                dsetFitsize.Dfith = dsh;


                dsetFitsize.Dfitw = (Math.Round(srw / srh * dsh, MidpointRounding.AwayFromZero));
            }
            else
            {
                if (srBl < dsBl)
                {
                    dsetFitsize.Dfitw = dsw;
                    dsetFitsize.Dfith = (Math.Round(srh / srw * dsw, MidpointRounding.AwayFromZero));
                }
                else
                {
                    dsetFitsize.Dfith = dsh;
                    dsetFitsize.Dfitw = dsw;
                }
            }

            dsetFitsize.Dfitsize = Math.Round(dsetFitsize.Dfith / srh, 5, MidpointRounding.AwayFromZero);
            return dsetFitsize;
        }

        /// <summary>
        /// 获取从外往里适配比例
        /// </summary>
        /// <param name="srw">原图宽度</param>
        /// <param name="srh">原图高度</param>
        /// <param name="dsw">容器宽度</param>
        /// <param name="dsh">容器宽度</param>
        /// <returns></returns>
        public static ImageStruct.FitSizeTable FitSizeOutSide(long srw, long srh, long dsw, long dsh)
        {
            var srBl = (float)srh / srw;
            var dsBl = (float)dsh / dsw;

            ImageStruct.FitSizeTable dsetFitsize;

            if (srBl < dsBl)
            {
                dsetFitsize.Fith = dsh;
                dsetFitsize.Fitw = ((long)Math.Round(srw / (float)srh * dsh));
            }
            else
            {
                if (srBl > dsBl)
                {
                    dsetFitsize.Fitw = dsw;
                    dsetFitsize.Fith = ((long)Math.Round(srh / (float)srw * dsw));
                }
                else
                {
                    dsetFitsize.Fith = dsh;
                    dsetFitsize.Fitw = dsw;
                }
            }

            dsetFitsize.Fitsize = (float)Math.Round((float)dsetFitsize.Fith / srh, 5);
            return dsetFitsize;
        }

        /// <summary>
        /// 获取从外往里适配比例
        /// </summary>
        /// <param name="srw">原图宽度</param>
        /// <param name="srh">原图高度</param>
        /// <param name="dsw">容器宽度</param>
        /// <param name="dsh">容器宽度</param>
        /// <returns></returns>
        public static ImageStruct.FitSizeTableDouble FitSizeOutSide(double srw, double srh, double dsw, double dsh)
        {
            double srBl = srh / srw;
            double dsBl = dsh / dsw;

            ImageStruct.FitSizeTableDouble dsetFitsize;

            if (srBl < dsBl)
            {
                dsetFitsize.Dfith = dsh;
                dsetFitsize.Dfitw = (Math.Round(srw / srh * dsh));
            }
            else
            {
                if (srBl > dsBl)
                {
                    dsetFitsize.Dfitw = dsw;
                    dsetFitsize.Dfith = (Math.Round(srh / srw * dsw));
                }
                else
                {
                    dsetFitsize.Dfith = dsh;
                    dsetFitsize.Dfitw = dsw;
                }
            }

            dsetFitsize.Dfitsize = Math.Round(dsetFitsize.Dfith / srh, 5);
            return dsetFitsize;
        }

        /// <summary>
        /// 获取适配后的坐标与大小
        /// </summary>
        /// <param name="w">要适配的宽度</param>
        /// <param name="h">要适配的高度</param>
        /// <param name="dw">目标宽度</param>
        /// <param name="dh">目标高度</param>
        /// <param name="outsize">是否裁切</param>
        /// <returns>适配结果</returns>
        public static ImageStruct.AdaptationSize GetAdaptationSize(double w, double h, double dw, double dh, bool outsize)
        {
            ImageStruct.AdaptationSize bitsize;

            ImageStruct.FitSizeTableDouble vPicsize;

            PointF vTempXy;

            if (outsize == false)
            {
                vPicsize = FitSize(w, h, dw, dh);

                vTempXy = PointXy(vPicsize, dw, dh);
            }
            else
            {
                vPicsize = FitSizeOutSide(w, h, dw, dh);

                vTempXy = PointXy(vPicsize, dw, dh);
            }

            bitsize.Size = vPicsize;
            bitsize.Point = vTempXy;

            return bitsize;

        }

        /// <summary>
        /// 绿背景抠像
        /// </summary>
        /// <param name="low">最小值</param>
        /// <param name="hig">最大值</param>
        /// <param name="similarity">相似度</param>
        /// <param name="bmp">位图</param>
        /// <returns></returns>
        public static Bitmap GreenScreenMatting(int low, int hig, int similarity, Bitmap bmp)
        {

            BitmapData bitmapData = null;
            try
            {
                bmp.MakeTransparent(Color.Green);
                Rectangle ret = new Rectangle(0, 0, bmp.Width, bmp.Height);
                var vHeight = bmp.Height;
                var vWidth = bmp.Width;
                bitmapData = bmp.LockBits(ret, ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                unsafe
                {

                    byte* pixels = (byte*)bitmapData.Scan0;

                    int nOffset = bitmapData.Stride - vWidth * 4;

                    for (int row = 0; row < vHeight; ++row)
                    {
                        for (int col = 0; col < vWidth; ++col)
                        {
                            var b = pixels[0];
                            var g = pixels[1];
                            var r = pixels[2];

                            int vAlpha = g - Math.Max(r, b);

                            if (vAlpha < 0) vAlpha = 0;

                            if ((vAlpha <= hig) && (vAlpha >= low))
                            {
                                vAlpha = 255;
                            }

                            if (vAlpha < similarity)
                                vAlpha = 0;

                            vAlpha = 255 - vAlpha;

                            if (g > Math.Max(r, b))
                            {
                                pixels[1] = Math.Max(r, b);

                            }

                            pixels[3] = (byte)vAlpha;

                            pixels += 4;
                        }
                        pixels += nOffset;
                    }
                }



            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            finally
            {
                if (bitmapData != null)
                {
                    bmp.UnlockBits(bitmapData);
                }
            }

            return bmp;
        }

        /// <summary>
        /// 将文本分行
        /// </summary>
        /// <param name="graphic">绘图图面</param>
        /// <param name="font">字体</param>
        /// <param name="text">文本</param>
        /// <param name="width">行宽</param>
        /// <returns>文本</returns>
        public static List<string> GetStringRows(Graphics graphic, Font font, string text)
        {
            int rowBeginIndex = 0;
            int textLength = text.Length;
            List<string> textRows = new List<string>();

            for (int index = 0; index < textLength; index++)
            {
                var rowEndIndex = index;

                if (index == textLength - 1)
                {
                    textRows.Add(text.Substring(rowBeginIndex));
                }
                else if (rowEndIndex + 1 < text.Length && text.Substring(rowEndIndex, 2) == "rn")
                {
                    textRows.Add(text.Substring(rowBeginIndex, rowEndIndex - rowBeginIndex));
                    rowEndIndex = index += 2;
                    rowBeginIndex = rowEndIndex;
                }
                else if (
                    graphic.MeasureString(text.Substring(rowBeginIndex, rowEndIndex - rowBeginIndex + 1), font)
                        .Width > graphic.VisibleClipBounds.Width)
                {
                    textRows.Add(text.Substring(rowBeginIndex, rowEndIndex - rowBeginIndex));
                    rowBeginIndex = rowEndIndex;
                }
            }
            return textRows;
        }

        /// <summary>
        /// 快速读取图像分辨率大小
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static System.Drawing.Size GetImageSize(FileInfo file)
        {

            using (var stm = file.OpenRead())
            {
                return GetImageSize(stm);
            }

        }

        /// <summary>
        /// 快速读取图像分辨率大小
        /// </summary>
        /// <param name="imageStream">img流</param>
        /// <returns>尺寸</returns>
        public static System.Drawing.Size GetImageSize(Stream imageStream)
        {
            System.Drawing.Size imgSize;
            var position = imageStream.Position;


            var bin = new byte[2];
            imageStream.Read(bin, 0, 2);
            imageStream.Seek(-2, SeekOrigin.Current);

            var fmt = bin[0] << 8 | bin[1];


            switch (fmt)
            {
                case 0xFFd8: //jpg
                    if (GetJpegSize(imageStream, out imgSize))
                        return imgSize;
                    break;
                case 0x8950: //png
                    if (GetPngSize(imageStream, out imgSize))
                        return imgSize;
                    break;
                case 0x4749: //gif
                    if (GetGifSize(imageStream, out imgSize))
                        return imgSize;
                    break;
                case 0x424D: //bmp
                    if (GetBmpSize(imageStream, out imgSize))
                        return imgSize;
                    break;
            }


            imageStream.Position = position;
            Image img = null;
            try
            {
                img = Image.FromStream(imageStream);
                return img.Size;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return new System.Drawing.Size();
            }
            finally
            {
                if (img != null)
                    img.Dispose();
            }
        }

        /// <summary>
        /// 快速读取jpg图像分辨率大小
        /// </summary>
        /// <param name="jpegStream">jpg流</param>
        /// <param name="size">尺寸</param>
        /// <returns>bool</returns>
        public static bool GetJpegSize(Stream jpegStream, out System.Drawing.Size size)
        {
            size = new System.Drawing.Size();
            var bin = new byte[2];
            jpegStream.Read(bin, 0, 2);

            if (bin[0] != 0xff && bin[1] != 0xd8) //SOI，Start of Image，图像开始
                return false;
            var dataLen = 0;
            while (jpegStream.CanRead)
            {
                int c = jpegStream.Read(bin, 0, 2);
                if (c != 2) //end of file;
                    break;
                if (bin[0] != 0xfF) //Error File!
                    break;
                int flag = bin[1];

                if (flag == 0xD9 || flag == 0xDA) //图像结整或图像数据开始
                    break;
                switch (flag)
                {
                    case 0xC0: //SOF0，Start of Frame，帧图像开始
                        jpegStream.Read(bin, 0, 2);
                        dataLen = bin[0] << 8 | bin[1];
                        dataLen -= 2;
                        byte[] data = new byte[dataLen];
                        jpegStream.Read(data, 0, dataLen);
                        size.Height = data[1] << 8 | data[2];
                        size.Width = data[3] << 8 | data[4];
                        return true; //无需读取其它数据
                    //case 0xD9://EOI，End of Image，图像结束 2字节
                    //case 0xDA://Start of Scan，扫描开始 12字节 图像数据，通常，到文件结束,遇到EOI标记
                    case 0xC4: //DHT，Difine Huffman Table，定义哈夫曼表
                    case 0xDD: // DRI，Define Restart Interval，定义差分编码累计复位的间隔
                    case 0xDB: // DQT，Define Quantization Table，定义量化表
                    case 0xE0: //APP0，Application，应用程序保留标记0。版本，DPI等信息
                    case 0xE1: //APPn，Application，应用程序保留标记n，其中n=1～15(任选)
                        jpegStream.Read(bin, 0, 2);
                        dataLen = bin[0] << 8 | bin[1];
                        dataLen -= 2;
                        break;
                    default:
                        if (flag > 0xE1 && flag < 0xEF) //APPx
                            goto case 0xE1;
                        //格式错误？？
                        break;
                }
                if (dataLen == 0) continue;
                jpegStream.Seek(dataLen, SeekOrigin.Current);
                dataLen = 0;
            }
            return !size.IsEmpty;
        }

        /// <summary>
        /// 快速读取png图像分辨率大小
        /// </summary>
        /// <param name="pngStm">png流</param>
        /// <param name="size">尺寸</param>
        /// <returns>bool</returns>
        public static bool GetPngSize(Stream pngStm, out System.Drawing.Size size)
        {
            size = new System.Drawing.Size();
            const uint pngHead = 0x89504e47;
            const uint pngHead2 = 0x0d0a1a0a; // PNG标识签名 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A;

            var bin = new byte[64];
            var c = pngStm.Read(bin, 0, 8);
            if (c != 8)
                return false;

            if ((uint)bin.Bytes2Int(0) != pngHead || (uint)bin.Bytes2Int(4) != pngHead2) //其它格式
                return false;

            while (pngStm.CanRead)
            {

                c = pngStm.Read(bin, 0, 8);
                if (c != 8)
                    return false;
                var dataLen = bin.Bytes2Int(0) + 4;
                var field = Encoding.ASCII.GetString(bin, 4, 4);
                switch (field)
                {
                    case "IHDR": //文件头数据块 
                        c = pngStm.Read(bin, 0, dataLen);
                        if (c != dataLen)
                            return false;
                        size.Width = bin.Bytes2Int(0);
                        size.Height = bin.Bytes2Int(4);
                        return true;
                }
                if (dataLen != 0)
                {
                    pngStm.Seek(dataLen, SeekOrigin.Current);
                }

            }
            return !size.IsEmpty;
        }

        /// <summary>
        /// 快速读取gif图像分辨率大小
        /// </summary>
        /// <param name="gifStm">gif流</param>
        /// <param name="size">尺寸</param>
        /// <returns>bool</returns>
        public static bool GetGifSize(Stream gifStm, out System.Drawing.Size size)
        {
            size = new System.Drawing.Size();

            var bin = new byte[32];
            var c = gifStm.Read(bin, 0, 32);
            if (c != 32)
                return false;

            if (bin[0] != 'G' || bin[1] != 'I' || bin[2] != 'F') //其它格式
                return false;

            size.Width = bin[6] | bin[7] << 8;
            size.Height = bin[8] | bin[9] << 8;

            return !size.IsEmpty;
        }

        /// <summary>
        /// 快速读取bmp图像分辨率大小
        /// </summary>
        /// <param name="bmpStm">bmp流</param>
        /// <param name="size">尺寸</param>
        /// <returns>bool</returns>
        public static bool GetBmpSize(Stream bmpStm, out System.Drawing.Size size)
        {
            size = new System.Drawing.Size();

            byte[] bin = new byte[32];
            int c = bmpStm.Read(bin, 0, 32);
            if (c != 32)
                return false;

            if (bin[0] != 'B' || bin[1] != 'M') //其它格式
                return false;

            size.Width = bin[18] | bin[19] << 8 | bin[20] << 16 | bin[21] << 24;
            size.Height = bin[22] | bin[23] << 8 | bin[24] << 16 | bin[25] << 24;

            return !size.IsEmpty;
        }

        /// <summary>
        /// 获取适配后的位图
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <param name="dsw">容器宽></param>
        /// <param name="dsh">容器高</param>
        /// <param name="outsize">是否裁切</param>
        /// <returns>Bitmap</returns>
        public static Bitmap GetFitBitmap(Bitmap bmp, int dsw, int dsh, bool outsize)
        {
            try
            {
                Point tempXy;

                var tempBitmap = (Bitmap)bmp.Clone();

                ImageStruct.FitSizeTable picsize;

                if (outsize == false)
                {
                    picsize = FitSize(tempBitmap.Width, tempBitmap.Height, dsw, dsh);

                    tempXy = PointXy(picsize, dsw, dsh);
                }
                else
                {
                    picsize = FitSizeOutSide(tempBitmap.Width, tempBitmap.Height, dsw, dsh);

                    tempXy = PointXy(picsize, dsw, dsh);
                }

                var rtBitmap = new Bitmap(dsw, dsh);

                rtBitmap.SetResolution(300, 300);

                var g = Graphics.FromImage(rtBitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;

                g.DrawImage(tempBitmap, tempXy.X, tempXy.Y,
                    (int)Math.Round((tempBitmap.Width * picsize.Fitsize), MidpointRounding.ToEven),
                    (int)Math.Round((tempBitmap.Height * picsize.Fitsize), MidpointRounding.ToEven));

                g.Dispose();

                tempBitmap.Dispose();

                return rtBitmap;

            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }

        }
        /// <summary>
        /// 获取JPG缩略图
        /// </summary>
        /// <param name="file">文件路径</param>
        /// <returns>BitmapSource</returns>
        public static BitmapSource GetThumBitmapSource(string file)
        {
            FileStream filestream = null;
            try
            {
                filestream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                var imagebytes = new byte[filestream.Length];

                filestream.Read(imagebytes, 0, imagebytes.Length);

                var decoder = BitmapDecoder.Create(new MemoryStream(imagebytes), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);

                var bitmapSource = decoder.Frames[0].Thumbnail;

                return bitmapSource;


            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                if (filestream != null) filestream.Close();
            }

        }

       
        /// <summary>
        /// 对比处理
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <param name="degree">对比度</param>
        /// <returns>Bitmap</returns>
        public static Bitmap KiContrast(Bitmap bmp, int degree)
        {
            if (bmp == null)
            {
                return null;
            }
            try
            {
                if (degree != 0)
                {
                    if (degree < -100) degree = -100;
                    if (degree > 100) degree = 100;
                    var contrast = (100.0 + degree) / 100.0;
                    contrast *= contrast;
                    var width = bmp.Width;
                    var height = bmp.Height;

                    var data = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                        PixelFormat.Format24bppRgb);
                    unsafe
                    {
                        var p = (byte*)data.Scan0;
                        var offset = data.Stride - width * 3;
                        for (var y = 0; y < height; y++)
                        {
                            for (int x = 0; x < width; x++)
                            {
                                // 处理指定位置像素的对比度
                                for (var i = 0; i < 3; i++)
                                {
                                    var pixel = ((p[i] / 255.0 - 0.5) * contrast + 0.5) * 255;
                                    if (pixel < 0) pixel = 0;
                                    if (pixel > 255) pixel = 255;
                                    p[i] = (byte)pixel;
                                } // i
                                p += 3;
                            } // x
                            p += offset;
                        } // y
                    }
                    bmp.UnlockBits(data);
                    return bmp;
                }
                return bmp;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
        }

        /// <summary>
        /// 锐化处理
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <param name="val">锐化度</param>
        /// <returns>Bitmap</returns>
        public static Bitmap KiSharpen(Bitmap bmp, float val)
        {
            if (bmp == null)
            {
                return null;
            }

            var w = bmp.Width;
            var h = bmp.Height;

            try
            {

                var bmpRtn = new Bitmap(w, h, bmp.PixelFormat);

                var srcData = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly,
                    PixelFormat.Format24bppRgb);
                var dstData = bmpRtn.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* pIn = (byte*)srcData.Scan0.ToPointer();
                    byte* pOut = (byte*)dstData.Scan0.ToPointer();
                    int stride = srcData.Stride;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            //取周围9点的值。位于边缘上的点不做改变。
                            if (x == 0 || x == w - 1 || y == 0 || y == h - 1)
                            {
                                //不做
                                pOut[0] = pIn[0];
                                pOut[1] = pIn[1];
                                pOut[2] = pIn[2];
                            }
                            else
                            {
                                //左上
                                var p = pIn - stride - 3;
                                int r1 = p[2];
                                int g1 = p[1];
                                int b1 = p[0];

                                //正上
                                p = pIn - stride;
                                int r2 = p[2];
                                int g2 = p[1];
                                int b2 = p[0];

                                //右上
                                p = pIn - stride + 3;
                                int r3 = p[2];
                                int g3 = p[1];
                                int b3 = p[0];

                                //左侧
                                p = pIn - 3;
                                int r4 = p[2];
                                int g4 = p[1];
                                int b4 = p[0];

                                //右侧
                                p = pIn + 3;
                                int r5 = p[2];
                                int g5 = p[1];
                                int b5 = p[0];

                                //右下
                                p = pIn + stride - 3;
                                int r6 = p[2];
                                int g6 = p[1];
                                int b6 = p[0];

                                //正下
                                p = pIn + stride;
                                int r7 = p[2];
                                int g7 = p[1];
                                int b7 = p[0];

                                //右下
                                p = pIn + stride + 3;
                                int r8 = p[2];
                                int g8 = p[1];
                                int b8 = p[0];

                                //自己
                                p = pIn;
                                int r0 = p[2];
                                int g0 = p[1];
                                int b0 = p[0];

                                var vR = r0 - (float)(r1 + r2 + r3 + r4 + r5 + r6 + r7 + r8) / 8;
                                var vG = g0 - (float)(g1 + g2 + g3 + g4 + g5 + g6 + g7 + g8) / 8;
                                var vB = b0 - (float)(b1 + b2 + b3 + b4 + b5 + b6 + b7 + b8) / 8;

                                vR = r0 + vR * val;
                                vG = g0 + vG * val;
                                vB = b0 + vB * val;

                                vR = vR > 0 ? Math.Min(255, vR) : Math.Max(0, vR);

                                vG = vG > 0 ? Math.Min(255, vG) : Math.Max(0, vG);

                                vB = vB > 0 ? Math.Min(255, vB) : Math.Max(0, vB);

                                pOut[0] = (byte)vB;
                                pOut[1] = (byte)vG;
                                pOut[2] = (byte)vR;

                            }

                            pIn += 3;
                            pOut += 3;
                        } // end of x

                        pIn += srcData.Stride - w * 3;
                        pOut += srcData.Stride - w * 3;
                    } // end of y
                }

                bmp.UnlockBits(srcData);
                bmpRtn.UnlockBits(dstData);

                return bmpRtn;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }

        }

        /// <summary>
        /// RGB颜色调整
        /// </summary>
        /// <param name="bmp">位图</param>
        /// <param name="rVal">R红色量</param>
        /// <param name="gVal">G绿色量</param>
        /// <param name="bVal">B蓝色量</param>
        /// <returns>Bitmap</returns>
        public static Bitmap KiColorBalance(Bitmap bmp, int rVal, int gVal, int bVal)
        {

            if (bmp == null)
            {
                return null;
            }


            int h = bmp.Height;
            int w = bmp.Width;

            try
            {
                if (rVal > 255 || rVal < -255 || gVal > 255 || gVal < -255 || bVal > 255 || bVal < -255)
                {
                    return null;
                }
                Bitmap temp = (Bitmap)bmp.Clone();

                BitmapData srcData = temp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite,
                    PixelFormat.Format24bppRgb);

                unsafe
                {
                    byte* p = (byte*)srcData.Scan0.ToPointer();

                    int nOffset = srcData.Stride - w * 3;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {

                            var b = p[0] + bVal;
                            if (bVal >= 0)
                            {
                                if (b > 255) b = 255;
                            }
                            else
                            {
                                if (b < 0) b = 0;
                            }

                            var g = p[1] + gVal;
                            if (gVal >= 0)
                            {
                                if (g > 255) g = 255;
                            }
                            else
                            {
                                if (g < 0) g = 0;
                            }

                            var r = p[2] + rVal;
                            if (rVal >= 0)
                            {
                                if (r > 255) r = 255;
                            }
                            else
                            {
                                if (r < 0) r = 0;
                            }

                            p[0] = (byte)b;
                            p[1] = (byte)g;
                            p[2] = (byte)r;

                            p += 3;
                        }

                        p += nOffset;


                    }
                } // end of unsafe

                temp.UnlockBits(srcData);

                return temp;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }

        }

        /// <summary>
        /// 亮度处理
        /// </summary>
        /// <param name="b">位图</param>
        /// <param name="degree">亮度</param>
        /// <returns>Bitmap</returns> 
        public static Bitmap KiLighten(Bitmap b, int degree)
        {
            const int bpp = 4;

            if (degree < -255) degree = -255;
            if (degree > 255) degree = 255;

            int width = b.Width;
            int height = b.Height;

            BitmapData data = b.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* p = (byte*)data.Scan0;
                int offset = data.Stride - width * bpp;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // 处理像素 B, G, R 亮度三分量
                        for (int i = 0; i < 3; i++)
                        {
                            var pixel = p[i] + degree;

                            if (pixel < 0) pixel = 0;
                            if (pixel > 255) pixel = 255;

                            p[i] = (byte)pixel;
                        } // i

                        p += bpp;
                    } // x
                    p += offset;
                } // y
            }

            b.UnlockBits(data);

            return b;
        }

        /// <summary>
        /// 图像加载
        /// </summary>
        /// <param name="path">加载路径</param>
        /// <param name="backcolor">背景颜色</param>
        /// <returns>位图</returns>
        public static Bitmap LoadBitmap(string path ,Color backcolor = default(Color))
        {

            MemoryStream ms = null;
            try
            {
                var fStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                var bytes = new byte[fStream.Length];

                fStream.Read(bytes, 0, bytes.Length);

                ms = new MemoryStream(bytes);
                fStream.Dispose();
                fStream.Close();

                var tempBitmap = (Bitmap) Image.FromStream(ms, false, false);

                var rBitmap = new Bitmap(tempBitmap.Width, tempBitmap.Height);

                var g = Graphics.FromImage(rBitmap);

                g.Clear(backcolor);

                g.DrawImage(tempBitmap, 0, 0, tempBitmap.Width, tempBitmap.Height);

                g.Dispose();

                tempBitmap.Dispose();

                return rBitmap;

            }
            catch (Exception ex)
            {

                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                ms = null;
                return null;
            }
            finally
            {
                if (ms != null) ms.Dispose();
            }
        }

        /// <summary>
        /// 图像加载
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">容器宽度</param>
        /// <param name="h">容器高度</param>
        /// <param name="isfit">释放裁切</param>
        /// <returns>位图内存流</returns>
        public static MemoryStream LoadBitmapStream(string path, int w, int h, bool isfit)
        {
            try
            {
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
                var ms = new MemoryStream(bytes);
                if (w > 0 & h > 0)
                {
                    var sBitmap = new Bitmap(ms);
                    var rBitmap = GetFitBitmap(sBitmap, w, h, isfit);
                    var rStream = new MemoryStream();
                    rBitmap.Save(rStream, sBitmap.RawFormat);
                    rBitmap.Dispose();
                    sBitmap.Dispose();
                    ms.Dispose();
                    rStream.Seek(0, SeekOrigin.Begin);
                    return rStream;
                }
                else
                {
                    return ms;
                }
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
               MemoryHepler.FlushMemory();
            }
        }

        /// <summary>
        /// 图像加载
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">容器宽度</param>
        /// <param name="h">容器高度</param>
        /// <param name="isfit">适配</param>
        /// <returns>位图</returns>
        public static  Bitmap LoadBitmap(string path, int w, int h, bool isfit)
        {
            try
            {
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
                var ms = new MemoryStream(bytes);
                if (w > 0 & h > 0)
                {
                    var sBitmap = new Bitmap(ms);
                    return  GetFitBitmap(sBitmap, w, h, isfit);                    
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                MemoryHepler.FlushMemory();
            }
        }

        /// <summary>
        /// 返回ImageBrush
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">显示宽度0为默认</param>
        /// <param name="h">显示高度0为默认</param>
        /// <returns></returns>
        public static ImageBrush LoadImageBrush(string path, int w, int h)
        {
            try
            {
                var b = new ImageBrush();
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
                var ms = new MemoryStream(bytes);
                var bit = new BitmapImage();
                bit.BeginInit();
                if (w > 0) bit.DecodePixelWidth = w;
                if (h > 0) bit.DecodePixelHeight = h;
                bit.CacheOption = BitmapCacheOption.None;
                bit.StreamSource = ms;
                bit.EndInit();
                b.ImageSource = bit;
                b.Stretch = Stretch.Uniform;
                return b;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
               MemoryHepler.FlushMemory();
            }
        }

        /// <summary>
        /// 图像加载ImageBrush
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">显示宽度0为默认</param>
        /// <param name="h">显示高度0为默认</param>
        /// <param name="stretch">适配方式</param>
        /// <returns>ImageBrush</returns>
        public static ImageBrush LoadImageBrush(string path, int w, int h, Stretch stretch)
        {
            try
            {
                var b = new ImageBrush();
                var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Dispose();
                fs.Close();
                var ms = new MemoryStream(bytes);
                var bit = new BitmapImage();
                bit.BeginInit();
                if (w > 0) bit.DecodePixelWidth = w;
                if (h > 0) bit.DecodePixelHeight = h;
                bit.CacheOption = BitmapCacheOption.None;
                bit.StreamSource = ms;
                bit.EndInit();
                b.ImageSource = bit;
                b.Stretch = stretch;
                return b;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                MemoryHepler.FlushMemory();
            }
        }

        /// <summary>
        /// ImageBrush加载带背景颜色
        /// </summary>
        /// <param name="path"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="stretch"></param>
        /// <param name="backColor"></param>
        /// <returns></returns>
        public static ImageBrush LoadImageBrushBackColor(string path, int w, int h, Stretch stretch,Color backColor = default(Color))
        {
            try
            {
                var b = new ImageBrush();
                var ms = new MemoryStream();
                LoadBitmap(path, backColor).Save(ms, ImageFormat.Png);
                var bit = new BitmapImage();
                bit.BeginInit();
                if (w > 0) bit.DecodePixelWidth = w;
                if (h > 0) bit.DecodePixelHeight = h;
                bit.CacheOption = BitmapCacheOption.None;
                bit.StreamSource = ms;
                bit.EndInit();
                b.ImageSource = bit;
                b.Stretch = stretch;
                return b;
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                MemoryHepler.FlushMemory();
            }
        }
 

        /// <summary>
        /// 返回BitmapImage
        /// </summary>
        /// <param name="path">图像路径</param>
        /// <param name="w">显示宽度0为默认</param>
        /// <param name="h">显示高度0为默认</param>
        /// <returns></returns>
        public static  BitmapImage LoadBitmapImage(string path, int w, int h)
        {

            try
            {
                using (var ms = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var image = new BitmapImage();

                    image.BeginInit();
                    if (w > 0)
                    {

                        image.DecodePixelWidth = w;
                    }
                    if (h > 0)
                    {
                        image.DecodePixelHeight = h;
                    }
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    return image;
                }

            }
            catch (Exception ex)
            {
                
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                MemoryHepler.FlushMemory();
            }
          
        }

        /// <summary>
        /// 加载BitmapImage
        /// </summary>
        /// <param name="path"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="isrotation">附加初始旋转</param>
        /// <returns></returns>
        public static BitmapImage LoadBitmapImage(string path, int w, int h, bool isrotation = false)
        {

            try
            {
                using (var ms = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    if (w > 0)
                    {
                        image.DecodePixelWidth = w;
                    }
                    if (h > 0)
                    {
                        image.DecodePixelHeight = h;
                    }
                    ExifReader exifread = null;
                    if (isrotation)
                    {
                        try
                        {
                            exifread = new ExifReader(ms);
                            object rotationvalue;
                            exifread.GetTagValue(ExifLib.ExifTags.Orientation, out rotationvalue);
                            if (rotationvalue != null)
                            {
                                switch (rotationvalue.ToString())
                                {
                                    case "6":
                                        image.Rotation = Rotation.Rotate90;
                                        break;
                                    case "8":
                                        image.Rotation = Rotation.Rotate270;
                                        break;
                                }
                            }

                        }
                        catch
                        {

                        }
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    if (exifread != null) exifread.Dispose();
                    return image;
                }

            }
            catch (Exception ex)
            {

                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                MemoryHepler.FlushMemory();
            }

        }

        /// <summary>
        /// 为JPG添加Exif信息
        /// </summary>
        /// <param name="filename">jpg路径</param>
        /// <param name="newDescription">添加的exif</param>
        /// <returns></returns>
        public static bool ModifyDescriptionInImage(string filename, List<ExifTagsData> newDescription)
        {
            try
            {
                var pic = Image.FromFile(filename);

                foreach (var t in pic.PropertyItems)
                {
                    var md = newDescription.Find(x => x.Id == t.Id);
                    if (md == null || string.IsNullOrWhiteSpace(md.Value)) continue;
                    var pi = t;


                    switch (pi.Type)
                    {
                        case 1:
                        case 2:
                            var getbytes = Encoding.UTF8.GetBytes(md.Value);

                            var bDescriptionAscii = new byte[getbytes.Length + 1];

                            for (int i = 0; i < getbytes.Length; i++)
                            {
                                bDescriptionAscii[i] = getbytes[i];
                            }
                            pi.Value = bDescriptionAscii;
                            pi.Len = bDescriptionAscii.Length;
                            pic.SetPropertyItem(pi);
                            break;
                        case 3:
                            var bDescription16 = BitConverter.GetBytes(ushort.Parse(md.Value));
                            pi.Value = bDescription16;
                            pi.Len = bDescription16.Length;
                            pic.SetPropertyItem(pi);
                            break;
                        case 4:
                            var bDescriptionU32 = BitConverter.GetBytes(UInt32.Parse(md.Value));
                            pi.Value = bDescriptionU32;
                            pi.Len = bDescriptionU32.Length;
                            pic.SetPropertyItem(pi);
                            break;
                        case 5:
                        case 10:
                            try
                            {
                                if (md.Value == "0") break;
                                var tb = decimal.Parse(md.Value).XXtoBl();
                                var n = BitConverter.GetBytes(UInt64.Parse(tb[0]));
                                var b = BitConverter.GetBytes(UInt64.Parse(tb[1]));

                                byte[] nb = new byte[8];
                                for (int i = 0; i < 4; i++)
                                {
                                    nb[i] = n[i];
                                }
                                for (int j = 0; j < 4; j++)
                                {
                                    int index = 4 + j;
                                    nb[index] = b[j];
                                }
                                pi.Value = nb;
                                pi.Len = nb.Length;
                                pic.SetPropertyItem(pi);
                            }
                            catch (Exception ex)
                            {
                                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                                return false;
                            }
                            break;
                    }
                }
                var tempfile = filename + ".temp";

                pic.Save(tempfile);

                pic.Dispose();

                File.Copy(tempfile, filename, true);

                File.Delete(tempfile);

                return true;

            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return false;
            }

        }

        /// <summary>
        /// 获取适配坐标
        /// </summary>
        /// <param name="picsize">适配比例</param>
        /// <param name="w">容器宽度</param>
        /// <param name="h">容器高度</param>
        /// <returns>TempXY</returns>
        public static Point PointXy(ImageStruct.FitSizeTable picsize, int w, int h)
        {

            var tempXy = new Point();

            if (w > picsize.Fitw)
            {
                tempXy.X = (int)((w - picsize.Fitw) / 2);
            }
            else if (w < picsize.Fitw)
            {
                tempXy.X = (int)((w - picsize.Fitw) / 2);
            }
            else
            {
                tempXy.X = 0;
            }

            if (h > picsize.Fith)
            {
                tempXy.Y = (int)((h - picsize.Fith) / 2);

            }
            else if (h < picsize.Fith)
            {
                tempXy.Y = (int)((h - picsize.Fith) / 2);
            }
            else
            {
                tempXy.Y = 0;
            }

            return tempXy;

        }

        /// <summary>
        /// 获取适配坐标
        /// </summary>
        /// <param name="picsize">适配比例</param>
        /// <param name="w">容器宽度</param>
        /// <param name="h">容器高度</param>
        /// <returns>TempXY</returns>
        public static PointF PointXy(ImageStruct.FitSizeTableDouble picsize, double w, double h)
        {
            PointF dxy = new PointF();

            if (w > picsize.Dfitw)
            {
                dxy.X = (float)((w - picsize.Dfitw) / 2);
            }
            else if (w < picsize.Dfitw)
            {
                dxy.X = (float)((w - picsize.Dfitw) / 2);
            }
            else
            {
                dxy.X = 0;
            }

            if (h > picsize.Dfith)
            {
                dxy.Y = (float)((h - picsize.Dfith) / 2);

            }
            else if (h < picsize.Dfith)
            {
                dxy.Y = (float)((h - picsize.Dfith) / 2);
            }
            else
            {
                dxy.Y = 0;
            }

            return dxy;

        }

        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="bmp">合成原图</param>
        /// <param name="synpath">合成边路径</param>
        /// <param name="pt">原图是否压在边框上</param>
        /// <returns>bitmap</returns>
        public static Bitmap Synthesis(Bitmap bmp, string synpath, bool pt)
        {
            try
            {
                var fs = new FileStream(synpath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                var filebytes = new byte[fs.Length];

                fs.Read(filebytes, 0, filebytes.Length);

                fs.Dispose();

                fs.Close();

                var backpicStream = new MemoryStream(filebytes);

                var tempBack = (Bitmap)new Bitmap(backpicStream).Clone();

                backpicStream.Close();

                return Synthesis(bmp, tempBack, 0, 0, pt,Color.Transparent);
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
           

        }

        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="obmp">合成原图</param>
        /// <param name="sbmp">边框</param>
        /// <param name="x">合成起点x坐标</param>
        /// <param name="y">合成起点y坐标</param>
        /// <param name="pt">原图是否压在边框上</param>
        /// <param name="backcolor">背景色</param>
        /// <returns>Bitmap</returns>
        public static Bitmap Synthesis(Bitmap obmp, Bitmap sbmp, int x, int y, bool pt,Color backcolor)
        {
            var b = new Bitmap(sbmp.Width, sbmp.Height);

            var g = Graphics.FromImage(b);
            g.Clear(backcolor);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.High;

            if (pt)
            {
                g.DrawImage(sbmp, 0, 0, sbmp.Width, sbmp.Height);
                g.DrawImage(obmp, x, y, obmp.Width, obmp.Height);
            }
            else
            {
                g.DrawImage(obmp, x, y, obmp.Width, obmp.Height);
                g.DrawImage(sbmp, 0, 0, sbmp.Width, sbmp.Height);
            }
          
            g.Dispose();
            obmp.Dispose();
            sbmp.Dispose();

            return b;
        }
        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="obmp">合成原图</param>
        /// <param name="sbmp">边框</param>
        /// <param name="x">合成起点x坐标</param>
        /// <param name="y">合成起点y坐标</param>
        /// <param name="pt">原图是否压在边框上</param>
        /// <returns></returns>
        public static Bitmap Synthesis(Bitmap obmp, Bitmap sbmp, int x, int y, bool pt)
        {
            var b = new Bitmap(sbmp.Width, sbmp.Height);

            var g = Graphics.FromImage(b);

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.High;

            if (pt)
            {
                g.DrawImage(sbmp, 0, 0, sbmp.Width, sbmp.Height);
                g.DrawImage(obmp, x, y, obmp.Width, obmp.Height);
            }
            else
            {
                g.DrawImage(obmp, x, y, obmp.Width, obmp.Height);
                g.DrawImage(sbmp, 0, 0, sbmp.Width, sbmp.Height);
            }

            g.Dispose();
            obmp.Dispose();
            sbmp.Dispose();

            return b;
        }

        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="obmp">合成原图</param>
        /// <param name="synpath">边框路径</param>
        /// <param name="x">合成起点x坐标</param>
        /// <param name="y">合成起点y坐标</param>
        /// <returns></returns>
        public static MemoryStream Synthesis(Bitmap obmp, string synpath, int x, int y)
        {

            Bitmap mpic = (Bitmap)obmp.Clone();

            MemoryStream backpicStream = new MemoryStream(File.ReadAllBytes(synpath));

            Bitmap backpic = new Bitmap(backpicStream);

            MemoryStream returnms = new MemoryStream();

            Bitmap b = new Bitmap(backpic.Width, backpic.Height);
            Graphics g = Graphics.FromImage(b);

            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.High;

            g.DrawImage(mpic, x, y, backpic.Width, backpic.Height);
            g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
            g.Dispose();
            mpic.Dispose();
            backpic.Dispose();
            backpicStream.Dispose();
            b.Save(returnms, ImageFormat.Png);
            b.Dispose();
            return returnms;
        }

        /// <summary>
        /// 合成图像
        /// </summary>
        /// <param name="obmp">合成原图</param>
        /// <param name="synpath">边框路径</param>
        /// <param name="x">合成起点x坐标</param>
        /// <param name="y">合成起点y坐标</param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public static Bitmap Synthesis(Bitmap obmp, string synpath, int x, int y, int x1, int y1)
        {
            Bitmap returnBitmap = null;

            MemoryStream backpicStream = null;
            try
            {
                var mpic = (Bitmap)obmp.Clone();

                var fStream = new FileStream(synpath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

                var bytes = new byte[fStream.Length];

                fStream.Read(bytes, 0, bytes.Length);

                fStream.Close();

                backpicStream = new MemoryStream(bytes);

                var backpic = new Bitmap(backpicStream);

                returnBitmap = new Bitmap(backpic.Width, backpic.Height);

                Graphics g = Graphics.FromImage(returnBitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;

                g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                g.DrawImage(mpic, x, y, x1 - x, y1 - y);
                g.Dispose();
                mpic.Dispose();
                backpic.Dispose();
                backpicStream.Dispose();

                return (Bitmap)returnBitmap.Clone();
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                if (returnBitmap != null) returnBitmap.Dispose();
                if (backpicStream != null) backpicStream.Close();
                if (obmp != null) obmp.Dispose();
            }
        }

        /// <summary>
        ///图像合成
        /// </summary>
        /// <param name="obmp">原图</param>
        /// <param name="sbmp">边框</param>
        /// <param name="x">起点坐标x</param>
        /// <param name="y">起点坐标y</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="pt">原图是否压在边框上</param>
        /// <returns></returns>
        public static Bitmap Synthesis(Bitmap obmp, Bitmap sbmp, double x, double y, double w, double h, bool pt)
        {
            Bitmap returnBitmap = null;

            try
            {
                var mpic = (Bitmap)obmp.Clone();

                var backpic = (Bitmap)sbmp.Clone();

                returnBitmap = new Bitmap(backpic.Width, backpic.Height);

                Graphics g = Graphics.FromImage(returnBitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;

                if (pt)
                {
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                }
                else
                {
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                }

                g.Dispose();
                mpic.Dispose();
                backpic.Dispose();
                return (Bitmap)returnBitmap.Clone();
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                if (returnBitmap != null) returnBitmap.Dispose();
                if (obmp != null) obmp.Dispose();
            }
        }

        ///  <summary>
        /// 图像合成
        ///  </summary>
        ///  <param name="obmp">原图</param>
        ///  <param name="sbmp">边框</param>
        ///  <param name="x">起点坐标x</param>
        ///  <param name="y">起点坐标y</param>
        ///  <param name="w">宽</param>
        ///  <param name="h">高</param>
        ///  <param name="pt">原图是否压在边框上</param>
        /// <param name="backcolor">背景色
        /// </param>
        /// <returns></returns>
        public static Bitmap Synthesis(Bitmap obmp, Bitmap sbmp, double x, double y, double w, double h, bool pt,Color backcolor)
        {
            Bitmap returnBitmap = null;

            try
            {
                var mpic = (Bitmap)obmp.Clone();

                var backpic = (Bitmap)sbmp.Clone();

                returnBitmap = new Bitmap(backpic.Width, backpic.Height);

                Graphics g = Graphics.FromImage(returnBitmap);
                g.Clear(backcolor);
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;

                if (pt)
                {
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                }
                else
                {
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                }

                g.Dispose();
                mpic.Dispose();
                backpic.Dispose();
                return (Bitmap)returnBitmap.Clone();
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                if (returnBitmap != null) returnBitmap.Dispose();
                if (obmp != null) obmp.Dispose();
            }
        }

        /// <summary>
        ///图像合成
        /// </summary>
        /// <param name="obmp">原图</param>
        /// <param name="sbmp">边框</param>
        /// <param name="x">起点坐标x</param>
        /// <param name="y">起点坐标y</param>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="pt">原图是否压在边框上</param>
        /// <returns></returns>
        public static  T Synthesis<T>(Bitmap obmp, Bitmap sbmp, double x, double y, double w, double h,
            bool pt) where T : BitmapSource
        {
            Bitmap returnBitmap = null;

            try
            {
                var mpic = (Bitmap)obmp.Clone();

                var backpic = (Bitmap)sbmp.Clone();

                returnBitmap = new Bitmap(backpic.Width, backpic.Height);

                var g = Graphics.FromImage(returnBitmap);

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.High;

                if (pt)
                {
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                }
                else
                {
                    g.DrawImage(mpic, (int)x, (int)y, (int)w, (int)h);
                    g.DrawImage(backpic, 0, 0, backpic.Width, backpic.Height);
                }

                g.Dispose();
                mpic.Dispose();
                backpic.Dispose();
               
                return (T) BitmapToBitmapImage((Bitmap)returnBitmap.Clone());
            }
            catch (Exception ex)
            {
                ErrorHelper.ErrorPutting(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                return null;
            }
            finally
            {
                if (returnBitmap != null) returnBitmap.Dispose();
                if (obmp != null) obmp.Dispose();
            }
        }

       /// <summary>
       /// 读取Jpg中Exif的缩略图
       /// </summary>
       /// <param name="imagePath"></param>
       /// <returns></returns>
        public static Image GetThumbImage(string imagePath)
        {
            const int GDI_ERR_PROP_NOT_FOUND = 19;    // Property not found error
            const int GDI_ERR_OUT_OF_MEMORY = 3;

            IntPtr hImage = IntPtr.Zero;
            IntPtr buffer = IntPtr.Zero;    // Holds the thumbnail data
            int ret;
            ret = WinApiHepler.GdipLoadImageFromFile(imagePath, out hImage);

            try
            {
                if (ret != 0)
                    throw createException(ret);

                int propSize;

                ret = WinApiHepler. GdipGetPropertyItemSize(hImage, WinApiHepler.THUMBNAIL_DATA, out propSize);
                // Image has no thumbnail data in it. Return null
                if (ret == GDI_ERR_PROP_NOT_FOUND)
                    return null;
                if (ret != 0)
                    throw createException(ret);


                // Allocate a buffer in memory
                buffer = Marshal.AllocHGlobal(propSize);
                if (buffer == IntPtr.Zero)
                    throw createException(GDI_ERR_OUT_OF_MEMORY);

                ret = WinApiHepler.GdipGetPropertyItem(hImage, WinApiHepler.THUMBNAIL_DATA, propSize, buffer);
                if (ret != 0)
                    throw createException(ret);

                // buffer has the thumbnail data. Now we have to convert it to
                // an Image
                return convertFromMemory(buffer);
            }

            finally
            {
                // Free the buffer
                if (buffer != IntPtr.Zero)
                    Marshal.FreeHGlobal(buffer);

                WinApiHepler.GdipDisposeImage(hImage);
            }
        }

        /// <summary>
        /// Generates an exception depending on the GDI+ error codes (I removed some error
        /// codes)
        /// </summary>
        private static Exception createException(int gdipErrorCode)
        {
            switch (gdipErrorCode)
            {
                case 1:
                    return new ExternalException("Gdiplus Generic Error", -2147467259);
                case 2:
                    return new ArgumentException("Gdiplus Invalid Parameter");
                case 3:
                    return new OutOfMemoryException("Gdiplus Out Of Memory");
                case 4:
                    return new InvalidOperationException("Gdiplus Object Busy");
                case 5:
                    return new OutOfMemoryException("Gdiplus Insufficient Buffer");
                case 7:
                    return new ExternalException("Gdiplus Generic Error", -2147467259);
                case 8:
                    return new InvalidOperationException("Gdiplus Wrong State");
                case 9:
                    return new ExternalException("Gdiplus Aborted", -2147467260);
                case 10:
                    return new FileNotFoundException("Gdiplus File Not Found");
                case 11:
                    return new OverflowException("Gdiplus Over flow");
                case 12:
                    return new ExternalException("Gdiplus Access Denied", -2147024891);
                case 13:
                    return new ArgumentException("Gdiplus Unknown Image Format");
                case 18:
                    return new ExternalException("Gdiplus Not Initialized", -2147467259);
                case 20:
                    return new ArgumentException("Gdiplus Property Not Supported Error");
            }

            return new ExternalException("Gdiplus Unknown Error", -2147418113);
        }
        /// <summary>
        /// Converts the IntPtr buffer to a property item and then converts its 
        /// value to a Drawing.Image item
        /// </summary>
        private static Image convertFromMemory(IntPtr thumbData)
        {
            propertyItemInternal prop =
                (propertyItemInternal)Marshal.PtrToStructure
                (thumbData, typeof(propertyItemInternal));

            // The image data is in the form of a byte array. Write all 
            // the bytes to a stream and create a new image from that stream
            byte[] imageBytes = prop.Value;
            MemoryStream stream = new MemoryStream(imageBytes.Length);
            stream.Write(imageBytes, 0, imageBytes.Length);

            return Image.FromStream(stream);
        }

        /// <summary>
        /// Used in Marshal.PtrToStructure().
        /// We need this dummy class because Imaging.PropertyItem is not a "blittable"
        /// class and Marshal.PtrToStructure only accepted blittable classes.
        /// (It's not blitable because it uses a byte[] array and that's not a blittable
        /// type. See MSDN for a definition of Blittable.)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private class propertyItemInternal
        {
            public int id = 0;
            public int len = 0;
            public short type = 0;
            public IntPtr value = IntPtr.Zero;

            public byte[] Value
            {
                get
                {
                    byte[] bytes = new byte[(uint)len];
                    Marshal.Copy(value, bytes, 0, len);
                    return bytes;
                }
            }
        }

    }

    
}
