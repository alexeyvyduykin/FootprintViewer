using DataSettingsSample.ViewModels.Interfaces;
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
        private static readonly IDictionary<string, Func<ISourceBuilderViewModel>> _cache = new Dictionary<string, Func<ISourceBuilderViewModel>>();

        static ProviderViewModel()
        {
            _cache.Add(".database", () => new DatabaseBuilderViewModel());
            _cache.Add(".json", () => new JsonBuilderViewModel());
        }

        public ProviderViewModel()
        {
            Sources = new List<ISourceViewModel>();
            SourceBuilderItems = new List<SourceBuilderItemViewModel>();

            Add = ReactiveCommand.Create<SourceBuilderItemViewModel>(AddImpl, outputScheduler: RxApp.MainThreadScheduler);

            Remove = ReactiveCommand.Create<ISourceViewModel>(RemoveImpl, outputScheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(s => s.SourceBuilderItemSelected)
                .Where(s => s != null && SourceBuilderVisible == false)
                .Select(s => s!)
                .InvokeCommand(Add);
        }

        protected void AddImpl(SourceBuilderItemViewModel item)
        {
            SourceBuilder = CreateSourceBuilder(item.Name!);

            SourceBuilderVisible = true;
        }

        protected void RemoveImpl(ISourceViewModel source)
        {
            var list = Sources.ToList();

            var index = list.FindIndex(s => string.Equals(s.Name, source.Name));

            list.RemoveAt(index);

            Sources = new List<ISourceViewModel>(list);
        }

        private ISourceBuilderViewModel CreateSourceBuilder(string key)
        {
            var sourceBuilder = _cache[key].Invoke();

            sourceBuilder.Add.Subscribe(source =>
            {
                var list = Sources.ToList();

                var number = Min(list);

                source.Name = $"Source{number}";

                list.Add(source);

                Sources = new List<ISourceViewModel>(list);

                SourceBuilderVisible = false;
            });

            sourceBuilder.Cancel.Subscribe(s =>
            {
                SourceBuilderVisible = false;
            });

            return sourceBuilder;
        }

        private int Min(List<ISourceViewModel> list)
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

        public ReactiveCommand<SourceBuilderItemViewModel, Unit> Add { get; set; }

        public ReactiveCommand<ISourceViewModel, Unit> Remove { get; set; }

        [Reactive]
        public ISourceBuilderViewModel? SourceBuilder { get; set; }

        [Reactive]
        public SourceBuilderItemViewModel? SourceBuilderItemSelected { get; set; }

        [Reactive]
        public bool SourceBuilderVisible { get; set; } = false;

        [Reactive]
        public IList<SourceBuilderItemViewModel> SourceBuilderItems { get; set; }

        [Reactive]
        public string? Header { get; set; }

        [Reactive]
        public IList<ISourceViewModel> Sources { get; set; }
    }
}
