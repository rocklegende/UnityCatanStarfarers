using System;
using UnityEngine;

public abstract class ResourceTile : Tile_
{
    public DiceChip diceChip = null;
    public ChipGroup chipGroup;

    public readonly Resource resource;
    
    public ResourceTile(Resource resource, ChipGroup group)
    {
        this.resource = resource;
        this.chipGroup = group;
    }

    public void SetDiceChip(DiceChip dc)
    {
        //TODO: somehow need to prevent setting dicechips from wrong group, need to improve that
        //if (!dc.chipGroup.IsEqualTo(chipGroup))
        //{
        //    throw new ArgumentException("Chipgroups dont match");
        //}
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
