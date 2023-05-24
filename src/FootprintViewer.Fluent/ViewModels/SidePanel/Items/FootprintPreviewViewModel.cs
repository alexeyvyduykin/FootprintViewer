using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;
using SkiaSharp;

namespace FootprintViewer.Fluent.ViewModels.SidePanel.Items;

public sealed class FootprintPreviewViewModel : ViewModelBase, IViewerItem
{
    public FootprintPreviewViewModel() : this(FootprintPreviewBuilder.CreateRandom()) { }

    public FootprintPreviewViewModel(FootprintPreview model)
    {
        Name = model.Name!;
        Date = model.Date;
        SatelliteName = model.SatelliteName;
        SunElevation = model.SunElevation;
        CloudCoverFull = model.CloudCoverFull;
        TileNumber = model.TileNumber;
        Path = model.Path;
        Image = model.Image;
    }

    public string Name { get; }

    public string? Date { get; set; }

    public string? SatelliteName { get; set; }

    public double SunElevation { get; set; }

    public double CloudCoverFull { get; set; }

    public string? TileNumber { get; set; }

    public string? Path { get; set; }

    public SKImage? Image { get; set; }

    public bool IsShowInfo { get; set; }
}
