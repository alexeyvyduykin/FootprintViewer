using System;
using System.Linq;

namespace FootprintViewer.ViewModels
{
    public class NameFilter<T> : ViewerListFilter<T> where T : IViewerItem
    {
        private readonly string[]? _names;

        public NameFilter(string[]? names) : base()
        {
            _names = names;
        }

        public override string[]? Names => _names;

        public override bool Filtering(T value)
        {
            return (_names == null) || _names.Contains(value.Name);
        }
    }
}
