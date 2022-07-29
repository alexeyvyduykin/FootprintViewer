using Avalonia.Media.Imaging;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using SkiaSharp;
using System;
using System.IO;
using System.Reactive.Disposables;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class FootprintPreviewItemView : ReactiveUserControl<FootprintPreviewViewModel>
    {
        public FootprintPreviewItemView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel, vm => vm.Image, v => v.FootprintImageImage.Source, value => Convert(value)).DisposeWith(disposables);
            });
        }

        private static object Convert(SKImage? image)
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
    }
}
