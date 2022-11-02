namespace FootprintViewer.Data;

public interface IDataFactory
{
    FootprintViewer.Data.DataManager.IDataManager CreateDataManager();
}
