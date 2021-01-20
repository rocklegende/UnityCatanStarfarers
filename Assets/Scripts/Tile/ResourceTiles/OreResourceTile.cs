using System;
using UnityEngine;

public class OreResourceTile : ResourceTile
{
    public OreResourceTile() : base(new OreResource())
    {
    }

    public override Color? GetColor()
    {
        return base.GetColor();
    }
}
