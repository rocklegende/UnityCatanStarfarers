using System;
using UnityEngine;

public abstract class TradeStationTile : Tile_
{
    public AbstractTradeStation assignedTradeStation;
    public int numDockingPorts;
    public TradeStationTile(AbstractTradeStation assignedTradeStation, int numDockingPorts)
    {
        this.assignedTradeStation = assignedTradeStation;
        this.numDockingPorts = numDockingPorts;
    }

    public override bool blocksTraffic()
    {
        return false;
    }
}
