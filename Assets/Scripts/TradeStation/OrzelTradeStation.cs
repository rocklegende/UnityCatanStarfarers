using System;
using UnityEngine;

[Serializable]
public class OrzelTradeStation : TradeStation
{
    public OrzelTradeStation()
        : base(
            new AbstractFriendshipCard[] {
                new OrzelFriendshipCardFood(),
                new OrzelFriendshipCardGoods(),
                new OrzelFriendshipCardFuel(),
                new OrzelFriendshipCardCarbon(),
                new OrzelFriendshipCardOre()
            },
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
