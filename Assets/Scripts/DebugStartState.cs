using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.onebuckgames.UnityStarFarers;

public abstract class DebugStartState
{
    protected GameController controller;
    public DebugStartState(GameController controller)
    {
        this.controller = controller;
    }

    public List<Player> GetGenericPlayers()
    {
        return new List<Player> { new Player(new SFColor(Color.white)), new Player(new SFColor(Color.red)) };
    }

    public void BuildShipsForMainPlayer(Map map, Player player)
    {
        player.BuildTokenWithoutCost(map, new ColonyBaseToken().GetType(), new SpacePoint(new HexCoordinates(5, 5), 1), new SpacePortToken().GetType());
        player.BuildTokenWithoutCost(map, new TradeBaseToken().GetType(), new SpacePoint(new HexCoordinates(5, 5), 0), new ShipToken().GetType());
        player.BuildTokenWithoutCost(map, new ColonyBaseToken().GetType(), new SpacePoint(new HexCoordinates(5, 5).W(), 0), new ShipToken().GetType());
    }

    public void BuildShipsForSecondPlayer(Map map, Player player)
    {
        player.BuildTokenWithoutCost(map, new ColonyBaseToken().GetType(), new SpacePoint(new HexCoordinates(7, 4), 0), new SpacePortToken().GetType());
        player.BuildTokenWithoutCost(map, new TradeBaseToken().GetType(), new SpacePoint(new HexCoordinates(7, 4), 1), new ShipToken().GetType());
        player.BuildTokenWithoutCost(map, new ColonyBaseToken().GetType(), new SpacePoint(new HexCoordinates(7, 4).E(), 1), new ShipToken().GetType());
    }

    public void SetUpgradesForMainPlayer(int amount)
    {
        TestHelper.SetUpgradesForPlayer(controller.mainPlayer, amount);
    }

    public void GiveResourcesOfAllTypesToPlayer(Player player, int amount)
    {
        player.AddHand(Hand.FromResources(amount, amount, amount, amount, amount));
    }

    public void CommonSetup()
    {
        MapGenerator generator = new DefaultMapGenerator();

        var gameStateInfo = new SFGameStateInfo();
        gameStateInfo.players = GetGenericPlayers();
        gameStateInfo.mapmodel = generator.GenerateRandomMap();
        controller.mainPlayerGuid = controller.players[0].name;
        controller.LoadGameState(gameStateInfo);

        controller.HUD.GetComponent<HUDScript>().OnPlayerDataChanged();

        

        controller.payoutHandler = new PayoutHandler(controller.mapModel);
        GiveResourcesOfAllTypesToPlayer(controller.mainPlayer, 5);
    }

    void CommonMapSetup()
    {
        
    }

    public abstract void Setup();
}

public class ShipBuildingOneColonyShipAndOneSpacePort : DebugStartState
{
    public ShipBuildingOneColonyShipAndOneSpacePort(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.State = new BuildAndTradeState(controller);

        //CommonSetup();
        SetUpgradesForMainPlayer(5);

        controller.mainPlayer.BuildTokenWithoutCost(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 9), 1),
            new SpacePortToken().GetType()
        );

        controller.mainPlayer.BuildColonyShipForFree(
            controller.mapModel,
            new SpacePoint(new HexCoordinates(4, 9), 0)
        );
    }
}

public class EncounterCardTestingState : DebugStartState
{
    public EncounterCardTestingState(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.State = new BuildAndTradeState(controller);

        CommonSetup();
        controller.mainPlayer.BuildUpgradeWithoutCost(new BoosterUpgradeToken());

        controller.mainPlayer.BuildToken(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 1),
            new SpacePortToken().GetType()
        );

        controller.mainPlayer.BuildToken(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 0),
            new ShipToken().GetType()
        );

        //var encounterFactory = new EncounterCardFactory(controller);
        //controller.encounterCardHandler.PlayEncounterCard(encounterFactory.CreateEncounterCard16());
    }
}

public class EncounterCardTestingStateManual : DebugStartState
{
    public EncounterCardTestingStateManual(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.State = new BuildAndTradeState(controller);

        CommonSetup();
        controller.mainPlayer.BuildUpgradeWithoutCost(new BoosterUpgradeToken());

        controller.mainPlayer.BuildToken(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 1),
            new SpacePortToken().GetType()
        );

