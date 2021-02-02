using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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


            case SFNotification.dice_thrown:
                //PayoutPlayers((DiceThrow)p_data[0]);
                PayoutPlayers(new DiceThrow(2, 1));
                break;

        }
    }

    public GameObject HUD;
    public GameObject Map;
    private Map mapModel;

    public GameState state;
    public Player player;

    // Start is called before the first frame update
    void Start()
    {


        state = new StartState(this);

        player = new Player(Color.blue);

        for (int i = 0; i < 5; i++)
        {
            player.hand.AddCard(new GoodsCard());
            player.hand.AddCard(new FuelCard());
            player.hand.AddCard(new CarbonCard());
            player.hand.AddCard(new FoodCard());
            player.hand.AddCard(new OreCard());
        }

        Token spacePort = new ColonyBaseToken();
        spacePort.attachedToken = new SpacePortToken();
        spacePort.SetPosition(new SpacePoint(new HexCoordinates(5,5), 1));
        player.BuildToken(spacePort);

        MapGenerator generator = new MapGenerator();
        mapModel = generator.GenerateRandomMap();

        Map.GetComponent<MapScript>().SetMap(mapModel);
        Map.GetComponent<MapScript>().SetPlayers(new Player[] { player });
        HUD.GetComponent<HUDScript>().SetPlayer(player);



        player.hand.AddCard(new CarbonCard());
        app.Notify(SFNotification.player_data_changed, this);

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

        //TODO: player model should send this notification
        app.Notify(SFNotification.player_data_changed, this);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
