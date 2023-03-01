using DynamicData;
using FootprintViewer.Data.Caches;
using System.Reactive.Linq;

namespace FootprintViewer.UnitTests.Data;

public class CacheUnitTest
{
    private class Person
    {
        public Person(string name)
        {
            Name = name;
        }

        public string Name { get; init; } = string.Empty;
    }

    private readonly string _key = "key";
    private readonly string[] _subKeys = new[] { "subKey1", "subKey2", "subKey3" };

    private static Cache<string, string> CreateCache() => new();

    [Fact]
    public void Caching_Value_Types()
    {
        var cache = CreateCache();

        var arr1 = new[] { 1, 2 };
        var arr2 = new[] { 3, 4 };
        var arr3 = new[] { 5, 6 };
        var arr0 = Enumerable.Concat(arr1, arr2).Concat(arr3).ToArray();

        cache.Caching(_key, _subKeys[0], arr1.Cast<object>().ToList());
        cache.Caching(_key, _subKeys[1], arr2.Cast<object>().ToList());
        cache.Caching(_key, _subKeys[2], arr3.Cast<object>().ToList());

        var res1 = cache.GetValues<int>(_key).ToArray();
        var res2 = cache.GetValues<int>(_key, _subKeys[1]).ToArray();

        Assert.Equal(arr0, res1);
        Assert.Equal(arr2, res2);
    }

    [Fact]
    public void Caching_Reference_Types()
    {
        var cache = CreateCache();

        var arr1 = new[] { new Person("A"), new Person("B") };
        var arr2 = new[] { new Person("C"), new Person("D") };
        var arr3 = new[] { new Person("E"), new Person("F") };
        var arr0 = Enumerable.Concat(arr1, arr2).Concat(arr3).Cast<Person>().ToArray();

        cache.Caching(_key, _subKeys[0], arr1);
        cache.Caching(_key, _subKeys[1], arr2);
        cache.Caching(_key, _subKeys[2], arr3);

        var res1 = cache.GetValues<Person>(_key).ToArray();
        var res2 = cache.GetValues<Person>(_key, _subKeys[1]).ToArray();

        Assert.Equal(arr0, res1);
        Assert.Equal(arr2, res2);
    }

    [Fact]
    public void Clear_Cache()
    {
        var cache = CreateCache();

        var arr1 = new[] { 1, 2 };
        var arr2 = new[] { 3, 4 };
        var arr3 = new[] { 5, 6 };

        cache.Caching(_key, _subKeys[0], arr1.Cast<object>().ToList());
        cache.Caching(_key, _subKeys[1], arr2.Cast<object>().ToList());
        cache.Caching(_key, _subKeys[2], arr3.Cast<object>().ToList());

        cache.Clear(_key, new[] { _subKeys[0], _subKeys[2] });
        var res1 = cache.GetValues<int>(_key).ToArray();
        var hasKey1 = cache.ContainsKey(_key);

        Assert.Equal(2, res1.Length);
        Assert.True(hasKey1);

        cache.Clear(_key, new[] { _subKeys[1] });
        var res2 = cache.GetValues<int>(_key).ToArray();
        var hasKey2 = cache.ContainsKey(_key);

        Assert.Empty(res2);
        Assert.False(hasKey2);
    }

    [Fact]
    public void Recaching()
    {
        var cache = CreateCache();

        var arr1 = new[] { 1, 2 };
        var arr2 = new[] { 3, 4 };

        cache.Caching(_key, _subKeys[0], arr1.Cast<object>().ToList());
        var res1 = cache.GetValues<int>(_key).ToArray();
        Assert.Equal(arr1, res1);

        cache.Caching(_key, _subKeys[0], arr2.Cast<object>().ToList());
        var res2 = cache.GetValues<int>(_key).ToArray();
        Assert.Equal(arr2, res2);
    }

    [Fact]
    public void Init_Cache()
    {
        var cache = CreateCache();

        bool hasKey1 = cache.ContainsKey(_key);
        bool hasSubKey1 = cache.ContainsKeys(_key, _subKeys[0]);

        Assert.False(hasKey1);
        Assert.False(hasSubKey1);

        var arr2 = new[] { 1, 2 };

        cache.Caching(_key, _subKeys[1], arr2.Cast<object>().ToList());

        bool hasKey2 = cache.ContainsKey(_key);
        bool hasSubKey2 = cache.ContainsKeys(_key, _subKeys[0]);

        Assert.True(hasKey2);
        Assert.False(hasSubKey2);
    }
}