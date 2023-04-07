using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using FootprintViewer.ViewModels;
using System.Collections.Generic;

namespace FootprintViewer.Fluent.Selectors;

public class DataTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<string, IDataTemplate> Templates { get; } = new Dictionary<string, IDataTemplate>();

    public IControl Build(object param)
    {
        var key = ((ISelectorItem)param).GetKey();

        if (key != null)
        {
            return Templates[key].Build(param);
        }

        throw new Exception();
    }

    public bool Match(object data)
    {
        return data is ISelectorItem;
    }
}
