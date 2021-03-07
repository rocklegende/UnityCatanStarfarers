using System;
using UnityEngine;

public class FuelResourceTile : ResourceTile
{
    public FuelResourceTile(ChipGroup group) : base(new FuelResource(), group)
    {
    }

    public override Color? GetColor()
    {
        return base.GetColor();
    }
}
