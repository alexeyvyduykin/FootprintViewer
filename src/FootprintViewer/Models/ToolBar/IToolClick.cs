using System.Windows.Input;

namespace FootprintViewer.Models;

public interface IToolClick : ITool
{
    ICommand Click { get; }
}
