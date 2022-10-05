using Avalonia;
using Avalonia.Platform;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;

namespace DataSettingsSample.ViewModels
{
    public class JsonBuilderViewModel : BaseSourceBuilderViewModel, IJsonBuilderViewModel
    {
        private readonly string _key = string.Empty;

        public JsonBuilderViewModel(string providerName)
        {
            _key = providerName;

            var list1 = new List<FileViewModel>();
            var list2 = new List<FileViewModel>();

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var res = assets?.GetAssets(new Uri("avares://DataSettingsSample/Assets/"), null).ToList() ?? new List<Uri>();

            foreach (var uri in res)
            {
                if (Equals(Path.GetExtension(uri.LocalPath), ".json") == true)
                {
                    if (new Random().Next(0, 2) == 1)
                    {
                        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                        var filename = Path.GetFileName(uri.LocalPath);
                        list1.Add(new FileViewModel()
                        {
                            Name = filename,
                            Path = Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets", filename)),
                            IsSelected = new Random().Next(0, 2) == 1,
                        });
                    }
                    else
                    {
                        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                        var filename = Path.GetFileName(uri.LocalPath);
                        var file = new FileViewModel()
                        {
                            Name = filename,
                            Path = Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets", filename)),
                            IsSelected = new Random().Next(0, 2) == 1,
                        };

                        file.Verified(_key);

                        list2.Add(file);
                    }
                }
            }

            AvailableFiles = new List<FileViewModel>(list1);

            TargetFiles = new List<FileViewModel>(list2);

            Update = ReactiveCommand.Create<string?>(UpdateImpl, outputScheduler: RxApp.MainThreadScheduler);
            ToTarget = ReactiveCommand.Create(ToTargetImpl, outputScheduler: RxApp.MainThreadScheduler);
            FromTarget = ReactiveCommand.Create(FromTargetImpl, outputScheduler: RxApp.MainThreadScheduler);

            this.WhenAnyValue(s => s.Directory).InvokeCommand(Update);
        }

        private void UpdateImpl(string? directory)
        {
            if (directory != null)
            {
                var paths = System.IO.Directory.GetFiles(directory, "*.json").Select(Path.GetFullPath);

                var list = paths.Select(path => new FileViewModel()
                {
                    Name = Path.GetFileName(path),
                    Path = path,
                    IsSelected = false,
                }).ToList();

                AvailableFiles = new List<FileViewModel>(list);

                TargetFiles = new List<FileViewModel>();
            }
        }

        private void ToTargetImpl()
        {
            var list1 = AvailableFiles.ToList();
            var list2 = TargetFiles.ToList();

            var listFalse = list1.Where(s => s.IsSelected == false).ToList();
            var listTrue = list1.Where(s => s.IsSelected == true).ToList();

            listTrue.ForEach(s => s.IsSelected = false);
            listTrue.ForEach(s => s.Verified(_key));

            list2.AddRange(listTrue);

            AvailableFiles = new List<FileViewModel>(listFalse);

            TargetFiles = new List<FileViewModel>(list2);
        }

        private void FromTargetImpl()
        {
            var list1 = AvailableFiles.ToList();
            var list2 = TargetFiles.ToList();

            var listFalse = list2.Where(s => s.IsSelected == false).ToList();
            var listTrue = list2.Where(s => s.IsSelected == true).ToList();

            listTrue.ForEach(s => s.IsSelected = false);

            list1.AddRange(listTrue);

            AvailableFiles = new List<FileViewModel>(list1);

            TargetFiles = new List<FileViewModel>(listFalse);
        }

        private ReactiveCommand<string?, Unit> Update { get; }

        public ReactiveCommand<Unit, Unit> ToTarget { get; }

        public ReactiveCommand<Unit, Unit> FromTarget { get; }

        [Reactive]
        public IList<FileViewModel> AvailableFiles { get; set; }

        [Reactive]
        public IList<FileViewModel> TargetFiles { get; set; }

        [Reactive]
        public string? Directory { get; set; }
    }
}
