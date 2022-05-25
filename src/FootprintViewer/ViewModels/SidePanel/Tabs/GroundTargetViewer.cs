using FootprintViewer.Data;
using FootprintViewer.Layers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewer : SidePanelTab
    {
        private readonly ITargetLayerSource _source;
        private readonly IProvider<GroundTargetInfo> _groundTargetProvider;

        public GroundTargetViewer(IReadonlyDependencyResolver dependencyResolver)
        {
            _groundTargetProvider = dependencyResolver.GetExistingService<IProvider<GroundTargetInfo>>();
            _source = dependencyResolver.GetExistingService<ITargetLayerSource>();

            Title = "Просмотр наземных целей";

            Preview = new PreviewMainContent("Наземные цели при текущем приблежение не доступны");

            ViewerList = ViewerListBuilder.CreateViewerList(_groundTargetProvider);

            //          ShowHighlight = ReactiveCommand.Create<GroundTargetInfo?>(ShowHighlightImpl);

            //          HideHighlight = ReactiveCommand.Create(HideHighlightImpl);

            // First loading

            IsEnable = false;

            // Update

            _source.Refresh.Subscribe(s => Names = s);

            this.WhenAnyValue(s => s.Names).Subscribe(s => IsEnable = !(s == null));

            this.WhenAnyValue(s => s.IsActive, s => s.IsEnable, s => s.Names)
                .Where((s) => s.Item1 == true && s.Item2 == true)
                .Subscribe(_ => ViewerList.FiringUpdate(Names));
        }

        public async Task<List<GroundTargetInfo>> GetGroundTargetInfoAsync(string name)
        {
            return await _groundTargetProvider.GetValuesAsync(new NameFilter<GroundTargetInfo>(new[] { name }));
        }

        private void ShowHighlightImpl(GroundTargetInfo? groundTarget)
        {
            if (groundTarget != null)
            {
                var name = groundTarget.Name;

                if (name != null)
                {
                    _source.ShowHighlight(name);
                }
            }
        }

        private void HideHighlightImpl()
        {
            _source.HideHighlight();
        }

        [Reactive]
        private string[]? Names { get; set; }

        public ReactiveCommand<GroundTargetInfo?, Unit> ShowHighlight { get; }

        public ReactiveCommand<Unit, Unit> HideHighlight { get; }

        [Reactive]
        public bool IsEnable { get; set; }

        [Reactive]
        public PreviewMainContent Preview { get; private set; }

        [Reactive]
        public IViewerList<GroundTargetInfo> ViewerList { get; private set; }
    }
}
