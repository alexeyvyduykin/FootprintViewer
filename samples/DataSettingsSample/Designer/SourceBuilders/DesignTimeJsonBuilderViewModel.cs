using Avalonia;
using Avalonia.Platform;
using DataSettingsSample.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSettingsSample.Designer
{
    public class DesignTimeJsonBuilderViewModel : JsonBuilderViewModel
    {
        public DesignTimeJsonBuilderViewModel()
        {
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
                        list1.Add(new FileViewModel()
                        {
                            Name = Path.GetFileName(uri.LocalPath),
                            IsSelected = new Random().Next(0, 2) == 1,
                        });
                    }
                    else
                    {
                        list2.Add(new FileViewModel()
                        {
                            Name = Path.GetFileName(uri.LocalPath),
                            IsSelected = new Random().Next(0, 2) == 1,
                            IsVerified = new Random().Next(0, 2) == 1,
                        });
                    }
                }
            }

            AvailableFiles = new List<FileViewModel>(list1);

            TargetFiles = new List<FileViewModel>(list2);
        }
    }
}
