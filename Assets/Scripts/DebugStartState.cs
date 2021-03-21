using System.Collections;
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
        return new Player[] { new Player(Color.green, controller), new Player(Color.red, controller) };
    }

    public void BuildShipsForMainPlayer(Player player)
    {
        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 1));
        player.BuildTokenWithoutCost(spacePort);

        Token colonyShip = new TradeBaseToken();
        colonyShip.attachedToken = new ShipToken();
        colonyShip.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 0));
        player.BuildTokenWithoutCost(colonyShip);

        Token colonyShip2 = new ColonyBaseToken();
        colonyShip2.attachedToken = new ShipToken();
        colonyShip2.SetPosition(new SpacePoint(new HexCoordinates(5, 5).W(), 0));
        player.BuildTokenWithoutCost(colonyShip2);
    }

    public void BuildShipsForSecondPlayer(Player player)
    {
        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(7, 4), 0));
        player.BuildTokenWithoutCost(spacePort);

        Token colonyShip = new TradeBaseToken();
        colonyShip.attachedToken = new ShipToken();
        colonyShip.SetPosition(new SpacePoint(new HexCoordinates(7, 4), 1));
        player.BuildTokenWithoutCost(colonyShip);

        Token colonyShip2 = new ColonyBaseToken();
        colonyShip2.attachedToken = new ShipToken();
        colonyShip2.SetPosition(new SpacePoint(new HexCoordinates(7, 4).E(), 1));
        player.BuildTokenWithoutCost(colonyShip2);
    }

    public void CommonSetup()
    {
        controller.players = GetGenericPlayers();
        controller.mainPlayer = controller.players[0];
        controller.HUD.GetComponent<HUDScript>().SetPlayers(controller.players);
        controller.HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

        for (int i = 0; i < 10; i++)
        {
            controller.mainPlayer.AddCard(new GoodsCard());
            controller.mainPlayer.AddCard(new FuelCard());
            controller.mainPlayer.AddCard(new CarbonCard());
            controller.mainPlayer.AddCard(new FoodCard());
            controller.mainPlayer.AddCard(new OreCard());
        }
    }

    public void CommonMapSetup()
    {
        MapGenerator generator = new MapGenerator();
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

        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 1));
        controller.mainPlayer.BuildToken(spacePort);

        Token colonyShip = new ColonyBaseToken();
        colonyShip.attachedToken = new ShipToken();
        colonyShip.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 0));
        controller.mainPlayer.BuildToken(colonyShip);

        CommonMapSetup();
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

        BuildShipsForMainPlayer(controller.players[0]);
        BuildShipsForSecondPlayer(controller.players[1]);       

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
        BuildShipsForMainPlayer(controller.players[0]);

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
        BuildShipsForMainPlayer(controller.players[0]);

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
        BuildShipsForMainPlayer(controller.players[0]);

        for (int i = 0; i < 5; i++)
        {
            controller.players[0].ship.Add(new BoosterUpgradeToken());
            controller.players[0].ship.Add(new FreightPodUpgradeToken());
            controller.players[0].ship.Add(new CannonUpgradeToken());
        }

        controller.mainPlayer.AddFameMedal();
        CommonMapSetup();
    }
}