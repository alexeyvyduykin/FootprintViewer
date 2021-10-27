using FootprintViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FootprintViewer.ViewModels
{
    public class InfoPanel : ReactiveObject
    {
        private readonly Dictionary<string, InfoPanelItem?> _dict = new Dictionary<string, InfoPanelItem?>();

        public InfoPanel()
        {
            Items = new ObservableCollection<InfoPanelItem>();
        }

        public void Open(string key, string description, Action closing)
        {
            InfoPanelItem item = new InfoPanelItem()
            {
                Title = key,
                Text = description,
                CommandTitle = "X",
                Command = ReactiveCommand.Create<InfoPanelItem>((item) => 
                {
                    Remove(item);
                    closing.Invoke();
                })
            };

            SetItem(key, item);
        }

        public void Close(string key)
        {
            if (_dict.ContainsKey(key) == false)
            {
                return;
            }

            var s = _dict[key];
            
            if (s != null)
            {
                Items.Remove(s);
                _dict[key] = null;              
            }
        }

        private void SetItem(string key, InfoPanelItem item)
        {
            if(_dict.ContainsKey(key) == false)
            {
                _dict.Add(key, null);              
            }

            var s = _dict[key];

            if (s != null)
            {
                Items.Remove(s);
                _dict[key] = item;
                Items.Insert(0, item);
            }
            else
            {
                _dict[key] = item;
                Items.Insert(0, item);
            }
        }

        private void Remove(InfoPanelItem item)
        {
            if (Items.Contains(item) == true)
            {
                Items.Remove(item);

                foreach (var key in _dict.Keys)
                {
                    if (_dict[key] == item)
                    {
                        _dict[key] = null;
                        break;
                    }
                }
            }
        }

        [Reactive]
        public ObservableCollection<InfoPanelItem> Items { get; set; }
    }

    public class InfoPanelDesigner : InfoPanel
    {
        public InfoPanelDesigner()
        {
            InfoPanelItem routeItem = new InfoPanelItem()
            {
                Title = "Route",
                Text = "Description",
                CommandTitle = "X",
            };

            InfoPanelItem aoiItem = new InfoPanelItem()
            {
                Title = "AOI",
                Text = "Description",
                CommandTitle = "X",
            };

            Items = new ObservableCollection<InfoPanelItem>(new[] { routeItem, aoiItem });
        }
    }
}
