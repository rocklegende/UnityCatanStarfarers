using System;
using UnityEngine;


public class OrzelTradeStation : TradeStation
{
    public OrzelTradeStation()
        : base(
            new AbstractFriendshipCard[] { new OrzelFriendshipCardFood(), new OrzelFriendshipCardFood() },
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
