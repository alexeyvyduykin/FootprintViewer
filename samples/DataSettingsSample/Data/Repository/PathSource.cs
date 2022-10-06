using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public class PathSource<T> : BaseSource
    {
        private readonly string _path;
        public PathSource(string path)
        {
            _path = path;
        }
        public override IList<object> GetValues()
        {
            return Repository.DeserializeFromStream<T>(_path).Cast<object>().ToList();
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
            return await Task.Run(() => Repository.DeserializeFromStream<T>(_path).Cast<object>().ToList());
        }
    }
}
