using System;
public class TradingCalculator
{
    Player player;
    public TradingCalculator(Player player)
    {
        this.player = player;
    }

    public bool IsOutputPossible(Hand inputHand, Hand outputHand)
    {
        int numTradedCards = 0;
        numTradedCards += GetTradeOutputFromCarbon(inputHand);
        numTradedCards += GetTradeOutputFromGoods(inputHand);
        numTradedCards += GetTradeOutputFromFood(inputHand);
        numTradedCards += GetTradeOutputFromFuel(inputHand);
        numTradedCards += GetTradeOutputFromOre(inputHand);
        return numTradedCards == outputHand.Count();
    }

    public int GetTradeOutputFromFood(Hand inputHand)
    {
        int numFoodCards = inputHand.NumberCardsOfType<FoodCard>();
        return numFoodCards / player.GetFoodTradingRatio();
    }

    public int GetTradeOutputFromGoods(Hand inputHand)
    {
        int numGoodsCards = inputHand.NumberCardsOfType<GoodsCard>();
        return numGoodsCards / player.GetGoodsTradingRatio();
    }

    public int GetTradeOutputFromOre(Hand inputHand)
    {
        int numOreCards = inputHand.NumberCardsOfType<OreCard>();
        return numOreCards / player.GetOreTradingRatio();
    }

    public int GetTradeOutputFromFuel(Hand inputHand)
    {
        int numFuelCards = inputHand.NumberCardsOfType<FuelCard>();
        return numFuelCards / player.GetFuelTradingRatio();
    }

    public int GetTradeOutputFromCarbon(Hand inputHand)
    {
        int numCarbonCards = inputHand.NumberCardsOfType<CarbonCard>();
        return numCarbonCards / player.GetCarbonTradingRatio();
    }
}
