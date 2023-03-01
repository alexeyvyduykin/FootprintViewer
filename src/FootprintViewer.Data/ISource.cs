namespace FootprintViewer.Data;

public interface ISource
{
    Task<IList<object>> GetValuesAsync();
}
