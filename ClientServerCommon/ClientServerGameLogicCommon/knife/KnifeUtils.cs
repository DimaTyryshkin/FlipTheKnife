using System;
using OEPFramework;

namespace Assets.game.model.knife
{
    public static class KnifeUtils
    {
        private static readonly KnifeRarity[] m_allKnifeRarities = (KnifeRarity[])Enum.GetValues(typeof(KnifeRarity));
        private static readonly KnifeMode[] m_allKnifeModes = (KnifeMode[])Enum.GetValues(typeof(KnifeMode));

        public static KnifeMode[] AllKnifeModes
        {
            get { return m_allKnifeModes; }
        }

        public static KnifeRarity[] AllKnifeRarities
        {
            get { return m_allKnifeRarities; }
        }

        public static KnifeRarity GetRarity(string rarityStr)
        {
            switch (rarityStr)
            {
                case "common": return KnifeRarity.Common;
                case "rare": return KnifeRarity.Rare;
                case "epic": return KnifeRarity.Epic;

                default:
                    throw new ArgumentException(rarityStr, "rarityStr");
            }
        }

        public static string GetRarityKey(KnifeRarity rarity)
        {
            switch (rarity)
            {
                case KnifeRarity.Common: return "common";
                case KnifeRarity.Rare: return "rare";
                case KnifeRarity.Epic: return "epic";
                default:
                    throw new ArgumentOutOfRangeException("Undefined rarity " + rarity, "rarity");
            }
        }

        public static string GetKnifeModeKey(KnifeMode knifeMode)
        {
            switch (knifeMode)
            {
                case KnifeMode.Slow:
                    return "slow";

                case KnifeMode.Medium:
                    return "medium";

                case KnifeMode.Fast:
                    return "fast";

                case KnifeMode.VeryFast:
                    return "veryfast";

                default:
                    throw new ArgumentException("Undefined KnifeMode " + knifeMode, "knifeMode");
            }
        }

        public static KnifeMode[] GetSupportedModes(RawNode knifeNode)
        {
            var modesNode = knifeNode.GetNode("modes");
            var supportedModes = new KnifeMode[AllKnifeModes.Length];

            int count = 0;
            foreach (var mode in AllKnifeModes)
            {
                var key = GetKnifeModeKey(mode);
                if(modesNode.CheckKey(key))
                {
                    supportedModes[count++] = mode;
                }
            }

            Array.Resize(ref supportedModes, count);
            return supportedModes;
        }
    }
}