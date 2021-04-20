using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using com.onebuckgames.UnityStarFarers;

public class TestHelper
{
    public TestHelper()
    {

    }

    public Player CreateGenericPlayer()
    {
        return new Player(new SFColor(Color.black));
    }

    public static List<Player> CreateGenericPlayers3()
    {
        var player1 = new Player(new SFColor(Color.black))
        {
            name = "Timmi",
        };

        var player2 = new Player(new SFColor(Color.green))
        {
            name = "Paul",
        };

        var player3 = new Player(new SFColor(Color.yellow))
        {
            name = "Gabi",
        };

        return new List<Player>() { player1, player2, player3 };
    }

    public static Map CreateMockMap()
    {
        return new Map(new Tile_[,] { });
    }


    public static Map SerializeAndDeserialize(Map original)
    {
        var serialized = SFFormatter.Serialize(original);
        var deserialized = (Map) SFFormatter.Deserialize(serialized);
        
        return deserialized;
    }

    public static void SetUpgradesForPlayer(Player player, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            player.ship.Add(new BoosterUpgradeToken());
            player.ship.Add(new FreightPodUpgradeToken());
            player.ship.Add(new CannonUpgradeToken());
        }
    }
}
