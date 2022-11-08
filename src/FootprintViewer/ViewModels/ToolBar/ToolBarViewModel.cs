using FootprintViewer.Models;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels.ToolBar;

public class ToolBarViewModel : ViewModelBase
{
    public ToolBarViewModel()
    {
        Tools = new ObservableCollection<IToolItem>();
    }

    public void AddTool(IToolItem tool)
    {
        if (tool is IToolCheck check)
        {
            check.BeforeActivate.Subscribe(UncheckDisinclude);
        }
        else if (tool is IToolCollection collection)
        {
            foreach (var item in collection.GetItems())
            {
                item.BeforeActivate.Subscribe(UncheckDisinclude);
            }
        }

        Tools.Add(tool);
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
                    check.IsCheck = false;

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
                            toolCheck.IsCheck = false;

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
