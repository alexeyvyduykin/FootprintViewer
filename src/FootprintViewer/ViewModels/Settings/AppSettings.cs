using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels.Settings
{
    [DataContract]
    public class AppSettings : ReactiveObject
    {
        public AppSettings()
        {
            FootprintSources = new List<ISourceInfo>();

            GroundTargetSources = new List<ISourceInfo>();

            GroundStationSources = new List<ISourceInfo>();

            SatelliteSources = new List<ISourceInfo>();

            UserGeometrySources = new List<ISourceInfo>();

            FootprintPreviewGeometrySources = new List<ISourceInfo>();

            MapBackgroundSources = new List<ISourceInfo>();

            FootprintPreviewSources = new List<ISourceInfo>();
        }

        [DataMember]
        public string? LastOpenDirectory { get; set; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> FootprintSources { get; private set; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> GroundTargetSources { get; private set; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> GroundStationSources { get; private set; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> SatelliteSources { get; private set; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> UserGeometrySources { get; private set; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> FootprintPreviewGeometrySources { get; private set; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> MapBackgroundSources { get; private set; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> FootprintPreviewSources { get; private set; }
    }
}
