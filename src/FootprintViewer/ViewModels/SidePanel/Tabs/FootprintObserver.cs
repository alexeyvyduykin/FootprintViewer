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

    public class FootprintViewer : SidePanelTab
    {
        private readonly FootprintLayer _footrpintLayer;

        public FootprintViewer() { }

        public FootprintViewer(Map map)
        {
            FootprintInfos = new ObservableCollection<FootprintInfo>();

            _footrpintLayer = map.GetLayer<FootprintLayer>(LayerType.Footprint);
        }

        [Reactive]
        public FootprintInfo? SelectedFootprintInfo { get; set; }

        [Reactive]
        public ObservableCollection<FootprintInfo> FootprintInfos { get; set; }
    }
}
