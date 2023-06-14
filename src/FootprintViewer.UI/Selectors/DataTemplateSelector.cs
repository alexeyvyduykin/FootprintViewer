using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using FootprintViewer.UI.ViewModels.SidePanel;
using System.Collections.Generic;

namespace FootprintViewer.UI.Selectors;

public class DataTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<string, IDataTemplate> Templates { get; } = new Dictionary<string, IDataTemplate>();

    public Control Build(object? param)
    {
        string? key = null;

        if (param is SidePanelTabViewModel tab)
        {
            key = tab.Key;
        }
        else if (param is ISelectorItem selectorItem)
        {
            key = selectorItem.Key;
        }
        else if (param is string str)
        {
            key = str;
        }

        if (key is not null)
        {
            return Templates[key].Build(param);
        }

        return new TextBlock() { Text = "error" };// throw new Exception("Key not register in DataTemplateSelector");
    }

    public bool Match(object? data)
    {
        return data is ISelectorItem or string;
    }
}
