﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugStartState
{
    protected GameController controller;
    public DebugStartState(GameController controller)
    {
        this.controller = controller;
    }

    public Player[] GetGenericPlayers()
    {
        return new Player[] { new Player(Color.white, controller), new Player(Color.red, controller) };
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
        for (int i = 0; i < amount; i++)
        {
            controller.players[0].ship.Add(new BoosterUpgradeToken());
            controller.players[0].ship.Add(new FreightPodUpgradeToken());
            controller.players[0].ship.Add(new CannonUpgradeToken());
        }
    }

    public void GiveResourcesOfAllTypesToPlayer(Player player, int amount)
    {
        foreach(var type in new Helper().GetAllResourceCardTypes())
        {
            for (int i = 0; i < amount; i++)
            {
                player.AddCard(type);
            }
        }
    }

    public void CommonSetup()
    {
        controller.players = GetGenericPlayers();
        controller.mainPlayer = controller.players[0];
        controller.HUD.GetComponent<HUDScript>().SetPlayers(controller.players);
        controller.HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

        GiveResourcesOfAllTypesToPlayer(controller.mainPlayer, 5);
    }

    public void CommonMapSetup()
    {
        MapGenerator generator = new DefaultMapGenerator();
        controller.mapModel = generator.GenerateRandomMap();

        controller.Map.GetComponent<MapScript>().SetMap(controller.mapModel);
        controller.Map.GetComponent<MapScript>().SetPlayers(controller.players);
        controller.Map.GetComponent<MapScript>().isReceivingNotifications = true;

        controller.payoutHandler = new PayoutHandler(controller.mapModel);
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
        controller.state = new StartState(controller);

        CommonSetup();
        SetUpgradesForMainPlayer(5);

        controller.mainPlayer.BuildToken2(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 1),
            new SpacePortToken().GetType()
        );

        controller.mainPlayer.BuildToken2(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 0),
            new ShipToken().GetType()
        );

        CommonMapSetup();
    }
}

public class EncounterCardTestingState : DebugStartState
{
    public EncounterCardTestingState(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.state = new StartState(controller);

        CommonSetup();
        SetUpgradesForMainPlayer(3);

        CommonMapSetup();

        controller.mainPlayer.BuildToken2(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 1),
            new SpacePortToken().GetType()
        );

        controller.mainPlayer.BuildToken2(
            controller.mapModel,
            new ColonyBaseToken().GetType(),
            new SpacePoint(new HexCoordinates(5, 5), 0),
            new ShipToken().GetType()
        );

        var encounterFactory = new EncounterCardFactory(controller);
        controller.encounterCardHandler.PlayEncounterCard(encounterFactory.CreateEncounterCard5());
    }
}

public class TwoTradeShipAndOneSpacePort : DebugStartState
{
    public TwoTradeShipAndOneSpacePort(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.state = new FlyShipsState(controller);

        CommonSetup();

        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 1));
        controller.mainPlayer.BuildToken(spacePort);

        Token colonyShip = new TradeBaseToken();
        colonyShip.attachedToken = new ShipToken();
        colonyShip.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 0));
        controller.mainPlayer.BuildToken(colonyShip);

        Token colonyShip2 = new TradeBaseToken();
        colonyShip2.attachedToken = new ShipToken();
        colonyShip2.SetPosition(new SpacePoint(new HexCoordinates(5, 5).W(), 0));
        controller.mainPlayer.BuildToken(colonyShip2);

        CommonMapSetup();
    }
}

public class OneTradeOneColonyShipAndOneSpacePort : DebugStartState
{
    public OneTradeOneColonyShipAndOneSpacePort(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.state = new FlyShipsState(controller);

        CommonSetup();

        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 1));
        controller.mainPlayer.BuildToken(spacePort);

        Token colonyShip = new TradeBaseToken();
        colonyShip.attachedToken = new ShipToken();
        colonyShip.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 0));
        controller.mainPlayer.BuildToken(colonyShip);

        Token colonyShip2 = new ColonyBaseToken();
        colonyShip2.attachedToken = new ShipToken();
        colonyShip2.SetPosition(new SpacePoint(new HexCoordinates(5, 5).W(), 0));
        controller.mainPlayer.BuildToken(colonyShip2);


        CommonMapSetup();
    }
}

public class TwoPlayersWithShips : DebugStartState
{
    public TwoPlayersWithShips(GameController controller) : base(controller)
    {
    }

    public override void Setup()
    {
        controller.state = new FlyShipsState(controller);

        CommonSetup();

        BuildShipsForMainPlayer(controller.mapModel, controller.players[0]);
        BuildShipsForSecondPlayer(controller.mapModel, controller.players[1]);       

        CommonMapSetup();
    }
}

public class TestShipDiceState : DebugStartState
{
    public TestShipDiceState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.state = new CastShipDiceState(controller);

        CommonSetup();
        BuildShipsForMainPlayer(controller.mapModel, controller.players[0]);

        controller.mainPlayer.AddFameMedal();
        CommonMapSetup();
    }
}

public class TestNormalDiceDebugState : DebugStartState
{
    public TestNormalDiceDebugState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.state = new CastNormalDiceState(controller);

        CommonSetup();
        BuildShipsForMainPlayer(controller.mapModel, controller.players[0]);

        controller.mainPlayer.AddFameMedal();
        CommonMapSetup();
    }
}

public class BeatPirateTokenDebugState : DebugStartState
{
    public BeatPirateTokenDebugState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.state = new FlyShipsState(controller);

        CommonSetup();
        BuildShipsForMainPlayer(controller.mapModel, controller.players[0]);
        SetUpgradesForMainPlayer(5);        
        controller.mainPlayer.AddFameMedal();
        CommonMapSetup();
    }
}

public class BuildASpacePortDebugState : DebugStartState
{
    public BuildASpacePortDebugState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.state = new StartState(controller);

        CommonSetup();
        controller.mainPlayer.BuildTokenWithoutCost(controller.mapModel, new ColonyBaseToken().GetType(), new SpacePoint(new HexCoordinates(5, 5), 1));
        CommonMapSetup();
    }
}

public class PlayerHasRichHelpPoorBonusDebugState : DebugStartState
{
    public PlayerHasRichHelpPoorBonusDebugState(GameController controller) : base(controller)
    {
    }
    public override void Setup()
    {
        controller.state = new StartState(controller);

        controller.players = new Player[] { new Player(Color.green, new SFElement()), new Player(Color.yellow, new SFElement()), new Player(Color.blue, new SFElement()), new Player(Color.red, new SFElement()), };
        controller.mainPlayer = controller.players[0];
        controller.HUD.GetComponent<HUDScript>().SetPlayers(controller.players);
        controller.HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

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
        CommonMapSetup();
    }
}