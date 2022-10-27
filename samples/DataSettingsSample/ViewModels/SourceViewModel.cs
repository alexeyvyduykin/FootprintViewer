using DataSettingsSample.Data;
using DataSettingsSample.ViewModels.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace DataSettingsSample.ViewModels
{
    public abstract class SourceViewModel : ReactiveObject, ISourceViewModel
    {
        [Reactive]
        public string Name { get; set; } = "Default";
    }

    public class CustomSourceViewModel : SourceViewModel
    {
        private readonly ISource _source;

        public CustomSourceViewModel(ISource source)
        {
            _source = source;
        }

        public ISource Source => _source;
    }

    //public class DatabaseSourceViewModel : SourceViewModel, IDatabaseSourceViewModel
    //{
    //    private readonly string _host;
    //    private readonly int _port;
    //    private readonly string _database;
    //    private readonly string _username;
    //    private readonly string _password;
    //    private readonly string _table;

    //    public DatabaseSourceViewModel(string host, int port, string database, string username, string password, string table)
    //    {
    //        _host = host;
    //        _port = port;
    //        _database = database;
    //        _username = username;
    //        _password = password;
    //        _table = table;
    //    }

    //    public string Host => _host;

    //    public int Port => _port;

    //    public string Database => _database;

    //    public string Username => _username;

    //    public string Password => _password;

    //    public string Table => _table;
    //}

    //public class JsonSourceViewModel : SourceViewModel, IJsonSourceViewModel
    //{
    //    private readonly IReadOnlyList<FileViewModel> _files;

    //    public JsonSourceViewModel(IList<FileViewModel> files)
    //    {
    //        _files = files.ToImmutableList();
    //    }

    //    public IReadOnlyList<FileViewModel> Files => _files;
    //}
}
