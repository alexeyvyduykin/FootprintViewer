using FootprintViewer.Data;

namespace RandomDataBuilder;

public abstract class BaseRandomSource : BaseSource
{
    public int GenerateCount { get; init; } = 0;
}
