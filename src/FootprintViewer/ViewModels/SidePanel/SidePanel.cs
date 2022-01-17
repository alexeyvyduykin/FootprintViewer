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
        private readonly ObservableCollection<SidePanelTab> _tabs;

        public SidePanel(IEnumerable<SidePanelTab> tabs)
        {
            _tabs = new ObservableCollection<SidePanelTab>(tabs);

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

            SelectedTab = _tabs.FirstOrDefault();
        }

        public ObservableCollection<SidePanelTab> Tabs => _tabs;

        [Reactive]
        public SidePanelTab? SelectedTab { get; set; }
    }
}
