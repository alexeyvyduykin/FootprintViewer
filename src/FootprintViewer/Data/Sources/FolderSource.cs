using System;

namespace FootprintViewer.Data.Sources
{
    public class FolderSource : IFolderSource
    {
        public string Directory { get; init; } = string.Empty;

        public string SearchPattern { get; init; } = string.Empty;

        public bool Equals(IDataSource? other)
        {
            if (other is IFolderSource source)
            {
                if (source.Directory == Directory &&
                    source.SearchPattern == SearchPattern)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
