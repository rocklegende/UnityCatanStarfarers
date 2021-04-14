using System;
using UnityEngine;

[Serializable]
public class KhornemTradeStationTile : TradeStationTile
{
    public override Color? GetColor()
    {
        return Color.red;
    }
}
