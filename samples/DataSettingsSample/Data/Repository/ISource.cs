using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public interface ISource
    {
        Task<IList<object>> GetValuesAsync();
    }
}
