using System;
using UnityEngine;

[Serializable]
public class OrzelTradeStationTile : TradeStationTile
{
    public override Color? GetColor()
    {
        return Color.yellow;
    }
}
