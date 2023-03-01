using FootprintViewer.Data;

namespace FootprintViewer;

public interface IDataFactory
{
    IDataManager CreateDataManager();
}
