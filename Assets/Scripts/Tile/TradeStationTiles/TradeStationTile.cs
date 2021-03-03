using System;
using UnityEngine;

public abstract class TradeStationTile : Tile_
{
    //Color color;

    public TradeStationTile() {
        //this.color = color;
    }

    

    public override bool blocksTraffic()
    {
        return false;
    }
}
