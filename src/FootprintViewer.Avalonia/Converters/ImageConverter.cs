using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.Globalization;
using System.IO;

namespace FootprintViewer.Avalonia.Converters;

public class ImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SKImage image)
        {
            //if (value is null || !(value is Image myImage))
            //{
            //    //ensure provided value is valid image.
            //    return (BitmapSource)Application.Current.FindResource("NotLoadingFootprintImage");
            //}

            if (image == null)
            {
                throw new Exception();
            }

            if (image.Height > Int16.MaxValue || image.Width > Int16.MaxValue)
            {
                //GetHbitmap will fail if either dimension is larger than max short value.             
                //Throwing here to reduce cpu and resource usage when error can be detected early.
                throw new ArgumentException($"Cannot convert System.Drawing.Image with either dimension greater than {Int16.MaxValue} to BitmapImage.\nProvided image's dimensions: {image.Width}x{image.Height}", nameof(image));
            }

            using var bitmap = SKBitmap.FromImage(image);

            Bitmap? avBitmap;

            using (var memory = new MemoryStream())
            {
                SKData d = SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100);
                d.SaveTo(memory);
                memory.Position = 0;
                avBitmap = new Bitmap(memory);
            }

            bitmap.Dispose();

            return avBitmap;
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
