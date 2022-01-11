namespace FootprintViewer.ViewModels
{
    public abstract class CustomInfoPanel : InfoPanelItem
    {
        public virtual string Title => nameof(CustomInfoPanel);

        public string? Text { get; set; }
    }

    public class AOIInfoPanel : CustomInfoPanel
    {
        public override string Title => "AOI";
    }

    public class RouteInfoPanel : CustomInfoPanel
    {
        public override string Title => "Route";
    }
}
