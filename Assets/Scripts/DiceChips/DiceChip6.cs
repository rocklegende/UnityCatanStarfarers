using System;
using UnityEngine;

public class DiceChip6 : DiceChip
{
    public DiceChip6(ChipGroup chipGroup) : base(new int[] { 6 }, chipGroup) { }

    public override string GetTextureName()
    {
        return "diceChip6";
    }


}
