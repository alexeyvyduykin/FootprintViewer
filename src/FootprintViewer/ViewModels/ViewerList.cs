using FootprintViewer.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public static class ViewerListBuilder
    {
        public static IFilter<T> CreateNameFilter<T>(string[]? names) where T : IViewerItem
        {
            return new NameFilter<T>(names);
        }

        public static IViewerList<T> CreateViewerList<TNative, T>(IProvider<TNative> provider, Func<TNative, T> converter, Func<T, TNative> converterBack) where T : IViewerItem
        {
            var viewerList = new ViewerList<TNative, T>(provider, converter, converterBack);

            if (provider is Provider<TNative> prvd)
            {
                // TODO: add to viewerList Update<Unit,Unit> command
                prvd.UpdateSources.Select(s => (IFilter<T>?)null).InvokeCommand(viewerList.Loading);
            }

            return viewerList;
        }
    }

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

    public class ViewerList<TNative, T> : BaseViewerList<T> where T : IViewerItem
    {
        private readonly IProvider<TNative> _provider;
        private Func<TNative, T> _converter;
        private Func<T, TNative> _converterBack;

        public ViewerList(IProvider<TNative> provider, Func<TNative, T> converter, Func<T, TNative> converterBack) : base()
        {
            _provider = provider;

            _converter = converter;

            _converterBack = converterBack;
        }

        protected override async Task<List<T>> LoadingAsync(IFilter<T>? filter = null)
        {
            return await _provider.GetValuesAsync(filter, _converter);
        }

        protected override async Task AddAsync(T? value)
        {
            if (value != null && _provider is IEditableProvider<TNative> editableProvider)
            {
                await editableProvider.AddAsync(_converterBack(value));
            }
        }

        protected override async Task RemoveAsync(T? value)
        {
            if (value != null && _provider is IEditableProvider<TNative> editableProvider)
            {
                await editableProvider.RemoveAsync(_converterBack(value));
            }
        }
    }
}
