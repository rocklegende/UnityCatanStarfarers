using System;
using UnityEngine;

public class FoodResourceTile : ResourceTile
{
    public FoodResourceTile() : base(new FoodResource())
    {
    }

    public override Color? GetColor()
    {
        return base.GetColor();
    }
}
