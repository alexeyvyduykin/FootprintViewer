namespace FootprintViewer.Data.Sources
{
    public class RandomSource : IRandomSource
    {
        public string Name { get; init; } = string.Empty;

        public int GenerateCount { get; init; }

        public bool Equals(IDataSource? other)
        {
            if (other is IRandomSource source)
            {
                if (source.Name == Name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
