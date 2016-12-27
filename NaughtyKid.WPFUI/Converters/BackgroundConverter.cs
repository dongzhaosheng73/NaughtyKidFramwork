using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NaughtyKid.WPFUI.Converters
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var imagesource = value as BitmapImage;

                var imageBrush = new ImageBrush
                {
                    ImageSource = imagesource != null
                        ? (BitmapImage) value
                        : new BitmapImage(new Uri(value.ToString(),
                            UriKind.RelativeOrAbsolute))
                };

                return imageBrush;
            }
            catch(Exception ex)
            {
                throw new Exception("BearChildrenWPFUI Error", ex);
            }                         
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
