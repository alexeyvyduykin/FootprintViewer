using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootprintViewer.Data.DataManager;

public abstract class BaseSource : ISource
{
    public abstract Task<IList<object>> GetValuesAsync();
}
