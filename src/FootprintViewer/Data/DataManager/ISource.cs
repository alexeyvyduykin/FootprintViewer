using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager;

public interface ISource
{
    Task<IList<object>> GetValuesAsync();
}
