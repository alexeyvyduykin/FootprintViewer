using System.Collections.Generic;

namespace DataSettingsSample.ViewModels.Interfaces
{
    public interface ISourceViewModel
    {
        string Name { get; set; }
    }

    public interface IDatabaseSourceViewModel : ISourceViewModel
    {
        string Host { get; }

        int Port { get; }

        string Database { get; }

        string Username { get; }

        string Password { get; }

        string Table { get; }
    }

    public interface IJsonSourceViewModel : ISourceViewModel
    {
        IReadOnlyList<FileViewModel> Files { get; }
    }
}
