namespace FootprintViewer.AppStates;

public interface ISuspendableState<T>
{
    void LoadState(T state);
}
