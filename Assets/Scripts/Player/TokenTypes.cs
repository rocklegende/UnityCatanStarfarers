using System;
using System.Collections.Generic;
using com.onebuckgames.UnityStarFarers;

[Serializable]
public class SpacePortToken : Token, BuildableToken
{
    public SpacePortToken() : base("spaceport_token", false, new Cost(new Resource[] { new CarbonResource(), new CarbonResource(), new CarbonResource(), new FoodResource(), new FoodResource() }))
    {
    }

    protected override void OnSettle()
    {
        //
    }

    public override int BaseVictoryPoints()
    {
        return 1;
    }

    public override Token makeCopy()
    {
        SpacePortToken newToken = new SpacePortToken();
        newToken.position = position;
        newToken.cost = cost;
        newToken.id = id;
        newToken.color = color;
        newToken.attachedToken = attachedToken;
        newToken.isTokenAttachable = isTokenAttachable;
        return newToken;
    }

    public override int ResourceProduce()
    {
        return 1;
    }



    public bool CanBeBuildByPlayer(Player player, Map map, Player[] players)
    {
        if (!PlayerHasTokenInStorageAndCanPay(player))
        {
            return false;
        }

        var filters = new List<SpacePointFilter> {
            new IsValidSpacePointFilter(),
            new HasSettledColonySpacePointFilter()
        };

        if (map.IsNotNull())
        {
            if (map.GetSpacePointsFullfillingFilters(filters, players).Count == 0)
            {
                return false;
            }
        }

        return true;
    }
}

[Serializable]
public abstract class BuildCondition
{
    public abstract bool TokenCanBeBuildByPlayer(Token token, Player player, Map map, Player[] players);
}

[Serializable]
public class TradeAndColonyBuildCondition : BuildCondition
{
    public override bool TokenCanBeBuildByPlayer(Token token, Player player, Map map, Player[] players)
    {
        if (!token.PlayerHasTokenInStorageAndCanPay(player))
        {
            return false;
        }

        if (player.tokenStorage.GetTokensOfType(new ShipToken().GetType()).Length == 0)
        {
            return false;
        }

        var filters = new List<SpacePointFilter> {
                new IsValidSpacePointFilter(),
                new IsSpacePointFreeFilter(),
                new IsNeighborOwnSpacePortFilter(player)
            };

        if (map.IsNotNull())
        {
            if (map.GetSpacePointsFullfillingFilters(filters, players).Count == 0)
            {
                return false;
            }
        }

        return true;
    }
}

[Serializable]
public class ColonyBaseToken : Token, Settable, BuildableToken
{
    BuildCondition buildCondition;
    public ColonyBaseToken() : base("colonybase_token", true, new Cost(new Resource[] { new FuelResource(), new OreResource(), new CarbonResource(), new FoodResource() }))
    {
        buildCondition = new TradeAndColonyBuildCondition();
    }

    public override int BaseVictoryPoints()
    {
        return 1;
    }

    public bool CanBeBuildByPlayer(Player player, Map map, Player[] players)
    {
        return buildCondition.TokenCanBeBuildByPlayer(this, player, map, players);
    }

    public bool CanSettle(Tile_[] tiles)
    {
        int count = 0;
        foreach (var tile in tiles)
        {
            if (tile is ResourceTile)
            {
                count += 1;
            }
        }
        return count == 2;
    }

    public override Token makeCopy()
    {
        ColonyBaseToken newToken = new ColonyBaseToken();
        newToken.position = position;
        newToken.cost = cost;
        newToken.id = id;
        newToken.color = color;
        newToken.attachedToken = attachedToken;
        newToken.isTokenAttachable = isTokenAttachable;
        return newToken;
    }

    protected override void OnSettle()
    {
        if (attachedToken != null)
        {
            owner.tokenStorage.AddToken(attachedToken);
            attachedToken = null;
        }
    }

    public override int ResourceProduce()
    {
        return 1;
    }
}

[Serializable]
public class TradeBaseToken : Token, Settable, BuildableToken
{
    BuildCondition buildCondition;
    public TradeBaseToken() : base("tradebase_token", true, new Cost(new Resource[] { new OreResource(), new FuelResource(), new GoodsResource(), new GoodsResource() }))
    {
        buildCondition = new TradeAndColonyBuildCondition();
    }

