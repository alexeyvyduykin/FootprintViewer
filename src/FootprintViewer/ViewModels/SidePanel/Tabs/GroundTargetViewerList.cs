﻿using FootprintViewer.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewerList : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<List<GroundTargetInfo>> _groundTargetInfos;
        private readonly GroundTargetProvider _groundTargetProvider;
        private string[]? _names = new string[] { };

        public GroundTargetViewerList(GroundTargetProvider provider)
        {
            _groundTargetProvider = provider;

            Loading = ReactiveCommand.CreateFromTask<string[], List<GroundTargetInfo>>(LoadingAsync);
           
            //LoadingEx = ReactiveCommand.CreateFromTask<Func<GroundTarget, bool>, List<GroundTargetInfo>>(LoadingExAsync);

            _groundTargetInfos = Loading.ToProperty(this, x => x.GroundTargetInfos, scheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(s => s.Checker).Throttle(TimeSpan.FromSeconds(1)).Select(_ => _names!/*NameFilter(_names)*/).InvokeCommand(Loading);
        }

        public static Func<GroundTarget, bool> NameFilter(string[]? names = null)
        {
            return groundTarget => (names == null) || names.Contains(groundTarget.Name);
        }

        public void Update(string[]? names = null)
        {
            _names = names ?? new string[] { };
            Checker = !Checker;
        }

        private async Task<List<GroundTargetInfo>> LoadingAsync(string[] names)
        {
            return await _groundTargetProvider.GetGroundTargetInfosAsync(names);
        }

        private async Task<List<GroundTargetInfo>> LoadingExAsync(Func<GroundTarget, bool> func)
        {
            return await _groundTargetProvider.GetGroundTargetInfosExAsync(func);
        }

        public ReactiveCommand<string[], List<GroundTargetInfo>> Loading { get; }

        //public ReactiveCommand<Func<GroundTarget, bool>, List<GroundTargetInfo>> LoadingEx { get; }

        public IObservable<GroundTargetInfo?> SelectedItemObservable => this.WhenAnyValue(s => s.SelectedGroundTargetInfo);

        [Reactive]
        private bool Checker { get; set; }

        [Reactive]
        public GroundTargetInfo? SelectedGroundTargetInfo { get; set; }

        public List<GroundTargetInfo> GroundTargetInfos => _groundTargetInfos.Value;
    }
}
