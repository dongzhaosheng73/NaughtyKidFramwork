using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NaughtyKid.Error;
using NaughtyKid.MyTools;

namespace NaughtyKid.Extension
{
    public static class BitmapImageExtension
    {
        public static  BitmapImage LoadBitmapImage(this string path,int w,int h)
        {
            try
            {
                using (var ms = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var image = new BitmapImage();

                    image.BeginInit();
                    if (w > 0 & h > 0)
                    {
                        image.DecodePixelHeight = h;
                        image.DecodePixelWidth = w;
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
    }
}
