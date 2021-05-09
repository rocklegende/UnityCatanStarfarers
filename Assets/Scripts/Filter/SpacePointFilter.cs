using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using com.onebuckgames.UnityStarFarers;

public abstract class SpacePointFilter
{
    public abstract bool pointFulfillsFilter(SpacePoint point, Map map);    
}

public class TradeshipSpacePointFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map)
    {
        Tile_[] tiles = map.getTilesAtPoint(point);
        var t = map.GetTilesOfType<ResourceTile>(tiles);
        return t.Length == 2;
    }
}

public class IsSpacePointFreeFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map)
    {
        foreach (Token token in map.tokensOnMap)
        {
            if (token.position.IsEqualTo(point))
            {
                return false;
            }
        }
        return true;
    }
}

public class IsValidSpacePointFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map)
    {
        var tiles = map.getTilesAtPoint(point);
        ResourceTile[] resourceTiles = map.GetTilesOfType<ResourceTile>(tiles);
        if (resourceTiles.Length == 3)
        {
            return false;
        } else
        {
            return true;
        }

    }
}

public class IsNeighborOwnSpacePortFilter : SpacePointFilter
{
    Player mainPlayer;
    public IsNeighborOwnSpacePortFilter(Player mainPlayer)
    {
        this.mainPlayer = mainPlayer;
    }

    public override bool pointFulfillsFilter(SpacePoint point, Map map)
    {
        List<SpacePoint> neighborPoints = map.GetNeighborsOfSpacePoint(point);
        foreach (SpacePoint neighbor in neighborPoints)
        {
            foreach (Token token in map.tokensOnMap)
            {
                if (token.position.IsEqualTo(neighbor))
                {
                    if (token.attachedToken is SpacePortToken && token.owner == mainPlayer)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

public class IsOwnSettledColonySpacePointFilter : SpacePointFilter
{
    Player mainPlayer;
    public IsOwnSettledColonySpacePointFilter(Player mainPlayer)
    {
        this.mainPlayer = mainPlayer;
    }

    public override bool pointFulfillsFilter(SpacePoint point, Map map)
    {
        foreach (Token token in map.tokensOnMap)
        {
            if (token.position.IsEqualTo(point) && token is ColonyBaseToken && token.attachedToken.IsNull() && token.owner == mainPlayer)
            {
                return true;
            }
        }
        return false;
    }
}

public class IsStepsAwayFilter : SpacePointFilter
{
    SpacePoint origin;
    int steps;
    public IsStepsAwayFilter(SpacePoint origin, int steps)
    {
        this.origin = origin;
        this.steps = steps;
    }
    public override bool pointFulfillsFilter(SpacePoint point, Map map)
    {
        return map.GetSpacePointsInsideRange(origin, steps).Contains(point);
    }
}

public class IsExactlyStepsAwayAndCannotSettleOnPointCounter : SpacePointFilter
{
    Token token;
    int exact_steps;
    public IsExactlyStepsAwayAndCannotSettleOnPointCounter(Token token, int exact_steps)
    {
        this.token = token;
        this.exact_steps = exact_steps;
    }
    public override bool pointFulfillsFilter(SpacePoint point, Map map)
    {
        var isExactlyStepsAway = map.GetSpacePointsInsideRange(token.position, exact_steps, exact_steps).Contains(point);
        if (!isExactlyStepsAway)
        {
            return true;
        }

        var tileGroup = map.FindTileGroupAtPoint(point);
        if (tileGroup != null)
        {
            var isOnSettleSpot = tileGroup.GetSettlePoints().Contains(point);
            if (!isOnSettleSpot)
            {
                return true;
            }
        } else
        {
            return true;
        }
        


        var tokenCanSettle = map.TokenCanSettle(token, point);
        if (tokenCanSettle)
        {
            return true;
        }

        return false;
    }
}

public class IsNeighborOfOwnSpacePortOrNotExactlyStepsAway : SpacePointFilter
{
    Token token;
    Player player;
    int exactSteps;
    public IsNeighborOfOwnSpacePortOrNotExactlyStepsAway(Token token, Player player, int exactSteps)
    {
        this.player = player;
        this.exactSteps = exactSteps;
        this.token = token;
    }

    public override bool pointFulfillsFilter(SpacePoint point, Map map)
    {
        var isExactlyStepsAway = map.GetSpacePointsInsideRange(token.position, exactSteps, exactSteps).Contains(point);
        if (!isExactlyStepsAway)
        {
            return true;
        }
        var neighbors = map.GetNeighborsOfSpacePoint(point);
        foreach (var neighbor in neighbors)
        {
            var token = map.TokenAtPoint(neighbor);
            if (token != null)
            {
                if (token.owner == player)
                {
                    if (token.attachedToken != null)
                    {
                        if (token.attachedToken is SpacePortToken)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}