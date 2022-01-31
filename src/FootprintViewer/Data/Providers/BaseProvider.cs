using System.Collections.Generic;

namespace FootprintViewer.Data
{
    public abstract class BaseProvider<T>
    {
        private readonly IList<T> _sources = new List<T>();

        public void AddSource(T source)
        {
            _sources.Add(source);
        }

        public IEnumerable<T> Sources => _sources;
    }
}
