using FootprintViewer.Data;

namespace FootprintViewer.Fluent.ViewModels;

public interface ISourceViewModel
{
    string? Name { get; set; }

    ISource Source { get; }
}
