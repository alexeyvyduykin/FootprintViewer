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

        public static IViewerList<T> CreateViewerList<T>(IProvider<T> provider) where T : IViewerItem
        {
            var viewerList = new ViewerList<T>(provider);

            if (provider is Provider<T> prvd)
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

    public class ViewerList<T> : BaseViewerList<T> where T : IViewerItem
    {
        private readonly IProvider<T> _provider;

        public ViewerList(IProvider<T> provider) : base()
        {
            _provider = provider;
        }

        protected override async Task<List<T>> LoadingAsync(IFilter<T>? filter = null)
        {
            return await _provider.GetValuesAsync(filter);
        }

        protected override async Task AddAsync(T? value)
        {
            if (value != null && _provider is IEditableProvider<T> editableProvider)
            {
                await editableProvider.AddAsync(value);
            }
        }

        protected override async Task RemoveAsync(T? value)
        {
            if (value != null && _provider is IEditableProvider<T> editableProvider)
            {
                await editableProvider.RemoveAsync(value);
            }
        }
    }
}
