using OEPFramework;

namespace Assets.game.model.shared.price
{
    public class PriceDef
    {
        public PriceType type;
        public int amount;

        public static PriceDef NewReal()
        {
            return new PriceDef { type = PriceType.Real };
        }

        public static PriceDef NewNone()
        {
            return new PriceDef { type = PriceType.None };
        }

        private PriceDef()
        { }

        public PriceDef(RawNode config)
        {
            type = PriceUtils.GetPriceType(config.GetString("type"));
            amount = config.GetInt("amount", 0);
        }
    }
}