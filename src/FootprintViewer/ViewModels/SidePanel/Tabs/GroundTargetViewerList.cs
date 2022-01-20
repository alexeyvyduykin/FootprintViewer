using Mapsui.Providers;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewerList : ReactiveObject
    {
        private readonly IGroundTargetDataSource _dataSource;
        private IEnumerable<IFeature>? _features;
        private readonly ReactiveCommand<Unit, Unit> begin;
        private readonly ReactiveCommand<Unit, Unit> end;
        private readonly ReactiveCommand<Unit, Unit> disable;

        public GroundTargetViewerList(IReadonlyDependencyResolver dependencyResolver)
        {
            _dataSource = dependencyResolver.GetExistingService<IGroundTargetDataSource>();

            GroundTargetInfos = new ObservableCollection<GroundTargetInfo>();

            _dataSource.RefreshDataObservable.Subscribe(f => GroundTargetsChanged(f));

            begin = ReactiveCommand.Create(() => { });

            end = ReactiveCommand.Create(() => { });

            disable = ReactiveCommand.Create(() => { });
        }

        public IObservable<Unit> BeginUpdate => begin;

        public IObservable<Unit> EndUpdate => end;

        public IObservable<Unit> Disable => disable;

        public IObservable<GroundTargetInfo?> SelectedItemObservable => this.WhenAnyValue(s => s.SelectedGroundTargetInfo);

        public void Update() => GroundTargetsChanged(_features);

        public void Reset() => ResetGroundTargets();

        private static async Task<IList<GroundTargetInfo>> LoadDataAsync(IGroundTargetDataSource dataSource, IEnumerable<IFeature> features)
        {
            return await Task.Run(() =>
            {
                Thread.Sleep(500);
                return dataSource.GetTargets(features).Select(s => new GroundTargetInfo(s)).ToList();
            });
        }

        private static async Task<IList<GroundTargetInfo>> LoadDataAsync(IGroundTargetDataSource dataSource)
        {
            return await Task.Run(() =>
            {
                return dataSource.GetTargets().Select(s => new GroundTargetInfo(s)).ToList();
            });
        }

        private async void GroundTargetsChanged(IEnumerable<IFeature>? features)
        {
            _features = features;

            if (features != null)
            {
                begin.Execute().Subscribe();

                var targets = await LoadDataAsync(_dataSource, features);

                GroundTargetInfos = new ObservableCollection<GroundTargetInfo>(targets);

                end.Execute().Subscribe();
            }
            else
            {
                GroundTargetInfos = new ObservableCollection<GroundTargetInfo>();

                disable.Execute().Subscribe();
            }
        }

        private async void ResetGroundTargets()
        {
            begin.Execute().Subscribe();

            var targets = await LoadDataAsync(_dataSource);

            GroundTargetInfos = new ObservableCollection<GroundTargetInfo>(targets);

            end.Execute().Subscribe();
        }

        [Reactive]
        public GroundTargetInfo? SelectedGroundTargetInfo { get; set; }

        [Reactive]
        public ObservableCollection<GroundTargetInfo> GroundTargetInfos { get; set; }
    }
}
