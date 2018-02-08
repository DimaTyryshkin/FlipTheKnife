namespace CodeWriter.Config
{
    public interface IConfig
    {
        int build { get; }
        string version { get; }
        string applicationName { get; }

        bool isDeveloperVersion { get; }
        bool isOldVersion { get; }

        string Get(string key);
    }
}