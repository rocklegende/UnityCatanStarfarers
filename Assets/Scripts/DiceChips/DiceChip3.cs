using System;
using UnityEngine;

public class DiceChip3: DiceChip
{
    public DiceChip3() : base(new int[] { 3 }) { }

    public override string GetTextureName() {
        return "diceChip3";
    }


}
