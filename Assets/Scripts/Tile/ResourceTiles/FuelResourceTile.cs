using System;
using UnityEngine;

[Serializable]
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
