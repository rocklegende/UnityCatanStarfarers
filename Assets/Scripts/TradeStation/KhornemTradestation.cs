using System;
using UnityEngine;


public class KhornemTradeStation : TradeStation
{
    public KhornemTradeStation()
        : base(
            new AbstractFriendshipCard[] {
                new KhornemOneBoostOneCannon(),
                new KhornemTwoBoost(),
                new KhornemTwoCannon()
            },
            "Khornem",
            new Tile_[] { new KhornemTradeStationTile(), new KhornemTradeStationTile(), new KhornemTradeStationTile() }
            )
    {
    }

    public override string GetPictureName()
    {
        return "KhornemStation";
    }
}
