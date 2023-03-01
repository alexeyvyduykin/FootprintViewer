namespace FootprintViewer.Data.Models;

public class MapResource
{
    public MapResource(string name, string path)
    {
        Name = name;
        Path = path;
    }
    public static Func<IList<string>, IList<object>> Builder =>
        paths => paths.Select(path => new MapResource(System.IO.Path.GetFileNameWithoutExtension(path), path)).ToList<object>();

    public string Name { get; private set; }

    public string Path { get; private set; }
}
