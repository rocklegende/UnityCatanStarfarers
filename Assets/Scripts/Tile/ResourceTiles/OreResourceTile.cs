using System;
using UnityEngine;

[Serializable]
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
