using OEPFramework;

namespace CodeWriter.Config
{
    public static class RawNodeConfigExtensions
    {
        public static RawNode GetRawNode(this IConfig config, string key)
        {
            var text = config.Get(key);
            var json = RawNodeUtils.FromJSON(text);
            return json;
        } 
    }
}