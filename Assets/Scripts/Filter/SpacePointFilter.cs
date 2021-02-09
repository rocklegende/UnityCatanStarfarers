using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class SpacePointFilter
{
    public abstract bool pointFulfillsFilter(SpacePoint point, Map map, Player[] players);
    public Token[] GetAllTokenOfPlayers(Player[] players)
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