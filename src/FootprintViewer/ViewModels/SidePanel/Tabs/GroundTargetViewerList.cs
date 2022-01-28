using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
        public GroundTargetViewerList()
        {
            GroundTargetInfos = new ObservableCollection<GroundTargetInfo>();

            UpdateAsync = ReactiveCommand.CreateFromTask<Func<IEnumerable<GroundTargetInfo>>>(UpdateAsyncImpl);
        }

        public IObservable<GroundTargetInfo?> SelectedItemObservable => this.WhenAnyValue(s => s.SelectedGroundTargetInfo);

        private async Task UpdateAsyncImpl(Func<IEnumerable<GroundTargetInfo>> load)
        {
            var targets = await Task.Run(() =>
            {
                Thread.Sleep(500);
                return load();
            });

            GroundTargetInfos = new ObservableCollection<GroundTargetInfo>(targets);
        }

        public ReactiveCommand<Func<IEnumerable<GroundTargetInfo>>, Unit> UpdateAsync { get; }

        [Reactive]
        public GroundTargetInfo? SelectedGroundTargetInfo { get; set; }

        [Reactive]
        public ObservableCollection<GroundTargetInfo> GroundTargetInfos { get; private set; }
    }

    public static class GroundTargetViewerListExtensions
    {
        public static void InvalidateData(this GroundTargetViewerList list, Func<IEnumerable<GroundTarget>> load)
        {
            list.UpdateAsync.Execute(() => load().Select(s => new GroundTargetInfo(s))).Subscribe();
        }

        public static void InvalidateData(this GroundTargetViewerList list, Func<IEnumerable<GroundTargetInfo>> load)
        {
            list.UpdateAsync.Execute(() => load()).Subscribe();
        }
    }
}
