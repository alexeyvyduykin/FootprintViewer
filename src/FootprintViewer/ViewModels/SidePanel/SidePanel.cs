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

            IsExpandedChanged = ReactiveCommand.Create<bool>(IsExpandedImpl);

            this.WhenAnyValue(s => s.IsExpanded).InvokeCommand(IsExpandedChanged);

            IsExpanded = true;
        }

        private void IsExpandedImpl(bool value)
        {
            foreach (var item in Tabs)
            {
                item.IsExpanded = value;
            }
        }

        private ReactiveCommand<bool, Unit> IsExpandedChanged { get; }

        [Reactive]
        public bool IsExpanded { get; set; }

        public List<SidePanelTab> Tabs { get; set; }

        [Reactive]
        public SidePanelTab? SelectedTab { get; set; }
    }
}
