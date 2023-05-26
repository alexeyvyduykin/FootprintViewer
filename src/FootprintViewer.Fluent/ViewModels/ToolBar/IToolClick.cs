using System.Windows.Input;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public interface IToolClick : ITool
{
    ICommand Click { get; }
}
