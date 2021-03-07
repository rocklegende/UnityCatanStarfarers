using System;
using System.Collections.Generic;
public abstract class TileGroup
{
    protected Tile_[] tiles;
    protected SpacePoint[] settlePoints;
    protected SpacePoint center;
    public TileGroup(Tile_[] tiles, int rightShifts = 0)
    {
        // shifts: shifts the tiles x times
        if (tiles.Length != 3)
        {
            throw new ArgumentException("TileGroup has to consist of exactly 3 tiles.");
        }
        this.tiles = tiles;
        ShiftTiles(rightShifts);
    }

    public Tile_[] GetTiles()
    {
        return this.tiles;
    }

    void DataChanged()
    {
        var notifier = new SFElement();
        //notifier.app.Notify(SFNotification.map_data_changed, notifier);
        notifier.app.Notify(SFNotification.tile_group_data_changed, notifier);
    }

    public void ShiftTiles(int numShifts)
    {
        Tile_[] shifted = new Tile_[tiles.Length];

        for (int i = 0; i < tiles.Length; i++)
        {
            shifted[(i + numShifts) % shifted.Length] = tiles[i];
        }
        this.tiles = shifted;
    }

    public void HandleTokenSettled(Token token)
    {
        OnTokenSettled(token);
        DataChanged();
    }

    protected bool SpacePointOnAtleastOneSettlePoint(SpacePoint point)
    {
        foreach (var settlePoint in settlePoints)
        {
            if (settlePoint.Equals(point))
            {
                return true;
            }
        }
        return false;
    }

    public abstract void SetCenter(SpacePoint center);

    public abstract void OnTokenEnteredArea(Token token);

    public abstract void OnTokenSettled(Token token);

    public abstract bool RequestSettleOfToken(Token token);


}

public class ResourceTileGroup : TileGroup
{    
    public ResourceTileGroup( ResourceTile[] tiles, int rightShifts = 0) : base(tiles, rightShifts)
    {
        this.tiles = tiles;
    }

    public override void SetCenter(SpacePoint center)
    {
        this.center = center;
        settlePoints = center.GetNeighbors();
    }

    public override void OnTokenEnteredArea(Token token)
    {
        RevealDiceChips();
    }

    public override void OnTokenSettled(Token token)
    {
        //throw new NotImplementedException();
    }

    PirateToken[] GetPirateTokens()
    {
        var pirateTokens = new List<PirateToken>();
        foreach(var tile in tiles)
        {
            var rt = (ResourceTile)tile;
            var diceChip = rt.GetDiceChip();
            if (diceChip is PirateToken)
            {
                pirateTokens.Add((PirateToken)diceChip);
            }
        }
        return pirateTokens.ToArray();
    }

    public bool ShipCanBeatPirateTokens(SpaceShip ship, PirateToken[] pirateTokens)
    {
        foreach(var pirateToken in pirateTokens)
        {
            if (!pirateToken.SpaceshipCanBeatIt(ship))
            {
                return false;
            }
        }
        return true;
    }

    public override bool RequestSettleOfToken(Token token)
    {
        if (!(token is ColonyBaseToken))
        {
            throw new WrongTokenTypeException("Token is not of Type ColonyBaseToken, therefore docking is denied");
        }

        if (!SpacePointOnAtleastOneSettlePoint(token.position))
        {
            throw new NotOnSettleSpotException();
        }

        if (GetPirateTokens().Length > 0)
        {
            if (!ShipCanBeatPirateTokens(token.owner.ship, GetPirateTokens()))
            {
                throw new CannotBeatPirateTokenException();
            }
        }

        //TODO:  deny if position of token is occupied by other token, should be prevented to even go there, maybe its not necessary

        return true;
    }

    public void RevealDiceChips()
    {
        foreach( var tile in tiles)
        {
            if (tile is ResourceTile)
            {
                var rt = (ResourceTile)tile;
                var diceChip = rt.GetDiceChip();
                if (diceChip != null)
                {
                    diceChip.isFaceUp = true;
                }
                
            }
            
        }
        var notifier = new SFElement();
        notifier.app.Notify(SFNotification.tile_group_data_changed, notifier);
    }
}
