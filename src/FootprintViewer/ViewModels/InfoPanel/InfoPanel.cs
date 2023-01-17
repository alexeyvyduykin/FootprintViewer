using DynamicData;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels;

public sealed class InfoPanel : ViewModelBase
{
    private readonly SourceList<InfoPanelItem> _panels = new();
    private readonly ReadOnlyObservableCollection<InfoPanelItem> _items;

    public InfoPanel()
    {
        _panels
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .DisposeMany()
            .Subscribe();
    }

    public ReadOnlyObservableCollection<InfoPanelItem> Panels => _items;

    public void Show(InfoPanelItem panel)
    {
        panel.Close.Subscribe(remove);

        void remove(InfoPanelItem panel)
        {
            _panels.Edit(innerList => innerList.Remove(panel));
        }

        _panels.Edit(innerList =>
        {
            for (int i = innerList.Count - 1; i >= 0; i--)
            {
                if (innerList[i].GetType() == panel.GetType())
                {
                    innerList.RemoveAt(i);
                }
            }

            innerList.Add(panel);
        });
    }

    public void CloseAll(Type type)
    {
        _panels.Edit(innerList =>
        {
            for (int i = innerList.Count - 1; i >= 0; i--)
            {
                if (innerList[i].GetType() == type)
                {
                    innerList.RemoveAt(i);
                }
            }
        });
    }
}
