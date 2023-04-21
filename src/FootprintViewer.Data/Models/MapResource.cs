namespace FootprintViewer.Data.Models;

public class MapResource
{
    public MapResource(string name, string path)
    {
        Name = name;
        Path = path;
    }

    public static MapResource Build(string path)
    {
        return new MapResource(System.IO.Path.GetFileNameWithoutExtension(path), path);
    }

    public string Name { get; private set; }

    public string Path { get; private set; }
}
