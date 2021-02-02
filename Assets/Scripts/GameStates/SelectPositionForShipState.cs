using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPositionForShipState : GameState
{
    GameController controller;
    Token token;
    public SelectPositionForShipState(GameController controller, Token token) : base(controller)
    {
        this.controller = controller;
        this.token = token;
        Init();
    }

    public void Init()
    {
        if (token is TradeBaseToken || token is ColonyBaseToken)
        {
            var filters = new SpacePointFilter[] {
                new IsValidSpacePointFilter(),
                new IsSpacePointFreeFilter(),
                new IsNeighborOwnSpacePortFilter()
            };
            controller.Map.GetComponent<MapScript>().ShowSpacePointsFulfillingFilters(filters);
        } else if (token is SpacePortToken)
        {
            var tokenFilters = new TokenFilter[] {
                new IsSettledColonyFilter()
            };
            controller.Map.GetComponent<MapScript>().HighlightTokensFullfillingFilters(tokenFilters);
        } else
        {

        }
        
    }

    public override void OnNextButtonClicked()
    {
        Debug.Log("jo1");
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        controller.Map.GetComponent<MapScript>().RemoveAllSpacePointButtons();

        // TODO: check if it can be build
        controller.player.BuildToken(token, point);
        controller.Map.GetComponent<MapScript>().DisplayToken(token);

        controller.SetState(new StartState(controller));

        // TODO: notification should be send from player model, it should notifies others that it changed
        controller.app.Notify(SFNotification.player_data_changed, controller);

        Debug.Log("jo2");
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        Debug.Log("nothing happening here");
        if (tokenModel is ColonyBaseToken)
        {
            tokenModel.attachToken(new SpacePortToken());
            controller.app.Notify(SFNotification.token_data_changed, controller); //TODO: send notification from inside the token
            controller.SetState(new StartState(controller));
        }
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