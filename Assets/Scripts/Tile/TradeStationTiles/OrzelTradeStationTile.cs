using System;
using UnityEngine;


public class OrzelTradeStationTile : TradeStationTile
{
    public OrzelTradeStationTile(AbstractTradeStation tradeStation, int numDockingPorts) : base(tradeStation, numDockingPorts)
    {
    }

    public override Color? GetColor()
    {
        return Color.yellow;
    }
}
