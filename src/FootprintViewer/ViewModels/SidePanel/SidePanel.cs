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

    public class SidePanelDesigner : SidePanel
    {
        public SidePanelDesigner()
        {
            var tab1 = new SceneSearchDesigner()
            {
                Name = "Scene",
                Title = "Поиск сцены"
            };

            var tab2 = new SceneSearch()
            {
                Name = "Test1",
                Title = "Default test title1"
            };

            var tab3 = new SceneSearch()
            {
                Name = "Test2",
                Title = "Default test title2"
            };

            Tabs = new ObservableCollection<SidePanelTab>(new[] { tab1, tab2, tab3 });

            SelectedTab = Tabs.FirstOrDefault();
        }
    }
}
