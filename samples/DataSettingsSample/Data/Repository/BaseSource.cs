using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public abstract class BaseSource : ISource
    {
        public abstract Task<IList<object>> GetValuesAsync();
    }
}
