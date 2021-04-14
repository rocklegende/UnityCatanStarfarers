using System;
using UnityEngine;

[Serializable]
public abstract class TradeStationTile : Tile_
{
    public TradeStationTile() {
    }

    

    public override bool blocksTraffic()
    {
        return false;
    }
}
