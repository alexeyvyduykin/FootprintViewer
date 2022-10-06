using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public class StreamSource<T> : BaseSource
    {
        private readonly Stream _stream;

        public StreamSource(Stream stream)
        {
            _stream = stream;
        }

        public override IList<object> GetValues()
        {
            return Repository.DeserializeFromStream<T>(_stream).Cast<object>().ToList();
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
            return await Task.Run(() => Repository.DeserializeFromStream<T>(_stream).Cast<object>().ToList());
        }
    }
}
