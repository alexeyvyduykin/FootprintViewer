namespace FootprintViewer.AppStates;

public interface IConfig
{
    // Gets the path of the config file
    string FilePath { get; }

    // Set the path of the config file
    void SetFilePath(string path);

    // Throw exception if the path of the config file is not set
    void AssertFilePathSet();

    // Serialize the config if the file path of the config file is set, otherwise throw exception
    void ToFile();

    // Load config from configuration file
    void LoadFile(bool createIfMissing = false);

    bool AreDeepEqual(object otherConfig);

    // Check if the config file differs from the config if the file path of the config file is set, otherwise throw exception
    bool CheckFileChange();
}
