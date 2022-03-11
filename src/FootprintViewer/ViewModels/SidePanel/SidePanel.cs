using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

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

            IsCompactChanged = ReactiveCommand.Create<bool>(IsCompactImpl);

            this.WhenAnyValue(s => s.IsCompact).InvokeCommand(IsCompactChanged);
        }

        private void IsCompactImpl(bool value)
        {
            foreach (var item in Tabs)
            {
                item.IsCompact = value;
            }
        }

        private ReactiveCommand<bool, Unit> IsCompactChanged { get; }

        [Reactive]
        public bool IsCompact { get; set; }

        public List<SidePanelTab> Tabs { get; set; }

        [Reactive]
        public SidePanelTab? SelectedTab { get; set; }
    }
}
