using System;
using UnityEngine;

[Serializable]
public class RahnaTradeStationTile : TradeStationTile
{
    public override Color? GetColor()
    {
        return Color.blue;
    }
}
