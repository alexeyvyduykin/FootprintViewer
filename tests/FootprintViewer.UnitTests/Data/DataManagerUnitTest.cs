using FootprintViewer.Data;
using FootprintViewer.Data.DbContexts;
using ReactiveUI;
using System.Reactive.Linq;

namespace FootprintViewer.UnitTests.Data;

public class DataManagerUnitTest
{
    private readonly string _key = "key";
    private readonly string _key2 = "key2";

    private class TestSource : ISource
    {
        private readonly List<int> _list;

        public TestSource(IList<int> list)
        {
            _list = new List<int>(list);
        }

        public async Task<IList<object>> GetValuesAsync()
        {
            var res = _list.Cast<object>().ToList() ?? new List<object>();
            return await Observable.Return(res);
        }
    }

    private class TestEditableSource : IEditableSource
    {
        private readonly List<int> _list;

        public TestEditableSource(IList<int> list)
        {
            _list = new List<int>(list);
        }

        public async Task AddAsync(string key, object value)
        {
            await Observable.Start(() => _list.Add((int)value), RxApp.TaskpoolScheduler);
        }

        public async Task EditAsync(string key, string id, object newValue)
        {
            await Observable.Start(() =>
            {
                int index = int.Parse(id);
                _list[index] = (int)newValue;
            }, RxApp.TaskpoolScheduler);
        }

        public async Task<IList<object>> GetValuesAsync()
        {
            var res = _list.Cast<object>().ToList() ?? new List<object>();
            return await Observable.Return(res);
        }

        public async Task RemoveAsync(string key, object value)
        {
            await Observable.Start(() => _list.Remove((int)value), RxApp.TaskpoolScheduler);
        }
    }

    [Fact]
    public void Get_Data()
    {
        var dataManager = new DataManager();

        var source1 = new TestSource(new[] { 1, 2 });
        var source2 = new TestSource(new[] { 3, 4 });
        var source3 = new TestSource(new[] { 5, 6 });
        var sources = new[] { source1, source2, source3 };
        var res = Task.WhenAll(sources.Select(s => s.GetValuesAsync())).Result.SelectMany(s => s.Select(s => (int)s)).ToList() ?? new();

        dataManager.RegisterSource(_key, source1);
        dataManager.RegisterSource(_key, source2);
        dataManager.RegisterSource(_key, source3);

        var res1 = dataManager.GetDataAsync<int>(_key).Result;
        Assert.Equal(res, res1);
    }

    [Fact]
    public void Add_Data()
    {
        var dataManager = new DataManager();

        var source1 = new TestEditableSource(new[] { 1, 2 });

        dataManager.RegisterSource(_key, source1);

        var res1 = dataManager.GetDataAsync<int>(_key).Result;
        Assert.Equal(new[] { 1, 2 }, res1);

        _ = dataManager.TryAddAsync(_key, 3).Result;
        var res2 = dataManager.GetDataAsync<int>(_key).Result;
        Assert.Equal(new[] { 1, 2, 3 }, res2);
    }

    [Fact]
    public void Remove_Data()
    {
        var dataManager = new DataManager();

        var source1 = new TestEditableSource(new[] { 1, 2 });

        dataManager.RegisterSource(_key, source1);

        var res1 = dataManager.GetDataAsync<int>(_key).Result;
        Assert.Equal(new[] { 1, 2 }, res1);

        _ = dataManager.TryRemoveAsync(_key, 1).Result;
        var res2 = dataManager.GetDataAsync<int>(_key).Result;
        Assert.Equal(new[] { 2 }, res2);
    }

    [Fact]
    public void Edit_Data()
    {
        var dataManager = new DataManager();

        var source1 = new TestEditableSource(new[] { 1, 2 });

        dataManager.RegisterSource(_key, source1);

        var res1 = dataManager.GetDataAsync<int>(_key).Result;
        Assert.Equal(new[] { 1, 2 }, res1);

        _ = dataManager.TryEditAsync(_key, "0", 2).Result;
        var res2 = dataManager.GetDataAsync<int>(_key).Result;
        Assert.Equal(new[] { 2, 2 }, res2);
    }

    [Fact]
    public void Signal_DataChanged()
    {
        var dataManager = new DataManager();

        var source1 = new TestSource(new[] { 1, 2 });
        var source2 = new TestEditableSource(new[] { 1, 2 });

        dataManager.RegisterSource(_key, source1);
        dataManager.RegisterSource(_key2, source2);

        var dirtyKeys = Array.Empty<string>();

        dataManager.DataChanged.Subscribe(s => dirtyKeys = s);

        _ = dataManager.TryEditAsync(_key2, "0", 2).Result;

        Assert.DoesNotContain(_key, dirtyKeys);
        Assert.Contains(_key2, dirtyKeys);
    }
}
