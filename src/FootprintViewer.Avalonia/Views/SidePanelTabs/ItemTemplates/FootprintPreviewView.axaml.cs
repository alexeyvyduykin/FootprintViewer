using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using FootprintViewer.ViewModels;
using ReactiveUI;
using Splat;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace FootprintViewer.Avalonia.Views.SidePanelTabs.ItemTemplates
{
    public partial class FootprintPreviewView : ReactiveUserControl<FootprintPreview>
    {
        private Border FootprintImageBorder => this.FindControl<Border>("FootprintImageBorder");

        private Image FootprintImageImage => this.FindControl<Image>("FootprintImageImage");
        
        private static SceneSearch? _sceneSearch;

        public FootprintPreviewView()
        {
            InitializeComponent();

            _enter = ReactiveCommand.Create(EnterImpl);

            _leave = ReactiveCommand.Create(LeaveImpl);

            this.WhenActivated(disposables =>
            {
                this.FootprintImageBorder.Events().PointerEnter.Select(args => Unit.Default).InvokeCommand(this, v => v._enter).DisposeWith(disposables);

                this.FootprintImageBorder.Events().PointerLeave.Select(args => Unit.Default).InvokeCommand(this, v => v._leave).DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Image, v => v.FootprintImageImage.Source, value => Convert(value)).DisposeWith(disposables);
            });
        }

        private readonly ReactiveCommand<Unit, Unit> _enter;

        private readonly ReactiveCommand<Unit, Unit> _leave;

        private void EnterImpl()
        {
            _sceneSearch ??= Locator.Current.GetExistingService<SceneSearch>();

            _sceneSearch.MouseOverEnter.Execute(ViewModel).Subscribe();
        }

        private void LeaveImpl()
        {
            _sceneSearch ??= Locator.Current.GetExistingService<SceneSearch>();

            _sceneSearch.MouseOverLeave.Execute().Subscribe();
        }

        private static object Convert(System.Drawing.Image? image)
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
          
            using var bitmap = new System.Drawing.Bitmap(image);

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

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}