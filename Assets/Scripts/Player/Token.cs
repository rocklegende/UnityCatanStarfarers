﻿using UnityEngine;
using System;
using System.Collections.Generic;
using com.onebuckgames.UnityStarFarers;
using System.Collections;
using System.Linq;

public interface Settable
{
    bool CanSettle(Tile_[] tiles);
}

public interface BuildableToken
{
    bool CanBeBuildByPlayer(Player player, Map map);
    //TODO: get possible build points for this token, add play tests for building tokens
}

[Serializable]
public abstract class SFModel : Subject
{

}

[Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;
    public SerializableVector3(Vector3 unityVector3)
    {
        this.x = unityVector3.x;
        this.y = unityVector3.y;
        this.z = unityVector3.z;
    }

    public Vector3 ToUnityVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public abstract class Token : SFModel
{
    public SpacePoint position = null;
    public SerializableVector3 unityPosition = null;
    public bool useOwnPositioningSystem = true;
    public Cost cost;
    public string id;
    protected SFColor color;
    public Token attachedToken = null;
    protected bool isTokenAttachable;
    public int stepsLeft = 20; //TODO: should be 0 in real scenario
    public Player owner;
    protected bool isDisabled = false; //TODO should only be readable from the outside
    public Map associatedMap;

    public Token(string id, bool isTokenAttachable, Cost cost)
    {
        this.id = id;
        this.isTokenAttachable = isTokenAttachable;
        this.cost = cost;
    }

    public bool IsNextToOtherSpacePort()
    {
        var neighbors = GetNeighborTokens();
        foreach (var neighbor in neighbors)
        {
            if (neighbor.owner != this.owner)
            {
                if (neighbor.attachedToken != null)
                {
                    if (neighbor.attachedToken is SpacePortToken)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Get Tokens that are next to this token, regardless of owner.
    /// </summary>
    /// <returns></returns>
    public List<Token> GetNeighborTokens()
    {
        var neighbors = new List<Token>();
        var neighborSpacePoints = associatedMap.GetNeighborsOfSpacePoint(position);
        foreach (var neighbor in neighborSpacePoints)
        {
            var token = associatedMap.TokenAtPoint(neighbor);
            if (token != null)
            {
                neighbors.Add(token);
            }
        }
        return neighbors;
    }

    /// <summary>
    /// Returns list of all space points this token can reach.
    /// </summary>
    /// <returns></returns>
    public List<SpacePoint> ReachableSpacePoints()
    {
        var pointsInsideRangeMap = associatedMap.GetSpacePointsInsideRangeWithDistanceMap(position, GetStepsLeft());

        var filters = new List<SpacePointFilter> {
            new IsValidSpacePointFilter(),
            new IsSpacePointFreeFilter(),
        };
        var validAndFreePoints = associatedMap.GetSpacePointsFullfillingFilters(filters);

        var result = new List<SpacePoint>();
        foreach(var point in validAndFreePoints)
        {
            
            if (!pointsInsideRangeMap.Keys.ToList().Contains(point))
            {
                continue;
            }

            var containsItKey = pointsInsideRangeMap.ContainsKey(point);
            var isAtMaximumReachOfToken = pointsInsideRangeMap[point] == GetStepsLeft();
            var cannotSettleAtPoint = !associatedMap.TokenCanSettle(this, point);
            if (isAtMaximumReachOfToken && (IsNextToOtherSpacePort() || cannotSettleAtPoint))
            {
                //prevent blocking spots for others
                continue;
            }

            result.Add(point);
        }
        return result;

        //var filters = new List<SpacePointFilter> {
        //    new IsValidSpacePointFilter(),
        //    new IsSpacePointFreeFilter(),
        //    // TODO: these filters take very long time to calculate because we make n * BFS to calculate if point is steps away,
        //    // better to only create distance map once and then filter out points that are not inside this distance map
        //    new IsStepsAwayFilter(position, GetStepsLeft()),
        //    new IsExactlyStepsAwayAndCannotSettleOnPointCounter(this, GetStepsLeft()),
        //    new IsNeighborOfOwnSpacePortOrNotExactlyStepsAway(this, owner, GetStepsLeft())
        //};

    }

    public abstract Token makeCopy();

    public void DataChanged()
    {
        Notify(new object[] { this });
    }

    public bool IsDisabled()
    {
        return isDisabled;
    }

    public void Disable()
    {
        isDisabled = true;
        DataChanged();
    }

    public void Enable()
    {
        isDisabled = false;
        DataChanged();
    }

    public bool PlayerHasTokenInStorageAndCanPay(Player player)
    {
        return player.tokenStorage.GetTokensOfType(GetType()).Length > 0 && player.hand.CanPayCost(cost);
    }

    public int GetStepsLeft()
    {
        return stepsLeft;
    }

    public Vector3 GetUnityPosition()
    {
        if (useOwnPositioningSystem)
        {
            return position.ToUnityPosition();
        } else
        {
            return unityPosition.ToUnityVector3();
        }
    }

    protected abstract void OnSettle();

    public void settle()
    {
        OnSettle();
        DataChanged();
    }


    public void FlyTo(SpacePoint destination)
    {
        if (!CanFly())
        {
            throw new ArgumentException("This token did already exceed its maximum steps.");
        }
        stepsLeft -= associatedMap.distanceBetweenPoints(position, destination);
        SetPosition(destination);
    }

    public void addSteps(int steps)
    {
        stepsLeft += steps;
    }

    public void SetStepsLeft(int steps)
    {
        stepsLeft = steps;
    }

    /// <summary>
    /// Returns true if the token has a ShipToken on top.
    /// </summary>
    /// <returns></returns>
    public bool HasShipTokenOnTop()
    {
        if (attachedToken == null)
        {
            return false;
        }
        return attachedToken is ShipToken;
    }

    public void HandleNewTurn()
    {
        isDisabled = false;
        DataChanged();
    }

    /// <summary>
    /// Token is able to fly minimum of 1 step.
    /// </summary>
    /// <returns></returns>
    public bool CanFly()
    {
        return HasShipTokenOnTop() && stepsLeft > 0 && !isDisabled;
    }

    public int GetVictoryPoints()
    {
        if (IsSettled())
        {
            int sum = 0;
            sum += BaseVictoryPoints();

            if (attachedToken != null)
            {
                sum += attachedToken.BaseVictoryPoints();
            }
            return sum;
        } else
        {
            return 0;
        }
    }

    public int GetResourceProductionMultiplier()
    {
        if (IsSettled())
        {
            return ResourceProduce();
        } else
        {
            return 0;
        }
    }

    public abstract int BaseVictoryPoints();

    public abstract int ResourceProduce();

    public bool IsSettled()
    {
        return !HasShipTokenOnTop();
    }

    public void attachToken(Token token)
    {
        if (isTokenAttachable)
        {
            attachedToken = token;
        }
    }

    public void detachToken()
    {
        attachedToken = null;
    }


    public void SetColor(SFColor color)
    {
        this.color = color;
        if (attachedToken != null)
        {
            attachedToken.SetColor(color);
        }
        DataChanged();
    }

    public SFColor GetColor()
    {
        return this.color;
    }

    public void SetPosition(SpacePoint pos)
    {
        this.position = pos;
        DataChanged();
    }

    public void SetUnityPosition(Vector3 pos)
    {
        this.unityPosition = new SerializableVector3(pos);
        DataChanged();
    }

    public bool IsOnGameBoard()
    {
        return this.position != null;
    }

    public void RemoveFromGameBoard()
    {
        this.position = null;
    }



}