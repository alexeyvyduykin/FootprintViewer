using FootprintViewer.Input;

namespace FootprintViewer
{
    public interface IViewCommand
    {
        void Execute(IView view, IController controller, InputEventArgs args);
    }
}