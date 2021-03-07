using System;
using UnityEngine;

public class OreResourceTile : ResourceTile
{
    public OreResourceTile(ChipGroup group) : base(new OreResource(), group)
    {
    }

    public override Color? GetColor()
    {
        return base.GetColor();
    }

}
