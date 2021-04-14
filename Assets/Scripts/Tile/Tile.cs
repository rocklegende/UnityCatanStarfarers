using System;
using UnityEngine;

[Serializable]
public abstract class Tile_
{
    public Tile_()
    {
    }

    public abstract Color? GetColor();

    public abstract bool blocksTraffic();
}

