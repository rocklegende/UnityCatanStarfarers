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


public class GameController : SFController
{
    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case SFNotification.token_was_selected:
                state.OnTokenClicked((Token)p_data[0], (GameObject)p_data[1]);
                break;
            case SFNotification.spacepoint_selected:
                var tiles = mapModel.getTilesAtPoint((SpacePoint)p_data[0]);
                state.OnSpacePointClicked((SpacePoint)p_data[0], (GameObject)p_data[1]);
                break;

            case SFNotification.HUD_build_token_btn_clicked:
                state.OnBuildShipOptionClicked((Token)p_data[0]);
                break;

            case SFNotification.HUD_build_upgrade_btn_clicked:
                state.OnBuildUpgradeOptionClicked((Token)p_data[0]);
                break;

            case SFNotification.next_button_clicked:
                state.OnNextButtonClicked();
                break;

            case SFNotification.settle_button_clicked:
                state.OnSettleButtonPressed();
                break;

            case SFNotification.dice_thrown:
                //PayoutPlayers((DiceThrow)p_data[0]);
                PayoutPlayers(new DiceThrow(2, 1));
                break;

            case SFNotification.token_data_changed:
                if (mapModel != null)
                {
                    mapModel.OnTokenDataChanged((Token) p_data[0]);
                }
                break;

        }
    }

    public GameObject HUD;
    public GameObject Map;
    public Map mapModel;

    public GameState state;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        var state = new TwoTradeShipAndOneSpacePort(this);
        state.Setup();

        //state = new FlyShipsState(this);

        //player = new Player(Color.blue, this);
        //HUD.GetComponent<HUDScript>().SetPlayer(player);
        //HUD.GetComponent<HUDScript>().isReceivingNotifications = true;

        //for (int i = 0; i < 5; i++)
        //{
        //    player.AddCard(new GoodsCard());
        //    player.AddCard(new FuelCard());
        //    player.AddCard(new CarbonCard());
        //    player.AddCard(new FoodCard());
        //    player.AddCard(new OreCard());
        //}

        //Token spacePort = new ColonyBaseToken();
        //spacePort.attachedToken = new SpacePortToken();
        //spacePort.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 1));
        //player.BuildToken(spacePort);

        //Token colonyShip = new ColonyBaseToken();
        //colonyShip.attachedToken = new ShipToken();
        //colonyShip.SetPosition(new SpacePoint(new HexCoordinates(5, 5), 0));
        //player.BuildToken(colonyShip);




        //MapGenerator generator = new MapGenerator();
        //mapModel = generator.GenerateRandomMap();

        //Map.GetComponent<MapScript>().SetMap(mapModel);
        //Map.GetComponent<MapScript>().SetPlayers(new Player[] { player });
        //Map.GetComponent<MapScript>().isReceivingNotifications = true;

    }

    public void SetState(GameState state)
    {
        this.state = state;
    }


    // just for testing
    void PayoutPlayers(DiceThrow dt) 
    {
        List<ResourceCard> payout = new List<ResourceCard>();
        foreach (Token token in player.tokens)
        {
            Tile_[] tiles = mapModel.getTilesAtPoint(token.position);
            ResourceTile[] resourceTiles = mapModel.GetTilesOfType<ResourceTile>(tiles);
            foreach (ResourceTile resourceTile in resourceTiles)
            {
                DiceChip diceChip = resourceTile.diceChip;
                if (diceChip.isFaceUp && diceChip.fulfillsDiceThrow(dt.GetValue()))
                {
                    var helper = new Helper();
                    var numResources = token.GetResourceProductionMultiplier();
                    for (int i = 0; i < numResources; i++)
                    {
                        ResourceCard resourceCard = helper.CreateResourceCardFromResource(resourceTile.resource);
                        player.AddCard(resourceCard);
                        payout.Add(resourceCard);
                    }                    
                }
            } 
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
