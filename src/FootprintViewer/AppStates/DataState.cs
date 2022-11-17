using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FootprintViewer.AppStates;

[DataContract]
public class DataState<T> where T : IEquatable<T>
{
    private readonly Dictionary<string, IList<T>> _excludeValues = new();

    public DataState()
    {

    }

    public DataState(Dictionary<string, IList<T>> states)
    {
        States = new(states);
    }

    public void SetExcludeValues(Dictionary<string, List<T>> values)
    {
        _excludeValues.Clear();

        foreach (var key in values.Keys)
        {
            if (_excludeValues.ContainsKey(key) == false)
            {
                _excludeValues.Add(key, new List<T>());
            }

            foreach (var value in values[key])
            {
                var isExist = _excludeValues[key].Any(s => s.Equals(value));

                if (isExist == false)
                {
                    _excludeValues[key].Add(value);
                }
            }
        }
    }

    public IDictionary<string, IList<T>> GetValues()
    {
        var dict = new Dictionary<string, IList<T>>();

        var keys = States.Keys;

        foreach (var key in keys)
        {
            var hasKey = _excludeValues.ContainsKey(key);

            foreach (var source in States[key])
            {
                var isExist = false;

                if (hasKey == true)
                {
                    isExist = _excludeValues[key].Contains(source);
                }

                if (isExist == false)
                {
                    if (dict.ContainsKey(key) == false)
                    {
                        dict.Add(key, new List<T>());
                    }

                    dict[key].Add(source);
                }
            }
        }

        return dict;
    }

    public void UpdateValues(Dictionary<string, List<T>> values)
    {
        var dict = new Dictionary<string, IList<T>>();

        foreach (var key in values.Keys)
        {
            var hasKey = _excludeValues.ContainsKey(key);

            foreach (var item in values[key])
            {
                if (hasKey == true)
                {
                    if (_excludeValues[key].Contains(item) == true)
                    {
                        continue;
                    }
                }

                if (dict.ContainsKey(key) == false)
                {
                    dict.Add(key, new List<T>());
                }

                if (dict[key].Contains(item) == false)
                {
                    dict[key].Add(item);
                }
            }
        }

        States = new Dictionary<string, IList<T>>(dict);
    }

    [DataMember]
    private Dictionary<string, IList<T>> States { get; set; } = new();
}
