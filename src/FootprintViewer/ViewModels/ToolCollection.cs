using DynamicData;
using Mapsui;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class ToolCollection : ReactiveObject
    {
        private readonly IEnumerable<Tool> _items;
        private Tool _first;

        public ToolCollection(IEnumerable<Tool> items)
        {
            _items = items;
            _first = items.FirstOrDefault();

            // TODO: to ReactiveUI/DynamicData
            Items = new ObservableCollection<Tool>();
            Items.CollectionChanged += MyItemsSource_CollectionChanged;

            this.WhenAnyValue(s => s.Visible).Subscribe(_ => Invalidate());

            this.WhenAnyValue(s => s.IsActive).Subscribe(isActive => 
            {
                _first.IsActive = IsActive;
            });

            Invalidate();
        }

        private void Invalidate()
        {
            if (Visible == true)
            {
                foreach (var item in _items)
                {
                    Items.Remove(item);
                }

                Items.Add(_first);

                foreach (var item in _items)
                {
                    if (item != _first)
                    {
                        Items.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in _items)
                {
                    Items.Remove(item);
                }

                Items.Add(_first);
            }
        }

        //private Dictionary<Tool, int> _dict = new Dictionary<Tool, int>();

        private void MyItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item != null && item is Tool tool)
                    {
                        tool.PropertyChanged += MyType_PropertyChanged;

                        //if(_dict.ContainsKey(tool) == false)
                        //{
                        //    _dict.Add(tool, 0);
                        //}

                        //_dict[tool]++;

                        //Debug.WriteLine($"Tool:{tool.Title} Count:{_dict[tool]}");
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item != null && item is Tool tool)
                    {
                        tool.PropertyChanged -= MyType_PropertyChanged;

                        //_dict[tool]--;

                        //Debug.WriteLine($"Tool:{tool.Title} Count:{_dict[tool]}");
                    }
                }
            }
        }

        private void MyType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Tool.IsActive))
            {
                if (sender is Tool tool && tool.IsActive == true)
                {
                    _first = tool;

                    foreach (var item in _items)
                    {
                        if (item != tool)
                        {
                            item.IsActive = false;
                        }
                    }

                    Invalidate();

                    IsActive = true;

                    Visible = false;
                }
                else
                {
                    IsActive = false;
                }
            }
        }

        [Reactive]
        public bool IsActive { get; set; }

        [Reactive]
        public bool Visible { get; set; }

        [Reactive]
        public ObservableCollection<Tool> Items { get; set; }
    }
}
