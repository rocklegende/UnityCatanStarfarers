using System;
using UnityEngine;


public class OrzelTradeStation : TradeStation
{
    public OrzelTradeStation()
        : base(
            new AbstractFriendshipCard[] { },
            "Orzel",
            new Tile_[]{ new OrzelTradeStationTile(), new OrzelTradeStationTile(), new OrzelTradeStationTile() }
            )
    {
    }

    public override string GetPictureName()
    {
        return "OrzelStation";
    }
}
