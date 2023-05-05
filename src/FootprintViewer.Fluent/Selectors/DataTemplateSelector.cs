using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using FootprintViewer.Fluent.ViewModels.SidePanel;
using System.Collections.Generic;

namespace FootprintViewer.Fluent.Selectors;

public class DataTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<string, IDataTemplate> Templates { get; } = new Dictionary<string, IDataTemplate>();

    public IControl Build(object param)
    {
        string? key = null;

        if (param is string str)
        {
            key = str;

        }
        else if (param is ISelectorItem selectorItem)
        {
            key = selectorItem.GetKey();
        }

        if (key is not null)
        {
            return Templates[key].Build(param);
        }

        throw new Exception("Key not register in DataTemplateSelector");
    }

    public bool Match(object data)
    {
        return data is ISelectorItem || data is string;
    }
}
