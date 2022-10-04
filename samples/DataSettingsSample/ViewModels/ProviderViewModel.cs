using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace DataSettingsSample.ViewModels
{
    public class ProviderViewModel : ReactiveObject
    {
        public ProviderViewModel()
        {
            Sources = new List<SourceViewModel>();
            SourceBuilderItems = new List<SourceBuilderItemViewModel>();

            Add = ReactiveCommand.Create(AddImpl, outputScheduler: RxApp.MainThreadScheduler);

            Remove = ReactiveCommand.Create<SourceViewModel>(RemoveImpl, outputScheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(s => s.SourceBuilderItemSelected)
                .Where(s => s != null && SourceBuilderVisible == false)
                .Select(s => Unit.Default)
                .InvokeCommand(Add);
        }

        protected void AddImpl()
        {
            SourceBuilder = CreateSourceBuilder();

            SourceBuilderVisible = true;
        }

        protected void RemoveImpl(SourceViewModel source)
        {
            var list = Sources.ToList();

            var index = list.FindIndex(s => string.Equals(s.Name, source.Name));

            list.RemoveAt(index);

            Sources = new List<SourceViewModel>(list);
        }

        private SourceBuilderViewModel CreateSourceBuilder()
        {
            var sourceBuilder = new SourceBuilderViewModel();

            sourceBuilder.Add.Subscribe(source =>
            {
                var list = Sources.ToList();

                var number = Min(list);

                source.Name = $"Source{number}";

                list.Add(source);

                Sources = new List<SourceViewModel>(list);

                SourceBuilderVisible = false;
            });

            sourceBuilder.Cancel.Subscribe(s =>
            {
                SourceBuilderVisible = false;
            });

            return sourceBuilder;
        }

        public int Min(List<SourceViewModel> list)
        {
            var list1 = list.Where(s => s.Name?.Contains("Source") ?? false).Select(s => s.Name!).ToList();
            var list2 = list1.Select(s => s.Replace("Source", "")).ToList();
            var numbers = list2.Where(s => int.TryParse(s, out var _)).Select(s => int.Parse(s)).Where(s => s > 0).ToList();

            int min = 1;

            numbers.Sort();

            foreach (var item in numbers)
            {
                if (item != min)
                {
                    return min;
                }
                else
                {
                    min++;
                }
            }

            return min;
        }

        public ReactiveCommand<Unit, Unit> Add { get; set; }

        public ReactiveCommand<SourceViewModel, Unit> Remove { get; set; }

        [Reactive]
        public SourceBuilderViewModel? SourceBuilder { get; set; }

        [Reactive]
        public SourceBuilderItemViewModel? SourceBuilderItemSelected { get; set; }

        [Reactive]
        public bool SourceBuilderVisible { get; set; } = false;

        [Reactive]
        public IList<SourceBuilderItemViewModel> SourceBuilderItems { get; set; }

        [Reactive]
        public string? Header { get; set; }

        [Reactive]
        public IList<SourceViewModel> Sources { get; set; }
    }
}
