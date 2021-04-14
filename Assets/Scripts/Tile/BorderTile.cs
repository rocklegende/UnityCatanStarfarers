using System;
using UnityEngine;

[Serializable]
public class BorderTile : Tile_
{
    public BorderTile()
    {
    }

    public override Color? GetColor()
    {
        return Color.yellow;
    }

    public override bool blocksTraffic()
    {
        return false;
    }
}
