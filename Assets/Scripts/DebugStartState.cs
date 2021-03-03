using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DebugStartState
{
    GameController controller;
    public DebugStartState(GameController controller)
    {
        this.controller = controller;
    }

    public abstract void Setup();
}

public class OneColonyShipAndOneSpacePort : DebugStartState
{
    GameController controller;
    public OneColonyShipAndOneSpacePort(GameController controller) : base(controller)
    {
        this.controller = controller;
    }

    public override void Setup()
    {
        controller.state = new FlyShipsState(controller);

        controller.player = new Player(Color.blue, controller);
        controller.HUD.GetComponent<HUDScript>().SetPlayer(controller.player);
        controller.HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

        for (int i = 0; i < 5; i++)
        {
            controller.player.AddCard(new GoodsCard());
            controller.player.AddCard(new FuelCard());
            controller.player.AddCard(new CarbonCard());
            controller.player.AddCard(new FoodCard());
            controller.player.AddCard(new OreCard());
        }

        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 1));
        controller.player.BuildToken(spacePort);

        Token colonyShip = new ColonyBaseToken();
        colonyShip.attachedToken = new ShipToken();
        colonyShip.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 0));
        controller.player.BuildToken(colonyShip);




        MapGenerator generator = new MapGenerator();
        controller.mapModel = generator.GenerateRandomMap();

        controller.Map.GetComponent<MapScript>().SetMap(controller.mapModel);
        controller.Map.GetComponent<MapScript>().SetPlayers(new Player[] { controller.player });
        controller.Map.GetComponent<MapScript>().isReceivingNotifications = true;
    }
}

public class TwoTradeShipAndOneSpacePort : DebugStartState
{
    GameController controller;
    public TwoTradeShipAndOneSpacePort(GameController controller) : base(controller)
    {
        this.controller = controller;
    }

    public override void Setup()
    {
        controller.state = new FlyShipsState(controller);

        controller.player = new Player(Color.blue, controller);
        controller.HUD.GetComponent<HUDScript>().SetPlayer(controller.player);
        controller.HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

        for (int i = 0; i < 5; i++)
        {
            controller.player.AddCard(new GoodsCard());
            controller.player.AddCard(new FuelCard());
            controller.player.AddCard(new CarbonCard());
            controller.player.AddCard(new FoodCard());
            controller.player.AddCard(new OreCard());
        }

        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 1));
        controller.player.BuildToken(spacePort);

        Token colonyShip = new TradeBaseToken();
        colonyShip.attachedToken = new ShipToken();
        colonyShip.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 0));
        controller.player.BuildToken(colonyShip);

        Token colonyShip2 = new TradeBaseToken();
        colonyShip2.attachedToken = new ShipToken();
        colonyShip2.SetPosition(new SpacePoint(new HexCoordinates(5, 5).W(), 0));
        controller.player.BuildToken(colonyShip2);


        MapGenerator generator = new MapGenerator();
        controller.mapModel = generator.GenerateRandomMap();

        controller.Map.GetComponent<MapScript>().SetMap(controller.mapModel);
        controller.Map.GetComponent<MapScript>().SetPlayers(new Player[] { controller.player });
        controller.Map.GetComponent<MapScript>().isReceivingNotifications = true;
    }
}

public class OneTradeOneColonyShipAndOneSpacePort : DebugStartState
{
    GameController controller;
    public OneTradeOneColonyShipAndOneSpacePort(GameController controller) : base(controller)
    {
        this.controller = controller;
    }

    public override void Setup()
    {
        controller.state = new FlyShipsState(controller);

        controller.player = new Player(Color.blue, controller);
        controller.HUD.GetComponent<HUDScript>().SetPlayer(controller.player);
        controller.HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

        for (int i = 0; i < 5; i++)
        {
            controller.player.AddCard(new GoodsCard());
            controller.player.AddCard(new FuelCard());
            controller.player.AddCard(new CarbonCard());
            controller.player.AddCard(new FoodCard());
            controller.player.AddCard(new OreCard());
        }

        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 1));
        controller.player.BuildToken(spacePort);

        Token colonyShip = new TradeBaseToken();
        colonyShip.attachedToken = new ShipToken();
        colonyShip.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 0));
        controller.player.BuildToken(colonyShip);

        Token colonyShip2 = new ColonyBaseToken();
        colonyShip2.attachedToken = new ShipToken();
        colonyShip2.SetPosition(new SpacePoint(new HexCoordinates(5, 5).W(), 0));
        controller.player.BuildToken(colonyShip2);


        MapGenerator generator = new MapGenerator();
        controller.mapModel = generator.GenerateRandomMap();

        controller.Map.GetComponent<MapScript>().SetMap(controller.mapModel);
        controller.Map.GetComponent<MapScript>().SetPlayers(new Player[] { controller.player });
        controller.Map.GetComponent<MapScript>().isReceivingNotifications = true;
    }
}