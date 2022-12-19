using FootprintViewer.Data.DataManager;

namespace RandomDataBuilder;

public abstract class BaseRandomSource : BaseSource
{
    public int GenerateCount { get; init; } = 0;
}
