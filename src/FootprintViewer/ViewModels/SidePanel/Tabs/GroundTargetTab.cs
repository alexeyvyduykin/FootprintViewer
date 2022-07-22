using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetTab : SidePanelTab
    {
        private readonly ITargetLayerSource _source;
        private readonly IProvider<GroundTarget> _provider;
        private string[]? _names;
        private readonly IViewerList<GroundTargetViewModel> _viewerList;

        public GroundTargetTab(IReadonlyDependencyResolver dependencyResolver)
        {
            _provider = dependencyResolver.GetExistingService<IProvider<GroundTarget>>();
            _source = dependencyResolver.GetExistingService<ITargetLayerSource>();
            var viewModelFactory = dependencyResolver.GetExistingService<ViewModelFactory>();

            Title = "Просмотр наземных целей";

            var preview = new PreviewMainContent("Наземные цели при текущем приблежение не доступны");

            _viewerList = viewModelFactory.CreateGroundTargetViewerList(_provider);

            _provider.Observable.Skip(1).Select(s => (IFilter<GroundTargetViewModel>?)null).InvokeCommand(_viewerList.Loading);

            // Update

            _source.Refresh.Subscribe(names =>
            {
                _names = names;

                if (IsActive == true)
                {
                    IsEnable = (names != null);

                    if (IsEnable == true)
                    {
                        _viewerList.FiringUpdate(_names, 0.0);
                    }
                }
            });

            this.WhenAnyValue(s => s.IsEnable).Where(s => s == true).Subscribe(_ => MainContent = (ReactiveObject)_viewerList);
            this.WhenAnyValue(s => s.IsEnable).Where(s => s == false).Subscribe(_ => MainContent = preview);

            this.WhenAnyValue(s => s.IsActive).Where(s => s == true).Subscribe(_ =>
            {
                IsEnable = (_names != null);

                if (IsEnable == true)
                {
                    _viewerList.FiringUpdate(_names, 0.0);
                }
            });
        }

        public async Task<List<GroundTargetViewModel>> GetGroundTargetViewModelsAsync(string name)
        {
            return await _provider.GetValuesAsync(new NameFilter<GroundTargetViewModel>(new[] { name }), s => new GroundTargetViewModel(s));
        }

        [Reactive]
        private bool IsEnable { get; set; }

        [Reactive]
        public ReactiveObject? MainContent { get; private set; }

        public IViewerList<GroundTargetViewModel> ViewerList => _viewerList;
    }
}
