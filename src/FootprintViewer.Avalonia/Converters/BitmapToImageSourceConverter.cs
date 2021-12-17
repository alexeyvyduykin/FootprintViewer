using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace FootprintViewer.Avalonia.Converters
{
    public class ImageToBitmapSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (value is null || !(value is Image myImage))
            //{
            //    //ensure provided value is valid image.
            //    return (BitmapSource)Application.Current.FindResource("NotLoadingFootprintImage");
            //}

            var myImage = (System.Drawing.Image)value;
          
            if (myImage.Height > Int16.MaxValue || myImage.Width > Int16.MaxValue)
            {//GetHbitmap will fail if either dimension is larger than max short value.
             //Throwing here to reduce cpu and resource usage when error can be detected early.
                throw new ArgumentException($"Cannot convert System.Drawing.Image with either dimension greater than {Int16.MaxValue} to BitmapImage.\nProvided image's dimensions: {myImage.Width}x{myImage.Height}", nameof(value));
            }

            using var bitmap = new System.Drawing.Bitmap(myImage);

            Bitmap? avBitmap;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                avBitmap = new Bitmap(memory);
            }

            bitmap.Dispose();

            return avBitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
