using FootprintViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class ToolBar : ReactiveObject
    {
        public ToolBar()
        {
            Tools = new ObservableCollection<IToolItem>();
        }

        public void AddTool(IToolItem tool)
        {
            if (tool is IToolCheck check)
            {
                check.Check.Subscribe(CheckChanged);
            }
            else if (tool is IToolCollection collection)
            {
                foreach (var item in collection.GetItems())
                {
                    item.Check.Subscribe(CheckChanged);
                }
            }

            Tools.Add(tool);
        }

        private void CheckChanged(IToolCheck tool)
        {
            if (tool.IsCheck == true)
            {
                UncheckDisinclude(tool);
            }
        }

        private void UncheckDisinclude(IToolCheck disinclude)
        {
            foreach (var item in Tools)
            {
                if (item is IToolCheck check && disinclude != check)
                {
                    if (string.IsNullOrEmpty(check.Group) == true || string.IsNullOrEmpty(disinclude.Group) == true)
                    {
                        continue;
                    }

                    if (check.Group.Equals(disinclude.Group) == false)
                    {
                        continue;
                    }

                    if (check.IsCheck == true)
                    {
                        check.Check.Execute(false).Subscribe();

                        return;
                    }
                }
                else if (item is IToolCollection collection)
                {
                    foreach (var toolCheck in collection.GetItems())
                    {
                        if (disinclude != toolCheck)
                        {
                            if (string.IsNullOrEmpty(toolCheck.Group) == true || string.IsNullOrEmpty(disinclude.Group) == true)
                            {
                                continue;
                            }

                            if (toolCheck.Group.Equals(disinclude.Group) == false)
                            {
                                continue;
                            }

                            if (toolCheck.IsCheck == true)
                            {
                                toolCheck.Check.Execute(false).Subscribe();

                                return;
                            }
                        }
                    }
                }
            }
        }

        [Reactive]
        public ObservableCollection<IToolItem> Tools { get; set; }
    }
}
