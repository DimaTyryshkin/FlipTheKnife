using OEPFramework;

namespace Assets.game.model.knife
{
    public class KnivesRepoDef
    {
        public KnifeDef[] knives;

        public KnivesRepoDef(RawNode config)
        {
            var dict = config.dictionary;

            knives = new KnifeDef[dict.Count];

            int index = 0;
            foreach (var item in dict)
            {
                knives[index++] = new KnifeDef(item.Key, KnifeMode.Medium, config.GetNode(item.Key));
            }
        }
    }
}