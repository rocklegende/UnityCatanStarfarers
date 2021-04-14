using System;
using UnityEngine;

[Serializable]
public class CarbonResourceTile : ResourceTile
{
    public CarbonResourceTile(ChipGroup group) : base(new CarbonResource(), group)
    {
    }

    public override Color? GetColor()
    {
        return base.GetColor();
    }
}
