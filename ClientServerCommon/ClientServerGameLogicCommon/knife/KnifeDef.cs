using Assets.game.model.shared.price;
using OEPFramework;

namespace Assets.game.model.knife
{
    public class KnifeDef
    {
        public readonly string key;

        public string name;
        public KnifeRarity rarity;
        public KnifeMode[] modes;

        public KnifeMode mode;
        public float minForce;
        public float maxForce;
        public float gravity;
        public float timeScale;
        public float heightMultiplier;
        public float bounceMultiplier;
        public float rotationSpeed;
        public float rotationDecrease;
        public float rotationMinSpeed;
        public float successDeflectionLeft;
        public float successDeflectionRight;
        public float perfectDeflectionLeft;
        public float perfectDeflectionRight;
        public string prefab;
        public float scale;
        public int sides;

        public PriceDef price;

        public bool visibleInCollection;
        public bool visibleInRoulette;
        public bool droppableFromRoulette;

        public string skillTooltip;
        public string skillIcon;

        public bool hasSkillSuccessDeflection;
        public float skillSuccessDeflectionLeft;
        public float skillSuccessDeflectionRight;

        public bool hasSkillPerfectDeflection;
        public float skillPerfectDeflectionLeft;
        public float skillPerfectDeflectionRight;

        public float? skillPefectFlipGoldProbability;
        public int skillPefectFlipGoldMin;
        public int skillPefectFlipGoldMax;

        public string skillAdsIcon;
        public float? skillAdsMultiplier;

        public string preview
        {
            get { return key; }
        }

        public KnifeDef()
        {

        }

        public KnifeDef(string key, KnifeMode mode, RawNode config)
        {
            this.key = key;
            this.mode = mode;

            name = config.GetString("name");
            rarity = KnifeUtils.GetRarity(config.GetString("rarity", "common"));
            modes = KnifeUtils.GetSupportedModes(config);
            timeScale = config.GetFloat("time_scale");
            minForce = config.GetFloat("knife_min_force");
            maxForce = config.GetFloat("knife_max_force");
            gravity = config.GetFloat("gravity");
            bounceMultiplier = config.GetFloat("knife_bounce_multiplier");
            heightMultiplier = config.GetFloat("height_multiplier");
            rotationSpeed = config.GetFloat("rotation_speed");
            rotationDecrease = config.GetFloat("rotation_decrease");
            rotationMinSpeed = config.GetFloat("rotation_min_speed");

            prefab = config.GetString("prefab");
            scale = config.GetFloat("scale");
            sides = config.GetInt("sides", 1);

            price = config.CheckKey("price") ? new PriceDef(config.GetNode("price")) : PriceDef.NewNone();

            visibleInCollection = config.GetBool("collection_visible", true);
            visibleInRoulette = config.GetBool("roulette_visible", true);
            droppableFromRoulette = config.GetBool("roulette_drop", true);

            ReadDeflection(config, "success_deflection", out successDeflectionLeft, out successDeflectionRight);
            ReadDeflection(config, "perfect_success_deflection", out perfectDeflectionLeft, out perfectDeflectionRight);

            hasSkillSuccessDeflection = HasDeflection(config, "skill_success_deflection");
            if (hasSkillSuccessDeflection)
            {
                ReadDeflection(config, "skill_success_deflection", out skillSuccessDeflectionLeft, out skillSuccessDeflectionRight);
            }

            hasSkillPerfectDeflection = HasDeflection(config, "skill_perfect_deflection");
            if (hasSkillPerfectDeflection)
            {
                ReadDeflection(config, "skill_perfect_deflection", out skillPerfectDeflectionLeft, out skillPerfectDeflectionRight);
            }

            if (config.CheckKey("skill_pefect_flip_gold_probability"))
            {
                skillPefectFlipGoldProbability = config.GetFloat("skill_pefect_flip_gold_probability");
                skillPefectFlipGoldMin = config.GetInt("skill_pefect_flip_gold_min", 0);
                skillPefectFlipGoldMax = config.GetInt("skill_pefect_flip_gold_max", 0);
            }

            if (config.CheckKey("skill_ads_multiplier"))
                skillAdsMultiplier = config.GetFloat("skill_ads_multiplier", 1);

            skillTooltip = config.GetString("skill_tooltip", null);
            skillIcon = config.GetString("skill_icon", null);
            skillAdsIcon = config.GetString("skill_ads_icon", null);
        }

        private static void ReadDeflection(RawNode config, string name, out float left, out float right)
        {
            var deflection = config.GetFloat(name);
            left = config.GetFloat(name + "_left", deflection);
            right = config.GetFloat(name + "_right", deflection);
        }

        private static bool HasDeflection(RawNode config, string name)
        {
            return config.CheckKey(name) || config.CheckKey(name + "_left") || config.CheckKey(name + "_right");
        }
    }
}