using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public interface ISource
    {
        IList<object> GetValues();

        Task<IList<object>> GetValuesAsync();
    }
}
