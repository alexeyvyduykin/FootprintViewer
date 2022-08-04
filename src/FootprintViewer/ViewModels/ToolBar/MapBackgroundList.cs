using DynamicData;
using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class MapBackgroundList : ReactiveObject
    {
        private readonly SourceList<MapResource> _mapResources = new();
        private readonly ReadOnlyObservableCollection<MapResource> _items;

        public MapBackgroundList()
        {
            WorldMapChanged = ReactiveCommand.Create<MapResource, MapResource>(s => s);

            _mapResources
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe();

            this.WhenAnyValue(x => x.SelectedMapBackground!).InvokeCommand(WorldMapChanged);
        }

        public void Update(IEnumerable<MapResource> maps)
        {
            _mapResources.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(maps);
            });
        }

        public ReactiveCommand<MapResource, MapResource> WorldMapChanged { get; }

        public ReadOnlyObservableCollection<MapResource> MapBackgrounds => _items;

        [Reactive]
        public MapResource? SelectedMapBackground { get; set; }
    }
}
