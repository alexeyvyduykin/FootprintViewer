using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace FootprintViewer.UI.Services2;

public static class CursorService
{
    public static Cursor GetGrabHandCursor()
    {
        //var loader = AvaloniaLocator.Current.GetService<IAssetLoader>();       
        var s = AssetLoader.Open(new Uri("avares://FootprintViewer.UI/resources/GrabHand32.png"));
        var bitmap = new Bitmap(s);
        return new Cursor(bitmap, new PixelPoint(12, 12));
    }
}
