using FootprintViewer.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace FootprintViewer.AppStates;

public abstract class ConfigBase : IConfig
{
    protected ConfigBase()
    {
    }

    protected ConfigBase(string filePath)
    {
        SetFilePath(filePath);
    }

    protected object FileLock { get; } = new();

    public string FilePath { get; private set; } = "";

    public void AssertFilePathSet()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            throw new NotSupportedException($"{nameof(FilePath)} is not set. Use {nameof(SetFilePath)} to set it.");
        }
    }

    public bool CheckFileChange()
    {
        AssertFilePathSet();

        if (!File.Exists(FilePath))
        {
            throw new FileNotFoundException($"{GetType().Name} file did not exist at path: `{FilePath}`.");
        }

        lock (FileLock)
        {
            string jsonString = ReadFileNoLock();
            object newConfigObject = Activator.CreateInstance(GetType())!;
            JsonConvert.PopulateObject(jsonString, newConfigObject, JsonSerializationOptions.Default.Settings);

            return !AreDeepEqual(newConfigObject);
        }
    }

    public virtual void LoadFile(bool createIfMissing = false)
    {
        if (createIfMissing)
        {
            AssertFilePathSet();

            lock (FileLock)
            {
                JsonConvert.PopulateObject("{}", this);

                if (!File.Exists(FilePath))
                {
                    Logger.LogInfo($"{GetType().Name} file did not exist. Created at path: `{FilePath}`.");
                }
                else
                {
                    try
                    {
                        LoadFileNoLock();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogInfo($"{GetType().Name} file has been deleted because it was corrupted. Recreated default version at path: `{FilePath}`.");
                        Logger.LogWarning(ex);
                    }
                }

                ToFileNoLock();
            }
        }
        else
        {
            lock (FileLock)
            {
                LoadFileNoLock();
            }
        }
    }

    public void SetFilePath(string path)
    {
        FilePath = path;
    }

    public bool AreDeepEqual(object otherConfig)
    {
        var serializer = JsonSerializer.Create(JsonSerializationOptions.Default.Settings);
        var currentConfig = JObject.FromObject(this, serializer);
        var otherConfigJson = JObject.FromObject(otherConfig, serializer);
        return JToken.DeepEquals(otherConfigJson, currentConfig);
    }

    public void ToFile()
    {
        lock (FileLock)
        {
            ToFileNoLock();
        }
    }

    protected void LoadFileNoLock()
    {
        string jsonString = ReadFileNoLock();

        JsonConvert.PopulateObject(jsonString, this, JsonSerializationOptions.Default.Settings);
    }

    protected void ToFileNoLock()
    {
        AssertFilePathSet();

        string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented, JsonSerializationOptions.Default.Settings);
        WriteFileNoLock(jsonString);
    }

    protected void WriteFileNoLock(string contents)
    {
        File.WriteAllText(FilePath, contents, Encoding.UTF8);
    }

    protected string ReadFileNoLock()
    {
        return File.ReadAllText(FilePath, Encoding.UTF8);
    }
}
