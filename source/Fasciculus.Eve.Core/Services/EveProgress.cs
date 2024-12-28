using Fasciculus.Maui.Support.Progressing;

namespace Fasciculus.Eve.Services
{
    public class EveProgress
    {
        public ProgressBarProgress MarketPricesProgress { get; }
        public ProgressBarProgress BuyOrdersProgress { get; }
        public ProgressBarProgress SellOrdersProgress { get; }

        public ProgressBarProgress IndustryIndicesProgress { get; }

        public EveProgress()
        {
            MarketPricesProgress = new();
            BuyOrdersProgress = new();
            SellOrdersProgress = new();

            IndustryIndicesProgress = new();
        }
    }
}
