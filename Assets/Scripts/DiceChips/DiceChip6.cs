using System;
using UnityEngine;

public class DiceChip6 : DiceChip
{
    public DiceChip6() : base(new int[] { 6 }) { }

    public override string GetTextureName()
    {
        return "diceChip6";
    }


}
