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
        private readonly InfoPanelItem?[] _list = new InfoPanelItem?[2] { null, null };

        public InfoPanel()
        {
            Items = new ObservableCollection<InfoPanelItem>();
        }

        public void OpenAOI(string description, Action closing)
        {
            InfoPanelItem aoiItem = new InfoPanelItem()
            {
                Title = "AOI",
                Text = description,
                CommandTitle = "X",
                Command = ReactiveCommand.Create<InfoPanelItem>((item) => 
                {
                    Remove(item);
                    closing.Invoke();
                })
            };

            SetItem(0, aoiItem);
        }

        public void CloseAOI()
        {
            var t = _list[0];
            
            if (t != null)
            {
                Items.Remove(t);
                _list[0] = null;              
            }
        }

        public void OpenRoute(string description, Action closing)
        {
            InfoPanelItem routeItem = new InfoPanelItem()
            {
                Title = "Route",
                Text = description,
                CommandTitle = "X",
                Command = ReactiveCommand.Create<InfoPanelItem>((item) => 
                {
                    Remove(item);
                    closing.Invoke();
                })
            };

            SetItem(1, routeItem);
        }

        public void CloseRoute()
        {
            var t = _list[1];

            if (t != null)
            {
                Items.Remove(t);
                _list[1] = null;
            }
        }

        private void SetItem(int index, InfoPanelItem item)
        {
            var t = _list[index];

            if (t != null)
            {
                Items.Remove(t);
                _list[index] = item;
                Items.Insert(0, item);
            }
            else
            {
                _list[index] = item;
                Items.Insert(0, item);
            }
        }

        private void Remove(InfoPanelItem item)
        {
            if (Items.Contains(item) == true)
            {
                Items.Remove(item);

                if (_list[0] == item)
                {
                    _list[0] = null;
                }

                if (_list[1] == item)
                {
                    _list[1] = null;
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
