namespace InteractiveSample.Input.Controller.Core
{
    public interface IViewCommand
    {
        void Execute(IView view, IController controller, InputEventArgs args);
    }
}