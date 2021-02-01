using UnityEngine;

public abstract class Token
{
    //color
    //image

    public SpacePoint position = null;
    public Cost cost;
    public string id;
    Color color;
    bool isSettled;
    public Token attachedToken = null;
    bool isTokenAttachable;



    public Token(string id, bool isTokenAttachable, Cost cost)
    {
        this.id = id;
        this.isTokenAttachable = isTokenAttachable;
        this.cost = cost;
    }


    public int GetVictoryPoints()
    {
        if (isSettled)
        {
            return BaseVictoryPoints();
        } else
        {
            return 0;
        }
    }

    public int GetResourceProductionMultiplier()
    {
        if (isSettled)
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
        return !(this is ShipToken) && (attachedToken == null);
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