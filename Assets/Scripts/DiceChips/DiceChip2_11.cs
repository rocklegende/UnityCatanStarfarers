using System;
using UnityEngine;

public class DiceChip2_11 : DiceChip
{
    public DiceChip2_11(ChipGroup chipGroup) : base(new int[] { 2, 11 }, chipGroup) { }

    public override string GetTextureName()
    {
        return "diceChip2_11";
    }


}
