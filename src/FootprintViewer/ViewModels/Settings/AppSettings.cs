using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Reactive;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    [DataContract]
    public class AppSettings : SidePanelTab//ReactiveObject
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

            Title = "Пользовательские настройки";

            RemoveFootprintSource = ReactiveCommand.Create<ISourceInfo>(RemoveFootprintSourceImpl);
            RemoveGroundTargetSource = ReactiveCommand.Create<ISourceInfo>(RemoveGroundTargetSourceImpl);
            RemoveGroundStationSource = ReactiveCommand.Create<ISourceInfo>(RemoveGroundStationSourceImpl);
            RemoveSatelliteSource = ReactiveCommand.Create<ISourceInfo>(RemoveSatelliteSourceImpl);
            RemoveUserGeometrySource = ReactiveCommand.Create<ISourceInfo>(RemoveUserGeometrySourceImpl);
            RemoveFootprintPreviewGeometrySource = ReactiveCommand.Create<ISourceInfo>(RemoveFootprintPreviewGeometrySourceImpl);
            RemoveMapBackgroundSource = ReactiveCommand.Create<ISourceInfo>(RemoveMapBackgroundSourceImpl);
            RemoveFootprintPreviewSource = ReactiveCommand.Create<ISourceInfo>(RemoveFootprintPreviewSourceImpl);
        }

        public ReactiveCommand<ISourceInfo, Unit> RemoveFootprintSource { get; }

        public ReactiveCommand<ISourceInfo, Unit> RemoveGroundTargetSource { get; }

        public ReactiveCommand<ISourceInfo, Unit> RemoveGroundStationSource { get; }

        public ReactiveCommand<ISourceInfo, Unit> RemoveSatelliteSource { get; }

        public ReactiveCommand<ISourceInfo, Unit> RemoveUserGeometrySource { get; }

        public ReactiveCommand<ISourceInfo, Unit> RemoveFootprintPreviewGeometrySource { get; }

        public ReactiveCommand<ISourceInfo, Unit> RemoveMapBackgroundSource { get; }

        public ReactiveCommand<ISourceInfo, Unit> RemoveFootprintPreviewSource { get; }

        private void RemoveFootprintSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(FootprintSources);

            temp.Remove(item);

            FootprintSources = new List<ISourceInfo>(temp);
        }

        private void RemoveGroundTargetSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(GroundTargetSources);

            temp.Remove(item);

            GroundTargetSources = new List<ISourceInfo>(temp);
        }

        private void RemoveGroundStationSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(GroundStationSources);

            temp.Remove(item);

            GroundStationSources = new List<ISourceInfo>(temp);
        }

        private void RemoveSatelliteSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(SatelliteSources);

            temp.Remove(item);

            SatelliteSources = new List<ISourceInfo>(temp);
        }

        private void RemoveUserGeometrySourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(UserGeometrySources);

            temp.Remove(item);

            UserGeometrySources = new List<ISourceInfo>(temp);
        }

        private void RemoveFootprintPreviewGeometrySourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(FootprintPreviewGeometrySources);

            temp.Remove(item);

            FootprintPreviewGeometrySources = new List<ISourceInfo>(temp);
        }

        private void RemoveMapBackgroundSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(MapBackgroundSources);

            temp.Remove(item);

            MapBackgroundSources = new List<ISourceInfo>(temp);
        }

        private void RemoveFootprintPreviewSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(FootprintPreviewSources);

            temp.Remove(item);

            FootprintPreviewSources = new List<ISourceInfo>(temp);
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
