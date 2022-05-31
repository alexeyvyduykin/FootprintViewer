using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace FootprintViewer.ViewModels
{
    public class SnapshotMaker : ReactiveObject
    {
        private readonly List<string> _extensions;
        private readonly ReactiveCommand<Unit, Unit> _create;

        public SnapshotMaker()
        {
            _extensions = new List<string>() { "*.png", "*.jrpg", "*.pdf" };

            SelectedExtension = Extensions.FirstOrDefault();

            _create = ReactiveCommand.CreateFromObservable<Unit, Unit>(s => Observable.Return(Unit.Default).Delay(TimeSpan.FromSeconds(2)));
        }

        public ICommand Create => _create;

        [Reactive]
        public string? SelectedExtension { get; set; }

        public List<string> Extensions => _extensions;
    }
}
