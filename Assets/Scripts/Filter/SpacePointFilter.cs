using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using com.onebuckgames.UnityStarFarers;

public abstract class SpacePointFilter
{
    public abstract bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players);
    public List<Token> GetAllTokenOfPlayers(Player[] players)
    {
        return new Helper().GetAllTokenOfPlayers(players);
    }
}

public class TradeshipSpacePointFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        Tile_[] tiles = map.getTilesAtPoint(point);
        var t = map.GetTilesOfType<ResourceTile>(tiles);
        return t.Length == 2;
    }
}

public class IsSpacePointFreeFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        foreach(Token token in GetAllTokenOfPlayers(players))
        {
            if (token.position.Equals(point))
            {
                return false;
            }
        }
        return true;
    }
}

public class IsValidSpacePointFilter : SpacePointFilter
{
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
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
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        SpacePoint[] neighborPoints = map.GetNeighborsOfSpacePoint(point); //all neighbors
        foreach (SpacePoint neighbor in neighborPoints)
        {
            foreach (Token token in GetAllTokenOfPlayers(players))
            {
                if (token.position.Equals(neighbor))
                {
                    if (token.attachedToken is SpacePortToken)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

public class HasSettledColonySpacePointFilter : SpacePointFilter
{
    //TODO: we look at the token of every player here! we only want the own player
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        foreach (Token token in GetAllTokenOfPlayers(players))
        {
            if (token.position.Equals(point) && token is ColonyBaseToken && token.attachedToken.IsNull())
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
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        return new Helper().SpacePointArrayContainsPoint(map.GetSpacePointsInsideRange(origin, steps), point);
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
    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        var isExactlyStepsAway = new Helper().SpacePointArrayContainsPoint(map.GetSpacePointsInsideRange(token.position, exact_steps, exact_steps), point);
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

    public override bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players)
    {
        var isExactlyStepsAway = new Helper().SpacePointArrayContainsPoint(map.GetSpacePointsInsideRange(token.position, exactSteps, exactSteps), point);
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