        controller.mainPlayer.BuildToken(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 0),
            new ShipToken().GetType()
        );

        var encounterFactory = new EncounterCardFactory(controller);
        controller.encounterCardHandler.PlayEncounterCard(encounterFactory.CreateEncounterCard12());
    }
}

public class TwoTradeShipAndOneSpacePort : DebugStartState
{
    string gameState;
    public TwoTradeShipAndOneSpacePort(GameController controller, string gameState = "flyships") : base(controller)
    {
        this.gameState = gameState;
    }

    public override void Setup()
    {
        controller.mainPlayer.BuildTokenWithoutCost(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 1),
            new SpacePortToken().GetType()
        );

        controller.mainPlayer.BuildColonyShipForFree(
            controller.mapModel,
            new SpacePoint(5, 5, 0)
        );

        controller.mainPlayer.BuildTradeShipForFree(
            controller.mapModel,
            new SpacePoint(new HexCoordinates(5, 5).W(), 0)
        );

        if (gameState == "flyShips")
        {
            controller.State = new FlyShipsState(controller);
        } else if (gameState == "encounter")
        {
            controller.State = new EncounterCardState(controller);
        } else
        {
            controller.State = new FlyShipsState(controller);
        }
        
    }
}

public class OneTradeOneColonyShipAndOneSpacePort : DebugStartState
{
    public OneTradeOneColonyShipAndOneSpacePort(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.State = new FlyShipsState(controller);

        CommonSetup();

        controller.mainPlayer.BuildTokenWithoutCost(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 1),
            new SpacePortToken().GetType()
        );

        controller.mainPlayer.BuildTokenWithoutCost(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 0),
            new ShipToken().GetType()
        );

        controller.mainPlayer.BuildTokenWithoutCost(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5).W(), 0),
            new ShipToken().GetType()
        );
    }
}

public class TwoPlayersWithShips : DebugStartState
{
    public TwoPlayersWithShips(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.State = new FlyShipsState(controller);

        CommonSetup();

        BuildShipsForMainPlayer(controller.mapModel, controller.players[0]);
        BuildShipsForSecondPlayer(controller.mapModel, controller.players[1]);       
    }
}

public class TestShipDiceState : DebugStartState
{
    public TestShipDiceState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.State = new CastShipDiceState(controller);

        CommonSetup();
        BuildShipsForMainPlayer(controller.mapModel, controller.players[0]);

        controller.mainPlayer.AddFameMedal();
    }
}

public class TestNormalDiceDebugState : DebugStartState
{
    public TestNormalDiceDebugState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.State = new CastNormalDiceState(controller);

        CommonSetup();
        BuildShipsForMainPlayer(controller.mapModel, controller.players[0]);

        controller.mainPlayer.AddFameMedal();
    }
}

public class BeatPirateTokenDebugState : DebugStartState
{
    public BeatPirateTokenDebugState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.State = new FlyShipsState(controller);

        CommonSetup();
        BuildShipsForMainPlayer(controller.mapModel, controller.players[0]);
        SetUpgradesForMainPlayer(5);        
        controller.mainPlayer.AddFameMedal();
        
    }
}

public class BuildASpacePortDebugState : DebugStartState
{
    public BuildASpacePortDebugState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.State = new BuildAndTradeState(controller);

        CommonSetup();
        controller.mainPlayer.BuildTokenWithoutCost(controller.mapModel, new ColonyBaseToken().GetType(), new SpacePoint(new HexCoordinates(5, 5), 1));
        
    }
}

public class PlayerHasRichHelpPoorBonusDebugState : DebugStartState
{
    public PlayerHasRichHelpPoorBonusDebugState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.State = new BuildAndTradeState(controller);

        //controller.players = new List<Player>() { new Player(new SFColor(Color.green)), new Player(new SFColor(Color.yellow)), new Player(new SFColor(Color.blue)), new Player(new SFColor(Color.red)), };
        controller.mainPlayerGuid = controller.players[0].name;
        controller.HUD.GetComponent<HUDScript>().OnPlayerDataChanged();

        foreach (var player in controller.players)
        {
            for (int i = 0; i < 3; i++)
            {
                foreach (var card in new Helper().GetAllResourceCardTypes())
                {
                    player.AddCard(card);
                }
            }
        }
        

        controller.mainPlayer.BuildTokenWithoutCost(controller.mapModel, new ColonyBaseToken().GetType(), new SpacePoint(new HexCoordinates(5, 5), 1));
        controller.mainPlayer.AddFriendShipCard(new RahnaRichHelpPoorBonus());
        
    }
}