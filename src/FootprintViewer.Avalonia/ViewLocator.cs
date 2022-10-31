using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Avalonia;

public class ViewLocator : IDataTemplate
{
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        name = name.Replace("FootprintViewer", "FootprintViewer.Avalonia");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object data)
    {
        return data is ViewModelBase;
    }
}
