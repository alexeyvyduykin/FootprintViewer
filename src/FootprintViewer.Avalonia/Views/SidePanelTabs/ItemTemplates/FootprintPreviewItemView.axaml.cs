using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using SkiaSharp;
using Splat;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class FootprintPreviewItemView : ReactiveUserControl<FootprintPreviewViewModel>
    {
        private static FootprintPreviewTab? _footprintPreviewTab;

        public FootprintPreviewItemView()
        {
            InitializeComponent();

            _enter = ReactiveCommand.Create(EnterImpl);

            _leave = ReactiveCommand.Create(LeaveImpl);

            this.WhenActivated(disposables =>
            {
                FootprintImageBorder.Events().PointerEnter.Select(args => Unit.Default).InvokeCommand(this, v => v._enter).DisposeWith(disposables);

                FootprintImageBorder.Events().PointerLeave.Select(args => Unit.Default).InvokeCommand(this, v => v._leave).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Image, v => v.FootprintImageImage.Source, value => Convert(value)).DisposeWith(disposables);
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _enter;

        private readonly ReactiveCommand<Unit, Unit> _leave;

        private void EnterImpl()
        {
            _footprintPreviewTab ??= Locator.Current.GetExistingService<FootprintPreviewTab>();

            _footprintPreviewTab.ViewerList.MouseOverEnter.Execute(ViewModel!).Subscribe();
        }

        private void LeaveImpl()
        {
            _footprintPreviewTab ??= Locator.Current.GetExistingService<FootprintPreviewTab>();

            _footprintPreviewTab.ViewerList.MouseOverLeave.Execute().Subscribe();
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
