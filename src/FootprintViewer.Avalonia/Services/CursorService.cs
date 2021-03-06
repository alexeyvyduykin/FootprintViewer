using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;

namespace FootprintViewer.Avalonia.Services
{
    public static class CursorService
    {
        public static Cursor GetGrabHandCursor()
        {
            var loader = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var s = loader!.Open(new Uri("avares://FootprintViewer.Avalonia/resources/GrabHand32.png"));
            var bitmap = new Bitmap(s);
            return new Cursor(bitmap, new PixelPoint(12, 12));
        }
    }
}
