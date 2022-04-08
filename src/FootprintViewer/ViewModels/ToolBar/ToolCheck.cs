using FootprintViewer.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
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
                Debug.WriteLine($"{Title} tool is before activate.");
                return this;
            });

            Activate = ReactiveCommand.Create<IToolCheck>(() =>
            {
                Debug.WriteLine($"{Title} tool  is activate.");
                return this;
            });

            Deactivate = ReactiveCommand.Create<IToolCheck>(() =>
            {
                Debug.WriteLine($"{Title} tool is deactivate.");
                return this;
            });

            var combined = ReactiveCommand.CreateCombined(new[] { BeforeActivate, Activate });

            this.WhenAnyValue(s => s.IsCheck).Where(s => s == true).Select(_ => Unit.Default).InvokeCommand(combined);

            this.WhenAnyValue(s => s.IsCheck).Where(s => s == false).Select(_ => Unit.Default).InvokeCommand(Deactivate);
        }

        public string GetKey() => Title ?? string.Empty;

        [Reactive]
        public bool IsCheck { get; set; }

        public string? Group { get; set; }

        public string? Tooltip { get; set; }

        public string? Title { get; set; }

        public ReactiveCommand<Unit, IToolCheck> BeforeActivate { get; }

        public ReactiveCommand<Unit, IToolCheck> Activate { get; }

        public ReactiveCommand<Unit, IToolCheck> Deactivate { get; }
    }
}
