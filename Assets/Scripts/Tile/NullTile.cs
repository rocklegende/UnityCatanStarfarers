using System;
using UnityEngine;

public class NullTile : Tile_
{
    public NullTile()
    {
    }

    public override Color? GetColor()
    {
        return Color.red;
    }

    public override bool blocksTraffic()
    {
        return true;
    }
}
