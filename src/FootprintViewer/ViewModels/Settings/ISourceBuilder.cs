using ReactiveUI;
using Splat;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FootprintViewer.ViewModels
{
    //public interface ISourceBuilder
    //{
    //    string Name { get; }

    //    ReactiveCommand<Unit, ISourceInfo?> Build { get; }

    //    Interaction<ISourceInfo, ISourceInfo?> ShowBuilderDialog { get; }
    //}

    //public abstract class BaseSourceBuilder : ReactiveObject, ISourceBuilder
    //{
    //    public BaseSourceBuilder()
    //    {
    //        Build = ReactiveCommand.CreateFromTask(BuildAsync);

    //        ShowBuilderDialog = new Interaction<ISourceInfo, ISourceInfo?>();
    //    }

    //    public abstract string Name { get; }

    //    public ReactiveCommand<Unit, ISourceInfo?> Build { get; }

    //    public Interaction<ISourceInfo, ISourceInfo?> ShowBuilderDialog { get; }

    //    protected abstract ISourceInfo GetDefaultSource();

    //    private async Task<ISourceInfo?> BuildAsync()
    //    {
    //        return await ShowBuilderDialog.Handle(GetDefaultSource());
    //    }
    //}

    //public class DatabaseSourceBuilder : BaseSourceBuilder
    //{
    //    private readonly IReadonlyDependencyResolver? _dependencyResolver;

    //    public DatabaseSourceBuilder(IReadonlyDependencyResolver? dependencyResolver) : base()
    //    {
    //        _dependencyResolver = dependencyResolver;
    //    }

    //    public TableInfo? TableInfo { get; set; }

    //    public override string Name => "Add database";

    //    protected override ISourceInfo GetDefaultSource()
    //    {
    //        var settings = _dependencyResolver?.GetService<AppSettingsState>();

    //        return new DatabaseSourceInfo()
    //        {
    //            Version = settings?.LastDatabaseSource?.Version,
    //            Host = settings?.LastDatabaseSource?.Host,
    //            Port = (settings?.LastDatabaseSource != null) ? settings.LastDatabaseSource.Port : 0,
    //            Database = settings?.LastDatabaseSource?.Database,
    //            Username = settings?.LastDatabaseSource?.Username,
    //            Password = settings?.LastDatabaseSource?.Password,
    //            Table = settings?.LastDatabaseSource?.Table,
    //            TableInfo = TableInfo,
    //        };
    //    }
    //}

    //public class RandomSourceBuilder : BaseSourceBuilder
    //{
    //    private readonly string _name;

    //    public RandomSourceBuilder(string name)
    //    {
    //        _name = name;
    //    }

    //    public override string Name => "Add random";

    //    protected override ISourceInfo GetDefaultSource() => new RandomSourceInfo(_name);
    //}

    //public class FileSourceBuilder : BaseSourceBuilder
    //{
    //    private readonly string _filterName;
    //    private readonly string _filterExtension;

    //    public FileSourceBuilder(string filterName, string filterExtension) : base()
    //    {
    //        _filterName = filterName;

    //        _filterExtension = filterExtension;
    //    }

    //    public override string Name => "Add file";

    //    protected override ISourceInfo GetDefaultSource() => new FileSourceInfo() { FilterName = _filterName, FilterExtension = _filterExtension };
    //}

    //public class FolderSourceBuilder : BaseSourceBuilder
    //{
    //    private readonly string _searchPattern;

    //    public FolderSourceBuilder(string searchPattern) : base()
    //    {
    //        _searchPattern = searchPattern;
    //    }

    //    public override string Name => "Add folder";

    //    protected override ISourceInfo GetDefaultSource() => new FolderSourceInfo() { SearchPattern = _searchPattern };
    //}
}
