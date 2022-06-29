namespace FootprintViewer.Settings
{
    public class SourceBuilderConfiguration
    {
        public string[] FootprintSourceBuilders { get; set; } = System.Array.Empty<string>();

        public string[] GroundTargetSourceBuilders { get; set; } = System.Array.Empty<string>();

        public string[] GroundStationSourceBuilders { get; set; } = System.Array.Empty<string>();

        public string[] SatelliteSourceBuilders { get; set; } = System.Array.Empty<string>();

        public string[] UserGeometrySourceBuilders { get; set; } = System.Array.Empty<string>();

        public string[] FootprintPreviewGeometrySourceBuilders { get; set; } = System.Array.Empty<string>();

        public string[] MapBackgroundSourceBuilders { get; set; } = System.Array.Empty<string>();

        public string[] FootprintPreviewSourceBuilders { get; set; } = System.Array.Empty<string>();
    }
}