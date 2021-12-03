using FootprintViewer.Layers;
using Mapsui;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Reactive;
using FootprintViewer.Data;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetInfo : ReactiveObject
    {
        public GroundTargetInfo() { }

        public GroundTargetInfo(GroundTarget groundTarget)
        {
            GroundTarget = groundTarget;
            Name = groundTarget.Name;
        }

        [Reactive]
        public string? Name { get; set; }

        [Reactive]
        public GroundTarget GroundTarget { get; set; }
    }


    public enum TargetViewerContentType
    {
        Empty,
        Show,
        Update
    }

    public class GroundTargetViewer : SidePanelTab
    {
        private readonly TargetLayer _targetLayer;

        public GroundTargetViewer() { }

        public GroundTargetViewer(Map map)
        {
            _targetLayer = map.GetLayer<TargetLayer>(LayerType.GroundTarget);       
        }

        [Reactive]
        public TargetViewerContentType Type { get; set; } = TargetViewerContentType.Empty;

        [Reactive]
        public ObservableCollection<GroundTargetInfo> GroundTargetInfos { get; protected set; }
    }

    public class GroundTargetViewerDesigner : GroundTargetViewer
    {
        public GroundTargetViewerDesigner() : base()
        {
            Type = TargetViewerContentType.Show;
        }
    }
}
