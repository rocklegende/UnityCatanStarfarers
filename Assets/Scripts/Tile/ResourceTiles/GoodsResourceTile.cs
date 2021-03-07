using System;
using UnityEngine;

public class GoodsResourceTile : ResourceTile
{
    public GoodsResourceTile(ChipGroup group) : base(new GoodsResource(), group)
    {
    }

    public override Color? GetColor()
    {
        return base.GetColor();
    }
}
