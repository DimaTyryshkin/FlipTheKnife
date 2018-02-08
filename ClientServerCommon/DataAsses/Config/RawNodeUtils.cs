using System;
using System.Text;
using System.Collections.Generic;
using OEPFramework;

namespace OEPFramework
{
    public static class RawNodeUtils
    {
        public static void Stringify(StringBuilder sb, string key, RawNode node)
        {
            Stringify(sb, 0, key, node.GetRawData());
        }

        public static void Stringify(StringBuilder sb, int offset, string key, object val)
        {
            if (val is Dictionary<string, object>)
            {
                sb.Append(new string(' ', offset)).AppendLine(key);
                foreach (var item in (Dictionary<string, object>)val)
                {
                    Stringify(sb, offset + 2, item.Key, item.Value);
                }
            }
            else
            {
                sb.Append(new string(' ', offset)).Append(key).Append(" : ").Append(val).AppendLine();
            }
        }
        
        public static RawNode FromJSON(string json)
        {
            return new RawNode(Json.Deserialize(json));
        }
    }
}