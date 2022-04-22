namespace InteractiveGeometry.UI.Input.Core
{
    public interface IViewCommand
    {
        void Execute(IView view, IController controller, InputEventArgs args);
    }
}