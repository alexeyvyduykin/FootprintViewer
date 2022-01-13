using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;

namespace FootprintViewer.ViewModels
{
    public class ToolBar : ReactiveObject
    {
        public ToolBar()
        {
            this.WhenAnyValue(x => x.RouteDistance.IsActive).Subscribe(isActive =>
            {
                if (isActive == true)
                {
                    if (AOICollection != null)
                    {
                        AOICollection.IsActive = false;
                    }
                }
                else
                {
                    if (Edit != null)
                    {
                        Edit.Command?.Execute().Subscribe();
                    }
                }
            });

            this.WhenAnyValue(x => x.AOICollection.IsActive).Subscribe(isActive =>
            {
                if (isActive == true)
                {
                    if (RouteDistance != null)
                    {
                        RouteDistance.IsActive = false;
                    }
                }
                else
                {
                    if (Edit != null)
                    {
                        Edit.Command?.Execute().Subscribe();
                    }
                }
            });
        }

        public void ResetAllTools()
        {
            if (AOICollection.IsActive == true)
            {
                AOICollection.IsActive = false;
            }

            if (RouteDistance.IsActive == true)
            {
                RouteDistance.IsActive = false;
            }
        }

        [Reactive]
        public Tool ZoomIn { get; set; }

        [Reactive]
        public Tool ZoomOut { get; set; }

        [Reactive]
        public ToolCollection AOICollection { get; set; }

        [Reactive]
        public Tool RouteDistance { get; set; }

        [Reactive]
        public Tool Edit { get; set; }

        [Reactive]
        public Tool WorldMaps { get; set; }

        //TODO: replace from ToolManager
        [Reactive]
        public WorldMapSelector WorldMapSelector { get; set; }
    }
}
