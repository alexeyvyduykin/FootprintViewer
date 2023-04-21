using System.Text.RegularExpressions;

namespace FootprintViewer.Data.DbContexts;

public class ConnectionString
{
    private ConnectionString() { }

    private ConnectionString(string connectionString)
    {
        var pattern = @"Host=(?<host>[^@]+);Port=(?<port>[\d]+);Database=(?<database>[^@]+);Username=(?<username>[^@]+);Password=(?<password>[^@]+)";

        var matches = Regex.Matches(connectionString, pattern);

        Host = matches.FirstOrDefault()?.Groups["host"].Value ?? string.Empty;
        var port = matches.FirstOrDefault()?.Groups["port"].Value ?? string.Empty;
        Port = int.Parse(port ?? string.Empty);
        Database = matches.FirstOrDefault()?.Groups["database"].Value ?? string.Empty;
        Username = matches.FirstOrDefault()?.Groups["username"].Value ?? string.Empty;
        Password = matches.FirstOrDefault()?.Groups["password"].Value ?? string.Empty;
    }

    public static ConnectionString Build(string host, int port, string database, string username, string password)
        => new()
        {
            Host = host,
            Port = port,
            Database = database,
            Username = username,
            Password = password
        };

    public static ConnectionString Build(string host, string database, string username, string password)
        => new()
        {
            Host = host,
            Database = database,
            Username = username,
            Password = password
        };

    public static ConnectionString Build(string connectionString)
        => new(connectionString);

    public string Host { get; private set; } = null!;

    public int Port { get; private set; } = 5432;

    public string Database { get; private set; } = null!;

    public string Username { get; private set; } = null!;

    public string Password { get; private set; } = null!;

    public override string ToString()
    {
        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
    }
}
