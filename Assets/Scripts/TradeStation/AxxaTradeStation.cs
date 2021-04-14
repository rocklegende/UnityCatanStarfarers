using System;
using UnityEngine;

[Serializable]
public class AxxaTradeStation : TradeStation
{
    public AxxaTradeStation()
        : base(
            new AbstractFriendshipCard[] { new AxxaIncreaseFood(), new AxxaIncreaseCarbon(), new AxxaIncreaseFuel(), new AxxaIncreaseGoods(), new AxxaIncreaseOre() },
            "Axxa",
            new Tile_[] { new AxxaTradeStationTile(), new AxxaTradeStationTile(), new AxxaTradeStationTile() }
            )
    {
    }

    public override string GetPictureName()
    {
        return "AxxaStation";
    }
}
