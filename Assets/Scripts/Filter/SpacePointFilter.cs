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
        if (point.Equals(new SpacePoint(4, 5, 0)))
        {
            Debug.Log("hallo");
        }
        List<SpacePoint> neighborPoints = map.GetNeighborsOfSpacePoint(point);
        foreach (SpacePoint neighbor in neighborPoints)
        {
            foreach (Token token in map.tokensOnMap)
            {
                if (token.position.Equals(neighbor))
                {
                    //TODO: we must be careful here, because sometimes we get a stale reference, because we updated the main player from serialized data
                    // and then token.owner == mainPlayer will not work anymore, because theyre not the same object anymore!
                    // probably better to just update the mainPlayer info instead of creating a whole new object
                    //
                    // Player.Update(newData)
                    // => if(newData["vp"]) => updateVP and so on...
                    if (token.attachedToken != null)
                    {
                        if (token.attachedToken is SpacePortToken && token.owner.guid == mainPlayer.guid)
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