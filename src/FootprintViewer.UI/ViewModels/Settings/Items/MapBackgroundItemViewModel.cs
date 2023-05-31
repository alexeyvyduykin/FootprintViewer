using System.IO;

namespace FootprintViewer.UI.ViewModels.Settings.Items;

public class MapBackgroundItemViewModel
{
    private readonly string _defaultName = "world";

    public MapBackgroundItemViewModel(string pathFile)
    {
        Name = Path.GetFileNameWithoutExtension(pathFile);

        IsRemovable = !string.Equals(Name, _defaultName);

        FullPath = pathFile;
    }

    public bool IsRemovable { get; set; }

    public string Name { get; set; }

    public string FullPath { get; set; }
}
