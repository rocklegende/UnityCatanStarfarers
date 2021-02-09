using UnityEngine;
using System;

public interface Settable
{
    bool CanSettle(Tile_[] tiles);
}

public abstract class Token 
{
    //color
    //image

    public SpacePoint position = null;
    public Cost cost;
    public string id;
    protected Color color;
    public Token attachedToken = null;
    protected bool isTokenAttachable;
    int stepsLeft = 5;



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

    public int GetStepsLeft()
    {
        return stepsLeft;
    }

    public void settle()
    {
        attachedToken = null;
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
    }

    public void SetPosition(SpacePoint pos)
    {
        this.position = pos;
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