using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TileGroup : SFModel
{
    protected Tile_[] tiles;
    protected List<SpacePoint> settlePoints;
    protected SpacePoint center;
    public readonly Guid guid;
    public TileGroup(Tile_[] tiles, int rightShifts = 0)
    {
        this.guid = Guid.NewGuid();
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

    public SpacePoint GetCenter()
    {
        return center;
    }

    protected void DataChanged()
    {
        Notify(null);
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
            if (settlePoint.IsEqualTo(point))
            {
                return true;
            }
        }
        return false;
    }

    public List<SpacePoint> GetSettlePoints()
    {
        return settlePoints;
    }

    public abstract void SetCenter(SpacePoint center);

    public abstract void OnTokenEnteredArea(Token token);

    public abstract void OnTokenSettled(Token token);

    public abstract bool RequestSettleOfToken(Token token, SpacePoint futurePositionOfToken = null);


}

[Serializable]
public class EmptyTileGroup : TileGroup
{
    public EmptyTileGroup() : base(new Tile_[] { new EmptyTile(), new EmptyTile(), new EmptyTile() })
    {
        settlePoints = new List<SpacePoint>();
    }

    public override void OnTokenEnteredArea(Token token)
    {
        //
    }

    public override void OnTokenSettled(Token token)
    {
        throw new NotImplementedException();
    }

    public override bool RequestSettleOfToken(Token token, SpacePoint futurePositionOfToken = null)
    {
        return false;
    }

    public override void SetCenter(SpacePoint center)
    {
        this.center = center;
    }
}

[Serializable]
public class ResourceTileGroup : TileGroup
{

    bool isRevealed = false;
    public ResourceTileGroup( ResourceTile[] tiles, int rightShifts = 0) : base(tiles, rightShifts)
    {
        this.tiles = tiles;
        //TODO: register as observer in Tiles, then notify app if something changed inside the tiles
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
        RemovePirateTokenAt(ResourceTilesWithPirateToken(), token.owner);
        DataChanged(); // TODO: change should be notified from inside ResourceTile class, then delegated to the ResourceTileGroup
        
    }

    void RemovePirateTokenAt(ResourceTile[] targetTiles, Player awardReceivingPlayer)
    {
        foreach (var tileWithPirateToken in targetTiles)
        {
            tileWithPirateToken.AssignNewDiceChipFromGroup();
            tileWithPirateToken.SetDiceChipFaceUp();
            awardReceivingPlayer.AddPirateTokenBeatenAward();
        }
    }

    ResourceTile[] ResourceTilesWithPirateToken()
    {
        var list = new List<ResourceTile>();
        foreach (var tile in tiles)
        {
            var rt = (ResourceTile)tile;
            var diceChip = rt.GetDiceChip();
            if (diceChip is PirateToken)
            {
                list.Add(rt);
            }
        }
        return list.ToArray();
    }

    public List<PirateToken> GetPirateTokens()
    {
        var pirateTokens = new List<PirateToken>();
        foreach(var tile in ResourceTilesWithPirateToken())
        {
            var diceChip = tile.GetDiceChip();
            if (diceChip is PirateToken)
            {
                pirateTokens.Add((PirateToken)diceChip);
            }
        }
        return pirateTokens;
    }

    /// <summary>
    /// Returns true if all the dice chips of this tilegroup have been revealed.
    /// </summary>
    /// <returns></returns>
    public bool IsRevealed()
    {
        return isRevealed;
    }

    public bool ShipCanBeatPirateTokens(SpaceShip ship, List<PirateToken> pirateTokens)
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

    public override bool RequestSettleOfToken(Token token, SpacePoint futurePositionOfToken = null)
    {
        if (!(token is ColonyBaseToken))
        {
            throw new WrongTokenTypeException("Token is not of Type ColonyBaseToken, therefore docking is denied");
        }

        if (futurePositionOfToken != null)
        {
            if (SpacePointOnAtleastOneSettlePoint(futurePositionOfToken))
            {
                if (!isRevealed)
                {
                    throw new NotOnSettleSpotException();
                }
            } else
            {
                throw new NotOnSettleSpotException();
            }
        } else
        {
            if (!SpacePointOnAtleastOneSettlePoint(token.position))
            {
                throw new NotOnSettleSpotException();
            }
        }
        

        if (GetPirateTokens().Count > 0)
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
        isRevealed = true;
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
        DataChanged();
    }

    
}
