using UnityEngine;
using System;

public interface Settable
{
    bool CanSettle(Tile_[] tiles);
}

public interface BuildableToken
{
    bool CanBeBuildByPlayer(Player player, Map map, Player[] players);
}

public abstract class Token 
{
    public SpacePoint position = null;
    public Vector3? unityPosition = null;
    public bool useOwnPositioningSystem = true;
    public Cost cost;
    public string id;
    protected Color color;
    public Token attachedToken = null;
    protected bool isTokenAttachable;
    int stepsLeft = 20; //TODO: should be 0 in real scenario
    public Player owner;

    public Token(string id, bool isTokenAttachable, Cost cost)
    {
        this.id = id;
        this.isTokenAttachable = isTokenAttachable;
        this.cost = cost;
    }

    public abstract Token makeCopy();

    public void DataChanged()
    {
        SFElement notifier = new SFElement();
        notifier.app.Notify(SFNotification.player_data_changed, notifier, new object[] { this });
        notifier.app.Notify(SFNotification.token_data_changed, notifier, new object[] { this });
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
            return (Vector3)unityPosition;
        }
    }

    protected abstract void OnSettle();

    public void settle()
    {
        OnSettle();
        DataChanged();
    }


    public void FlyTo(SpacePoint destination, Map map)
    {
        if (!CanFly())
        {
            throw new ArgumentException("This token did already exceed its maximum steps.");
        }
        stepsLeft -= map.distanceBetweenPoints(position, destination);
        SetPosition(destination);
    }

    public void addSteps(int steps)
    {
        stepsLeft += steps;
    }

    public bool IsFlyable()
    {
        if (attachedToken == null)
        {
            return false;
        }
        return attachedToken is ShipToken;
    }

    public bool CanFly()
    {
        return stepsLeft > 0;
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
        if (attachedToken != null)
        {
            var hasShipOnTop = attachedToken is ShipToken;
            return !hasShipOnTop;
        }
        return true;
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


    public void SetColor(Color color)
    {
        this.color = color;
        if (attachedToken != null)
        {
            attachedToken.SetColor(color);
        }
        DataChanged();
    }

    public Color GetColor()
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
        this.unityPosition = pos;
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