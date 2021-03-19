using System;

public class TradeOutput
{
    int numCards;
    int tradingRatio;
    public TradeOutput(int numCards, int tradingRatio)
    {
        this.numCards = numCards;
        this.tradingRatio = tradingRatio;
    }

    public int GetCardsLeftAfterTrade()
    {
        return numCards % tradingRatio;
    }

    public int GetTradedCards()
    {
        return numCards / tradingRatio;
    }
}

public class TradingCalculator
{
    Player player;
    public TradingCalculator(Player player)
    {
        this.player = player;
    }

    bool IsTradeAllowed(Hand inputHand, Hand outputHand)
    {
        if (outputHand.Count() == 0)
        {
            return false; //no trade should be allowed where the output is zero
        }

        return true;
    }

    public bool PlayerTradeIsPossible(Hand inputHand, Hand outputHand)
    {
        if (!IsTradeAllowed(inputHand, outputHand))
        {
            return false; 
        }

        return true;
    }

    public bool BankTradeIsPossible(Hand inputHand, Hand outputHand)
    {
        if (!IsTradeAllowed(inputHand, outputHand))
        {
            return false;
        }

        var tradeOutputs = new TradeOutput[]
        {
            GetTradeOutputFromCarbon(inputHand),
            GetTradeOutputFromGoods(inputHand),
            GetTradeOutputFromFood(inputHand),
            GetTradeOutputFromFuel(inputHand),
            GetTradeOutputFromOre(inputHand)
        };

        int numTradedCards = 0;
        int cardsLeftAfterTrade = 0;

        foreach(var output in tradeOutputs)
        {
            numTradedCards += output.GetTradedCards();
            cardsLeftAfterTrade += output.GetCardsLeftAfterTrade();
        }

        if (cardsLeftAfterTrade > 0)
        {
            return false;
        }

        return numTradedCards == outputHand.Count();
    }

    public TradeOutput GetTradeOutputFromFood(Hand inputHand)
    {
        int numFoodCards = inputHand.NumberCardsOfType<FoodCard>();
        return new TradeOutput(numFoodCards, player.GetFoodTradingRatio());
    }

    public TradeOutput GetTradeOutputFromGoods(Hand inputHand)
    {
        int numGoodsCards = inputHand.NumberCardsOfType<GoodsCard>();
        return new TradeOutput(numGoodsCards, player.GetGoodsTradingRatio());
    }

    public TradeOutput GetTradeOutputFromOre(Hand inputHand)
    {
        int numOreCards = inputHand.NumberCardsOfType<OreCard>();
        return new TradeOutput(numOreCards, player.GetOreTradingRatio());
    }

    public TradeOutput GetTradeOutputFromFuel(Hand inputHand)
    {
        int numFuelCards = inputHand.NumberCardsOfType<FuelCard>();
        return new TradeOutput(numFuelCards, player.GetFuelTradingRatio());
    }

    public TradeOutput GetTradeOutputFromCarbon(Hand inputHand)
    {
        int numCarbonCards = inputHand.NumberCardsOfType<CarbonCard>();
        return new TradeOutput(numCarbonCards, player.GetCarbonTradingRatio());
    }
}
