using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState
{
    GameController controller;
    public GameState(GameController controller)
    {
        this.controller = controller;
    }

    public abstract void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject);
    public abstract void OnTokenClicked(Token tokenModel, GameObject tokenGameObject);
    public abstract void OnNextButtonClicked();
    public abstract void OnBackButtonClicked();
    public abstract void OnBuildShipOptionClicked(Token token);
    public abstract void OnBuildUpgradeOptionClicked(Token token);
    

}

public class StartState : GameState
{
    GameController controller;
    public StartState(GameController controller) : base(controller)
    {
        this.controller = controller;
    }

    public override void OnNextButtonClicked()
    {
        Debug.Log("jo1");
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        Debug.Log("jo2");
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        Debug.Log("nothing happening here");
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        controller.Map.GetComponent<MapScript>().ShowAllAvailableSpacePoints();
        controller.SetState(new SelectPositionForShipState(controller, token));
    }

    public override void OnBackButtonClicked()
    {
        Debug.Log("pressed back");
    }

    public override void OnBuildUpgradeOptionClicked(Token token)
    {
        controller.player.BuildUpgrade(token);
        controller.app.Notify(SFNotification.player_data_changed, controller); //TODO: should be done in model class
    }
}

public class SelectPositionForShipState : GameState
{
    GameController controller;
    Token token;
    public SelectPositionForShipState(GameController controller, Token token) : base(controller)
    {
        this.controller = controller;
        this.token = token;
    }

    public override void OnNextButtonClicked()
    {
        Debug.Log("jo1");
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        // TODO: build ship at position
        controller.Map.GetComponent<MapScript>().RemoveAllSpacePointButtons();

        // check if it can be build
        controller.player.BuildToken(token);

        token.SetPosition(point);
        controller.Map.GetComponent<MapScript>().DisplayToken(token);

        controller.SetState(new StartState(controller));
        controller.app.Notify(SFNotification.player_data_changed, controller);

        Debug.Log("jo2");
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        Debug.Log("nothing happening here");
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        Debug.Log("jo");
    }

    public override void OnBackButtonClicked()
    {
        Debug.Log("pressed back");
    }

    public override void OnBuildUpgradeOptionClicked(Token token)
    {
        Debug.Log("sd");
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
                state.OnSpacePointClicked((SpacePoint)p_data[0], (GameObject)p_data[1]);
                break;

            case SFNotification.HUD_build_token_btn_clicked:
                state.OnBuildShipOptionClicked((Token)p_data[0]);
                break;

            case SFNotification.HUD_build_upgrade_btn_clicked:
                state.OnBuildUpgradeOptionClicked((Token)p_data[0]);
                break;
        }
    }

    public GameObject HUD;
    public GameObject Map;

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

        HUD.GetComponent<HUDScript>().SetPlayer(player);
        player.hand.AddCard(new CarbonCard());
        app.Notify(SFNotification.player_data_changed, this);
    }

    public void SetState(GameState state)
    {
        this.state = state;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
