using System;
using UnityEngine;

public abstract class ResourceTile : Tile_
{
    public DiceChip diceChip = null;
    public readonly Resource resource;
    
    public ResourceTile(Resource resource)
    {
        this.resource = resource;
    }

    public void SetDiceChip(DiceChip dc)
    {
        diceChip = dc;
    }

    public DiceChip GetDiceChip()
    {
        return diceChip;
    }

    public override Color? GetColor()
    {
        return this.resource.tileColor;
    }

    public override bool blocksTraffic()
    {
        return true;
    }


}
