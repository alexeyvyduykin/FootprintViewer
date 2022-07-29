using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class NameFilter<T> : BaseFilterViewModel<T> where T : IViewerItem
    {
        private readonly string[]? _names;

        public NameFilter(string[]? names) : base()
        {
            _names = names;
        }

        public override IObservable<Func<T, bool>> FilterObservable =>
            this.WhenAnyValue(s => s.FilterNames)
            .Throttle(TimeSpan.FromSeconds(1))
            .Select(_ => this)
            .Select(CreatePredicate);

        private static Func<T, bool> CreatePredicate(NameFilter<T> filter)
        {
            return value =>
            {
                return (filter.FilterNames == null) || filter.FilterNames.Contains(value.Name);
            };
        }

        [Reactive]
        public string[]? FilterNames { get; set; }

        public override string[]? Names => _names;

        public override bool Filtering(T value)
        {
            return (_names == null) || _names.Contains(value.Name);
        }
    }
}
