using System;
using UnityEngine;

[Serializable]
public class RahnaTradeStation : TradeStation
{
    public RahnaTradeStation()
        : base(
            new AbstractFriendshipCard[] { new RahnaBuyFamemedals(), new RahnaBuyFamemedals(), new RahnaDiscardLimitIncrease(), new RahnaNoIncomeBonus(), new RahnaRichHelpPoorBonus()},
            "Rahna",
            new Tile_[] { new RahnaTradeStationTile(), new RahnaTradeStationTile(), new RahnaTradeStationTile() } //TODO: no need really for extra classes for the trade station tiles, just create tiles with a certain color

            )
    {
    }

    public override string GetPictureName()
    {
        return "AxxaStation";
    }
}
