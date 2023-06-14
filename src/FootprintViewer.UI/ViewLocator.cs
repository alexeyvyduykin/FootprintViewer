using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FootprintViewer.UI.ViewModels;

namespace FootprintViewer.UI;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        var name = data?.GetType().FullName!.Replace("ViewModel", "View");
        //name = name.Replace("FootprintViewer", "FootprintViewer.UI");
        var type = name is null ? null : Type.GetType(name);

        if (type != null)
        {
            try
            {
                var instance = Activator.CreateInstance(type);
                if (instance is Control control)
                {
                    return control;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
