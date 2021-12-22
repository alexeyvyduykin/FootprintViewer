namespace InteractivitySample.Input.Controller.Core
{
    public interface IViewCommand<in T> : IViewCommand where T : InputEventArgs
    {
        void Execute(IView view, IController controller, T args);
    }
}