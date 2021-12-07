using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using FootprintViewer.Data;
using System.Threading;
using System.Linq;

namespace FootprintViewer.ViewModels
{
    public class FootprintInfo : ReactiveObject
    {
        private readonly Footprint _footprint;

        public FootprintInfo() { }

        public FootprintInfo(Footprint footprint)
        {
            _footprint = footprint;            
            Name = footprint.Name;
        }

        public Footprint Footprint => _footprint;

        [Reactive]
        public string? Name { get; set; }
    }

    public enum FootprintViewerContentType
    {    
        Show,
        Update
    }

    public class FootprintObserver : SidePanelTab
    {
        private readonly FootprintLayer _footrpintLayer;

        public FootprintObserver() { }

        public FootprintObserver(Map map)
        {
            FootprintInfos = new ObservableCollection<FootprintInfo>();

            _footrpintLayer = map.GetLayer<FootprintLayer>(LayerType.Footprint);

            Filter = new FootprintObserverFilter();

            this.WhenAnyValue(s => s.Type).Subscribe(type =>
            {
                if (type == FootprintViewerContentType.Update)
                {
                    FootprintsChanged();
                }
            });

            this.WhenAnyValue(s => s.IsActive).Subscribe(active =>
            {
                if (active == true)
                {                                           
                    Type = FootprintViewerContentType.Update;                   
                }
            });

            Type = FootprintViewerContentType.Update;
        }

        private static async Task<IList<Footprint>> LoadDataAsync(FootprintLayer layer)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(500);
                return layer.GetFootprints().ToList();
            });
        }

        private async void FootprintsChanged()
        {
            var footprints = await LoadDataAsync(_footrpintLayer);

            FootprintInfos = new ObservableCollection<FootprintInfo>(footprints.Select(s => new FootprintInfo(s)));
            
            Type = FootprintViewerContentType.Show;
        }

        [Reactive]
        public FootprintObserverFilter Filter { get; set; }

        [Reactive]
        public FootprintViewerContentType Type { get; set; }

        [Reactive]
        public FootprintInfo? SelectedFootprintInfo { get; set; }

        [Reactive]
        public ObservableCollection<FootprintInfo> FootprintInfos { get; set; }
    }

    public class FootprintObserverDesigner : FootprintObserver
    {
        public FootprintObserverDesigner() : base()
        {
            Type = FootprintViewerContentType.Show;
        }
    }

    public class ObservableFootprintCollection : ObservableCollection<FootprintInfo> { }
}
