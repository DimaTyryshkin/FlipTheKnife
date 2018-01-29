namespace Assets.game.model.shared.price
{
    public static class PriceUtils
    {
        public static PriceType GetPriceType(string key)
        {
            switch (key)
            {
                case "none": return PriceType.None;
                case "real": return PriceType.Real;
                case "ads": return PriceType.Ads;
                default: throw new System.ArgumentOutOfRangeException("key");
            }
        }

        public static string GetPriceTypeKey(PriceType type)
        {
            switch (type)
            {
                case PriceType.None: return "none";
                case PriceType.Real: return "real";
                case PriceType.Ads: return "ads";
                default: throw new System.ArgumentOutOfRangeException("type");
            }
        }
    }
}