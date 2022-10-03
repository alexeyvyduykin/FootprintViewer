using FootprintViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;

namespace FootprintViewer.ViewModels
{
    public class ToolCheck : ReactiveObject, IToolCheck
    {
        public ToolCheck()
        {
            BeforeActivate = ReactiveCommand.Create<IToolCheck>(() =>
            {
                Debug.WriteLine($"{Tag} tool is before activate.");
                return this;
            });

            Activate = ReactiveCommand.Create<IToolCheck>(() =>
            {
                Debug.WriteLine($"{Tag} tool  is activate.");
                return this;
            });

            Deactivate = ReactiveCommand.Create<IToolCheck>(() =>
            {
                Debug.WriteLine($"{Tag} tool is deactivate.");
                return this;
            });

            var combined = ReactiveCommand.CreateCombined(new[] { BeforeActivate, Activate });

            this.WhenAnyValue(s => s.IsCheck).Where(s => s == true).Select(_ => Unit.Default).InvokeCommand(combined);

            this.WhenAnyValue(s => s.IsCheck).Where(s => s == false).Select(_ => Unit.Default).InvokeCommand(Deactivate);
        }

        public void Subscribe(Action activate, Action deactivate)
        {
            Activate.Subscribe(_ => activate.Invoke());
            Deactivate.Subscribe(_ => deactivate.Invoke());
        }

        public string GetKey() => (string?)Tag ?? string.Empty;

        [Reactive]
        public bool IsCheck { get; set; }

        public string? Group { get; set; }

        public object? Tag { get; set; }

        public ReactiveCommand<Unit, IToolCheck> BeforeActivate { get; }

        public ReactiveCommand<Unit, IToolCheck> Activate { get; }

        public ReactiveCommand<Unit, IToolCheck> Deactivate { get; }
    }
}
