using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class InfoPanel : ReactiveObject
    {
        private readonly List<InfoPanelItem> _items = new();
        private readonly ObservableAsPropertyHelper<List<InfoPanelItem>> _panels;

        public InfoPanel()
        {
            Loading = ReactiveCommand.CreateFromTask(LoadingAsync);

            _panels = Loading.ObserveOn(RxApp.MainThreadScheduler).ToProperty(this, x => x.Panels);
        }

        public ReactiveCommand<Unit, List<InfoPanelItem>> Loading { get; }

        private async Task<List<InfoPanelItem>> LoadingAsync() => await Task.Run(() => new List<InfoPanelItem>(_items));

        public List<InfoPanelItem> Panels => _panels.Value;

        public async void ShowAsync(InfoPanelItem panel)
        {
            await Task.Run(() =>
            {
                AddImpl(panel);

                Loading.Execute().Subscribe();
            });
        }

        public void Show(InfoPanelItem panel)
        {
            Task.Run(() => ShowAsync(panel));
        }

        public void CloseAll(Type type)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                if (_items[i].GetType() == type)
                {
                    _items.RemoveAt(i);
                }
            }

            Loading.Execute().Subscribe();
        }

        private void AddImpl(InfoPanelItem? panel)
        {
            if (panel != null)
            {
                for (int i = _items.Count - 1; i >= 0; i--)
                {
                    if (_items[i].GetType() == panel.GetType())
                    {
                        _items.RemoveAt(i);
                    }
                }

                panel.Close.Subscribe(Remove);

                _items.Add(panel);
            }
        }

        private void Remove(InfoPanelItem? panel)
        {
            if (panel != null)
            {
                _items.Remove(panel);

                Loading.Execute().Subscribe();
            }
        }
    }
}
