using System;

namespace FootprintViewer.Data.Sources
{
    public class FileSource : IFileSource
    {
        public string Path { get; init; } = string.Empty;

        public bool Equals(IDataSource? other)
        {
            if (other is IFileSource source)
            {
                if (source.Path == Path)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
