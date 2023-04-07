using Avalonia;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace FootprintViewer.Fluent.Services;

public static class CursorService
{
    public static Cursor GetGrabHandCursor()
    {
        var loader = AvaloniaLocator.Current.GetService<IAssetLoader>();
        var s = loader!.Open(new Uri("avares://FootprintViewer.Fluent/resources/GrabHand32.png"));
        var bitmap = new Bitmap(s);
        return new Cursor(bitmap, new PixelPoint(12, 12));
    }
}
