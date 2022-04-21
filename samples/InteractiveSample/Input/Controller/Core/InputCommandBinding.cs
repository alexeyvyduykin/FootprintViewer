namespace InteractiveSample.Input.Controller.Core
{
    public class InputCommandBinding
    {
        public InputCommandBinding(InputGesture gesture, IViewCommand command)
        {
            Gesture = gesture;
            Command = command;
        }

        public InputGesture Gesture { get; private set; }

        public IViewCommand Command { get; private set; }
    }
}