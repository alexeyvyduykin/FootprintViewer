using DynamicData;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace FootprintViewer.UI.ViewModels.InfoPanel;

public sealed class InfoPanelViewModel : ViewModelBase
{
    private readonly SourceList<InfoPanelItemViewModel> _panels = new();
    private readonly ReadOnlyObservableCollection<InfoPanelItemViewModel> _items;

    public InfoPanelViewModel()
    {
        _panels
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _items)
            .DisposeMany()
            .Subscribe();
    }

    public ReadOnlyObservableCollection<InfoPanelItemViewModel> Panels => _items;

    public void Show(InfoPanelItemViewModel panel)
    {
        panel.Close.Subscribe(remove);

        void remove(InfoPanelItemViewModel panel)
        {
            _panels.Edit(innerList => innerList.Remove(panel));
        }

        _panels.Edit(innerList =>
        {
            for (int i = innerList.Count - 1; i >= 0; i--)
            {
                if (string.Equals(innerList[i].Key, panel.Key))
                {
                    innerList.RemoveAt(i);
                }
            }

            innerList.Add(panel);
        });
    }

    public void CloseAll(string key)
    {
        _panels.Edit(innerList =>
        {
            for (int i = innerList.Count - 1; i >= 0; i--)
            {
                if (string.Equals(innerList[i].Key, key))
                {
                    innerList.RemoveAt(i);
                }
            }
        });
    }
}
