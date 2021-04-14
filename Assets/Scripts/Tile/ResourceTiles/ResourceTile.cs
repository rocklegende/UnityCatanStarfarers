using System;
using UnityEngine;

[Serializable]
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
        return this.resource.tileColor.ToUnityColor();
    }

    public override bool blocksTraffic()
    {
        return true;
    }

    public void AssignNewDiceChipFromGroup() {
        //TODO: the following is the correct code -> var chip = chipGroup.RetrieveChip();
        var chip = NormalChipFactory.Create3_12Chip();
        SetDiceChip(chip);
    }

    public void FlipDiceChip()
    {
        if (diceChip != null)
        {
            diceChip.Flip();
        }
    }
    /// <summary>
    /// Set dice chip to the side where the dice value is visible.
    /// </summary>
    public void SetDiceChipFaceUp()
    {
        if (diceChip != null)
        {
            diceChip.SetFaceUp();
        }
    }

    /// <summary>
    /// Set dice chip to the side where the dice group symbol is visible.
    /// </summary>
    public void SetDiceChipFaceDown()
    {
        if (diceChip != null)
        {
            diceChip.SetFaceDown();
        }
    }


}
