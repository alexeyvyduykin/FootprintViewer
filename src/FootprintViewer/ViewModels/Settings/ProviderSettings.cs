using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Reactive;
using System.Runtime.Serialization;

namespace FootprintViewer.ViewModels
{
    public enum ProviderType
    {
        Footprints,
        GroundTargets,
        GroundStations,
        Satellites,
        UserGeometries,
        FootprintPreviewGeometries,
        MapBackgrounds,
        FootprintPreviews
    };

    [DataContract]
    public class ProviderSettings : ReactiveObject
    {
        public ProviderSettings()
        {
            Sources = new List<ISourceInfo>();

            AvailableSources = new List<ISourceBuilder>();

            RemoveSource = ReactiveCommand.Create<ISourceInfo>(RemoveSourceImpl);

            AddSource = ReactiveCommand.Create<ISourceInfo, ISourceInfo>(AddSourceImpl);
        }

        public ReactiveCommand<ISourceInfo, Unit> RemoveSource { get; }

        public ReactiveCommand<ISourceInfo, ISourceInfo> AddSource { get; }

        private void RemoveSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(Sources);

            temp.Remove(item);

            Sources = new List<ISourceInfo>(temp);
        }

        private ISourceInfo AddSourceImpl(ISourceInfo item)
        {
            var temp = new List<ISourceInfo>(Sources);

            temp.Add(item);

            Sources = new List<ISourceInfo>(temp);

            return item;
        }

        [DataMember]
        public ProviderType Type { get; init; }

        [DataMember]
        [Reactive]
        public List<ISourceInfo> Sources { get; private set; }

        [Reactive]
        public List<ISourceBuilder> AvailableSources { get; set; }
    }


}
