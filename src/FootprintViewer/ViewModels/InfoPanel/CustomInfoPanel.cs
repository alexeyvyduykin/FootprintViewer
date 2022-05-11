namespace FootprintViewer.ViewModels
{
    public abstract class CustomInfoPanel : InfoPanelItem, ISelectorItem
    {
        public abstract string GetKey();

        public string? Text { get; set; }
    }

    public class AOIInfoPanel : CustomInfoPanel
    {
        public override string GetKey() => "AOI";
    }

    public class RouteInfoPanel : CustomInfoPanel
    {
        public override string GetKey() => "Route";
    }

    public class FootprintClickInfoPanel : CustomInfoPanel
    {
        private readonly FootprintInfo _footprintInfo;

        public FootprintClickInfoPanel(FootprintInfo footprintInfo)
        {
            _footprintInfo = footprintInfo;
        }

        public FootprintInfo Info => _footprintInfo;

        public override string GetKey() => "FootprintClick";
    }

    public class GroundTargetClickInfoPanel : CustomInfoPanel
    {
        public override string GetKey() => "GroundTargetClick";
    }

    public class UserGeometryClickInfoPanel : CustomInfoPanel
    {
        public override string GetKey() => "UserGeometryClick";
    }
}
