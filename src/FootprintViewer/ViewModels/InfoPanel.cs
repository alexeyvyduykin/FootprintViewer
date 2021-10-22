using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FootprintViewer.ViewModels
{
    public class InfoPanel : ReactiveObject
    {
        [Reactive]
        public ObservableCollection<InfoPanelItem> Items { get; set; }
    }

    public class InfoPanelDesigner : InfoPanel
    {
        public InfoPanelDesigner()
        {
            InfoPanelItem routeItem = new InfoPanelItem()
            {
                Title = "Route",
                Text = "Description",
                CommandTitle = "X",
            };

            InfoPanelItem aoiItem = new InfoPanelItem()
            {
                Title = "AOI",
                Text = "Description",
                CommandTitle = "X",
            };

            Items = new ObservableCollection<InfoPanelItem>(new[] { routeItem, aoiItem });
        }
    }
}
