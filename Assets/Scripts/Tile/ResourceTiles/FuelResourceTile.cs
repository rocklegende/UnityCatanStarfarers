using System;
using UnityEngine;

public class FuelResourceTile : ResourceTile
{
    public FuelResourceTile() : base(new FuelResource())
    {
    }

    public override Color? GetColor()
    {
        return base.GetColor();
    }
}
