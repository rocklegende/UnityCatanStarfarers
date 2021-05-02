using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class RahnaTradeStation : TradeStation
{
    public RahnaTradeStation()
        : base(
            new List<AbstractFriendshipCard>() { new RahnaBuyFamemedals(), new RahnaBuyFamemedals(), new RahnaDiscardLimitIncrease(), new RahnaNoIncomeBonus(), new RahnaRichHelpPoorBonus()},
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
