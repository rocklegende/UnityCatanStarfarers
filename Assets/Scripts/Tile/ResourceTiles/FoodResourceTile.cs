﻿using System;
using UnityEngine;

public class FoodResourceTile : ResourceTile
{
    public FoodResourceTile(ChipGroup group) : base(new FoodResource(), group)
    {
    }

    public override Color? GetColor()
    {
        return base.GetColor();
    }
}
