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
            Tabs = new List<SidePanelTab>();

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

            SelectedTab = Tabs.FirstOrDefault();
        }

        public List<SidePanelTab> Tabs { get; set; }

        [Reactive]
        public SidePanelTab? SelectedTab { get; set; }
    }
}
