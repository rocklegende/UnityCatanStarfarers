using System;
using UnityEngine;

public class DiceChip3: DiceChip
{
    public DiceChip3(ChipGroup chipGroup) : base(new int[] { 3 }, chipGroup) { }

    public override string GetTextureName() {
        return "diceChip3";
    }


}
