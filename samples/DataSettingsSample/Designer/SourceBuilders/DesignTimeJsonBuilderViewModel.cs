using Avalonia;
using Avalonia.Platform;
using DataSettingsSample.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DataSettingsSample.Designer
{
    public class DesignTimeJsonBuilderViewModel : JsonBuilderViewModel
    {
        public DesignTimeJsonBuilderViewModel() : base("Footprints")
        {
            var list1 = new List<FileViewModel>();
            var list2 = new List<FileViewModel>();

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            var res = assets?.GetAssets(new Uri("avares://DataSettingsSample/Assets/"), null).ToList() ?? new List<Uri>();

            foreach (var uri in res)
            {
                if (Equals(Path.GetExtension(uri.LocalPath), ".json") == true)
                {
                    var filename = Path.GetFileName(uri.LocalPath);
                    var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

                    if (new Random().Next(0, 2) == 1)
                    {
                        list1.Add(new FileViewModel()
                        {
                            Name = filename,
                            Path = Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets", filename)),
                            IsSelected = new Random().Next(0, 2) == 1,
                        });
                    }
                    else
                    {
                        var file = new FileViewModel()
                        {
                            Name = filename,
                            Path = Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets", filename)),
                            IsSelected = new Random().Next(0, 2) == 1,
                        };

                        file.Verified("Footprints");

                        list2.Add(file);
                    }
                }
            }

            AddToAvailableList(list1);

            AddToTargetList(list2);
        }
    }
}
