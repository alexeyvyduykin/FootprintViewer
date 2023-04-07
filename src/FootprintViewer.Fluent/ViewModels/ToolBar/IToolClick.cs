using System.Windows.Input;

namespace FootprintViewer.Fluent.ViewModels.ToolBar;

public interface IToolClick : ITool
{
    ICommand Click { get; }
}
