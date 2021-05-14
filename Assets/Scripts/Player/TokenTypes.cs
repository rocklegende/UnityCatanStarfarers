using System;
using System.Collections.Generic;
using UnityEngine;
using com.onebuckgames.UnityStarFarers;

[Serializable]
public class SpacePortToken : Token, BuildableToken
{
    BuildCondition buildCondition;
    public SpacePortToken() : base("spaceport_token", false, new Cost(new Resource[] { new CarbonResource(), new CarbonResource(), new CarbonResource(), new FoodResource(), new FoodResource() }))
    {
        buildCondition = new SpacePortBuildCondition();
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



    public bool CanBeBuildByPlayer(Player player, Map map)
    {
        return buildCondition.TokenCanBeBuildByPlayer(this, player, map);
    }

    public List<SpacePoint> GetPossibleBuildSpots(Player player, Map map)
    {
        var filters = new List<SpacePointFilter> {
            new IsValidSpacePointFilter(),
            new IsOwnSettledColonySpacePointFilter(player)
        };

        return map.GetSpacePointsFullfillingFilters(filters);
    }
}

[Serializable]
public abstract class BuildCondition
{
    public abstract bool TokenCanBeBuildByPlayer(Token token, Player player, Map map);
}

[Serializable]
public class TradeAndColonyBuildCondition : BuildCondition
{
    public override bool TokenCanBeBuildByPlayer(Token token, Player player, Map map)
    {
        if (!token.PlayerHasTokenInStorageAndCanPay(player))
        {
            return false;
        }

        var hasNoShipsLeft = player.tokenStorage.GetTokensOfType(new ShipToken().GetType()).Length == 0;
        if (hasNoShipsLeft)
        {
            return false;
        }

        if (token is BuildableToken)
        {
            var buildableToken = (BuildableToken)token;
            if (map.IsNotNull())
            {
                var buildspots = buildableToken.GetPossibleBuildSpots(player, map);
                if (buildableToken.GetPossibleBuildSpots(player, map).Count == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

[Serializable]
public class SpacePortBuildCondition : BuildCondition
{
    public override bool TokenCanBeBuildByPlayer(Token token, Player player, Map map)
    {
        if (!token.PlayerHasTokenInStorageAndCanPay(player))
        {
            return false;
        }

        var filters = new List<SpacePointFilter> {
            new IsValidSpacePointFilter(),
            new IsOwnSettledColonySpacePointFilter(player)
        };

        if (map.IsNotNull())
        {
            if (map.GetSpacePointsFullfillingFilters(filters).Count == 0)
            {
                return false;
            }
        }

        return true;
    }
}


[Serializable]
public class ColonyBaseToken : Token, BuildableToken
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

    public bool CanBeBuildByPlayer(Player player, Map map)
    {
        return buildCondition.TokenCanBeBuildByPlayer(this, player, map);
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
            if (!owner.tokens.Contains(this))
            {
                Debug.Log("NOT OWNER");
                //throw new ArgumentException();
            }
            owner.tokenStorage.AddToken(attachedToken);
            attachedToken = null;
        }
    }

    public override int ResourceProduce()
    {
        return 1;
    }

    public List<SpacePoint> GetPossibleBuildSpots(Player player, Map map)
    {
        var filters = new List<SpacePointFilter> {
            new IsValidSpacePointFilter(),
            new IsSpacePointFreeFilter(),
            new IsNeighborOwnSpacePortFilter(player)
        };
        return map.GetSpacePointsFullfillingFilters(filters);
    }
}

[Serializable]
public class TradeBaseToken : Token, BuildableToken
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

    public bool CanBeBuildByPlayer(Player player, Map map)
    {
        return buildCondition.TokenCanBeBuildByPlayer(this, player, map);
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

    public List<SpacePoint> GetPossibleBuildSpots(Player player, Map map)
    {
        var filters = new List<SpacePointFilter> {
            new IsValidSpacePointFilter(),
            new IsSpacePointFreeFilter(),
            new IsNeighborOwnSpacePortFilter(player)
        };
        return map.GetSpacePointsFullfillingFilters(filters);
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
public class BoosterUpgradeToken : Upgrade, BuildableToken
{
    public BoosterUpgradeToken() : base("booster_upgrade", new Cost(new Resource[] { new FuelResource(), new FuelResource() }))
    {

    }

    public override bool CanBeBuildByPlayer(Player player, Map map)
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

    public List<SpacePoint> GetPossibleBuildSpots(Player player, Map map)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class CannonUpgradeToken : Upgrade, BuildableToken
{
    public CannonUpgradeToken() : base("cannon_upgrade", new Cost(new Resource[] { new CarbonResource(), new CarbonResource() }))
    {

    }

    public override bool CanBeBuildByPlayer(Player player, Map map)
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

    public List<SpacePoint> GetPossibleBuildSpots(Player player, Map map)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public abstract class Upgrade : SFModel
{
    public string id { get; }
    public Cost cost { get; }

    public Upgrade(string id, Cost cost)
    {
        this.id = id;
        this.cost = cost;
    }

    public abstract bool CanBeBuildByPlayer(Player player, Map map);
}

[Serializable]
public class FreightPodUpgradeToken : Upgrade, BuildableToken
{
    public FreightPodUpgradeToken() : base("freight_pod_upgrade", new Cost(new Resource[] { new OreResource(), new OreResource() }))
    {

    }

    public override bool CanBeBuildByPlayer(Player player, Map map)
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

    public List<SpacePoint> GetPossibleBuildSpots(Player player, Map map)
    {
        throw new NotImplementedException();
    }
}

