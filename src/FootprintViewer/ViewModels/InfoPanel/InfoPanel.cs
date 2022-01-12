using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class InfoPanel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<InfoPanelItem> _panels;
        private readonly SourceList<InfoPanelItem> _items = new SourceList<InfoPanelItem>();

        public InfoPanel()
        {
            _items.Connect().Reverse().ObserveOn(RxApp.MainThreadScheduler).Bind(out _panels).Subscribe();
        }

        public void Show(InfoPanelItem panel)
        {
            Type type = panel.GetType();

            CloseAll(type);

            AddPanel(panel);
        }

        public void CloseAll(Type type)
        {
            _items.Edit(innerList =>
            {
                for (int i = innerList.Count() - 1; i >= 0; i--)
                {
                    if (innerList[i].GetType() == type)
                    {
                        innerList.RemoveAt(i);
                    }
                }
            });
        }

        private void AddPanel(InfoPanelItem panel)
        {
            _items.Add(panel);

            panel.Close.Subscribe(RemovePanel);
        }

        private void RemovePanel(InfoPanelItem panel)
        {
            _items.Edit(innerList =>
            {
                if (innerList.Contains(panel) == true)
                {
                    innerList.Remove(panel);
                }
            });

            //if (_items.Items.Contains(panel) == true)
            //{
            //    _items.Remove(panel);
            //}
        }

        public ReadOnlyObservableCollection<InfoPanelItem> Panels => _panels;
    }
}
