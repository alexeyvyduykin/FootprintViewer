using Avalonia;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DataSettingsSample.Data
{
    public class StreamSource<T> : BaseSource
    {
        private Stream? _stream;
        private readonly Uri _uri;

        public StreamSource(Uri uri)
        {
            _uri = uri;
        }

        public override IList<object> GetValues()
        {
            if (_stream == null)
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                _stream = assets?.Open(_uri)!;
            }

            return Repository.DeserializeFromStream<T>(_stream).Cast<object>().ToList();
        }

        public override async Task<IList<object>> GetValuesAsync()
        {
            if (_stream == null)
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                _stream = assets?.Open(_uri!)!;
            }

            return await Task.Run(() => Repository.DeserializeFromStream<T>(_stream).Cast<object>().ToList());
        }
    }
}
