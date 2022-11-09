using System.Windows.Input;

namespace FootprintViewer.ViewModels;

public interface IToolClick : ITool
{
    ICommand Click { get; }
}
