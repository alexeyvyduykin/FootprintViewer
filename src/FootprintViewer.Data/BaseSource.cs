namespace FootprintViewer.Data;

public abstract class BaseSource : ISource
{
    public abstract Task<IList<object>> GetValuesAsync();
}