    public override int BaseVictoryPoints()
    {
        return 1;
    }

    public bool CanBeBuildByPlayer(Player player, Map map, Player[] players)
    {
        return buildCondition.TokenCanBeBuildByPlayer(this, player, map, players);
    }

    public bool CanSettle(Tile_[] tiles)
    {
        int count = 0;
        foreach (var tile in tiles)
        {
            if (tile is TradeStationTile)
            {
                count += 1;
            }
        }
        return count == 3;
    }

    public override Token makeCopy()
    {
        TradeBaseToken newToken = new TradeBaseToken();
        newToken.position = position;
        newToken.cost = cost;
        newToken.id = id;
        newToken.color = color;
        newToken.attachedToken = attachedToken;
        newToken.isTokenAttachable = isTokenAttachable;
        return newToken; ;
    }

    protected override void OnSettle()
    {
        if (attachedToken != null)
        {
            owner.tokenStorage.AddToken(attachedToken);
            attachedToken = null;
        }
    }

    public override int ResourceProduce()
    {
        return 0;
    }
}

[Serializable]
public class ShipToken : Token
{
    public ShipToken() : base("ship_token", false, new Cost(new Resource[] { }))
    {
    }

    public override int BaseVictoryPoints()
    {
        return 0;
    }

    public override Token makeCopy()
    {
        ShipToken newToken = new ShipToken();
        newToken.position = position;
        newToken.cost = cost;
        newToken.id = id;
        newToken.color = color;
        newToken.attachedToken = attachedToken;
        newToken.isTokenAttachable = isTokenAttachable;
        return newToken;
    }

    protected override void OnSettle()
    {
        throw new NotImplementedException();
    }

    public override int ResourceProduce()
    {
        return 0;
    }
}

[Serializable]
public class BoosterUpgradeToken : Token, BuildableToken
{
    public BoosterUpgradeToken() : base("booster_upgrade", false, new Cost(new Resource[] { new FuelResource(), new FuelResource() }))
    {

    }

    public override int BaseVictoryPoints()
    {
        return 0;
    }

    public override Token makeCopy()
    {
        throw new NotImplementedException();
    }

    protected override void OnSettle()
    {
        throw new NotImplementedException();
    }

    public override int ResourceProduce()
    {
        return 0;
    }

    public bool CanBeBuildByPlayer(Player player, Map map, Player[] players)
    {
        if (!player.hand.CanPayCost(cost))
        {
            return false;
        }

        if (player.ship.IsBoostersFull())
        {
            return false;
        }

        return true;
    }
}

[Serializable]
public class CannonUpgradeToken : Token, BuildableToken
{
    public CannonUpgradeToken() : base("cannon_upgrade", false, new Cost(new Resource[] { new CarbonResource(), new CarbonResource() }))
    {

    }

    public override int BaseVictoryPoints()
    {
        return 0;
    }

    public override Token makeCopy()
    {
        throw new NotImplementedException();
    }

    protected override void OnSettle()
    {
        throw new NotImplementedException();
    }

    public override int ResourceProduce()
    {
        return 0;
    }

    public bool CanBeBuildByPlayer(Player player, Map map, Player[] players)
    {
        if (!player.hand.CanPayCost(cost))
        {
            return false;
        }

        if (player.ship.IsCannonsFull())
        {
            return false;
        }

        return true;
    }
}

[Serializable]
public class FreightPodUpgradeToken : Token, BuildableToken
{
    public FreightPodUpgradeToken() : base("freight_pod_upgrade", false, new Cost(new Resource[] { new OreResource(), new OreResource() }))
    {

    }

    public override int BaseVictoryPoints()
    {
        return 0;
    }

    public override Token makeCopy()
    {
        throw new NotImplementedException();
    }

    protected override void OnSettle()
    {
        throw new NotImplementedException();
    }

    public override int ResourceProduce()
    {
        return 0;
    }

    public bool CanBeBuildByPlayer(Player player, Map map, Player[] players)
    {
        if (!player.hand.CanPayCost(cost))
        {
            return false;
        }

        if (player.ship.IsFreightPodsFull())
        {
            return false;
        }

        return true;
    }
}

