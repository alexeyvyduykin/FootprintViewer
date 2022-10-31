using FootprintViewer.Data.DataManager;
using FootprintViewer.ViewModels.Settings.SourceBuilders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace FootprintViewer.Designer;

public class DesignTimeJsonBuilderViewModel : JsonBuilderViewModel
{
    public DesignTimeJsonBuilderViewModel() : base(DbKeys.Footprints.ToString())
    {
        var list1 = new List<FileViewModel>();
        var list2 = new List<FileViewModel>();

        var root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var localPath = Path.GetFullPath(Path.Combine(root, @"..\..\..\Assets"));
        string[] files = System.IO.Directory.GetFiles(localPath);

        foreach (var f in files)
        {
            if (Equals(Path.GetExtension(f), ".json") == true)
            {
                var filename = Path.GetFileName(f);
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                var fullPath = Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets", filename));
                if (new Random().Next(0, 2) == 1)
                {
                    list1.Add(new FileViewModel(fullPath)
                    {
                        Name = filename,
                        IsSelected = new Random().Next(0, 2) == 1,
                    });
                }
                else
                {
                    var file = new FileViewModel(Path.GetFullPath(Path.Combine(path, @"..\..\..\Assets", filename)))
                    {
                        Name = filename,
                        IsSelected = new Random().Next(0, 2) == 1,
                    };

                    file.Verified(DbKeys.Footprints.ToString());

                    list2.Add(file);
                }
            }
        }

        AddToAvailableList(list1);

        AddToTargetList(list2);
    }
}
