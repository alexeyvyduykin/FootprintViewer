using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FootprintViewer.ViewModels
{
    public class SidePanel : ReactiveObject
    {
        public SidePanel()
        {
            this.WhenAnyValue(s => s.SelectedTab).Subscribe(tab =>
            {
                if (tab != null)
                {
                    foreach (var item in Tabs)
                    {
                        item.IsActive = false;

                        if (item == tab)
                        {
                            item.IsActive = true;
                        }                    
                    }
                }
            });
        }

        [Reactive]
        public ObservableCollection<SidePanelTab> Tabs { get; set; } = new ObservableCollection<SidePanelTab>();

        [Reactive]
        public SidePanelTab? SelectedTab { get; set; }
    }
}
