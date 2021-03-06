using System;
public class PayoutHandler
{
    Map mapModel;
    public PayoutHandler(Map mapModel)
    {
        this.mapModel = mapModel;
    }

    Hand GetBasicPayoutForPlayer(Player player, DiceThrow dt) {
        Hand payout = new Hand();
        foreach (Token token in player.tokens)
        {
            Tile_[] tiles = mapModel.getTilesAtPoint(token.position);
            ResourceTile[] resourceTiles = mapModel.GetTilesOfType<ResourceTile>(tiles);
            foreach (ResourceTile resourceTile in resourceTiles)
            {
                DiceChip diceChip = resourceTile.diceChip;
                if (diceChip.isFaceUp && diceChip.fulfillsDiceThrow(dt.GetValue()))
                {
                    var helper = new Helper();
                    var numResources = token.GetResourceProductionMultiplier();
                    for (int i = 0; i < numResources; i++)
                    {
                        ResourceCard resourceCard = helper.CreateResourceCardFromResource(resourceTile.resource);
                        payout.AddCard(resourceCard);
                    }
                }
            }
        }
        return payout;
    }

    Hand GetBonusPayoutForPlayer(Hand payoutThatRound, Player player)
    {
        //TODO: needs goooood refactoring, please dont code this ugly alright?

        Hand bonusPayout = new Hand();

        var carbonBonus = player.GetBonusForResource(new CarbonResource());
        if (carbonBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<CarbonResource>() != null)
            {
                var h = new Helper().GetHandWithResources(0, 0, 0, 0, carbonBonus);
                bonusPayout.AddHand(h);
            }
        }

        var fuelBonus = player.GetBonusForResource(new FuelResource());
        if (fuelBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<FuelResource>() != null)
            {
                var h = new Helper().GetHandWithResources(0, 0, fuelBonus);
                bonusPayout.AddHand(h);
            }
        }

        var foodBonus = player.GetBonusForResource(new FoodResource());
        if (foodBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<FoodResource>() != null)
            {
                var h = new Helper().GetHandWithResources(foodBonus);
                bonusPayout.AddHand(h);
            }
        }

        var oreBonus = player.GetBonusForResource(new OreResource());
        if (oreBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<OreResource>() != null)
            {
                var h = new Helper().GetHandWithResources(0, 0, 0, oreBonus);
                bonusPayout.AddHand(h);
            }
        }

        var goodsBonus = player.GetBonusForResource(new GoodsResource());
        if (goodsBonus != 0)
        {
            if (payoutThatRound.FindCardOfResource<GoodsResource>() != null)
            {
                var h = new Helper().GetHandWithResources(0, goodsBonus);
                bonusPayout.AddHand(h);
            }
        }

        return bonusPayout;
    }

    public Hand GetPayoutForPlayer(Player player, DiceThrow dt)
    {
        var completePayout = new Hand();
        var basic = GetBasicPayoutForPlayer(player, dt);
        var bonus = GetBonusPayoutForPlayer(basic, player);

        completePayout.AddHand(bonus);
        completePayout.AddHand(basic);

        return completePayout;
    }
}
