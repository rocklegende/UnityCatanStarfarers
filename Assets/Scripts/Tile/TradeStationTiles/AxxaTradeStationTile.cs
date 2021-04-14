using System;
using UnityEngine;

[Serializable]
public class AxxaTradeStationTile : TradeStationTile
{
    public override Color? GetColor()
    {
        return Color.green;
    }
}
