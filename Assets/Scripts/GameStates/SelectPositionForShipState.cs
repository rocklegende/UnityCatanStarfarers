using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPositionForShipState : GameState
{
    Token token;
    public SelectPositionForShipState(GameController controller, Token token) : base(controller)
    {
        this.token = token;
        hudScript.SetStateText("SelectPositionForShipState");
        hudScript.ShowSettleButton(false);
        Init();
    }

    public void Init()
    {
        if (token is TradeBaseToken || token is ColonyBaseToken)
        {
            //TODO: let Token decide which points to show
            var filters = new SpacePointFilter[] {
                new IsValidSpacePointFilter(),
                new IsSpacePointFreeFilter(),
                new IsNeighborOwnSpacePortFilter()
            };
            controller.Map.GetComponent<MapScript>().ShowSpacePointsFulfillingFilters(filters, controller.players);
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
        controller.SetState(new FlyShipsState(controller));
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        controller.Map.GetComponent<MapScript>().HideAllSpacePointButtons();
        controller.mainPlayer.BuildToken(controller.mapModel, token.GetType(), point, new ShipToken().GetType());
        controller.SetState(new StartState(controller));
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {

        if (tokenModel is ColonyBaseToken)
        {
            tokenModel.attachToken(new SpacePortToken());
            controller.SetState(new StartState(controller));
        }
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        throw new System.NotImplementedException();
    }

    public override void OnBackButtonClicked()
    {
        throw new System.NotImplementedException();
    }

    public override void OnBuildUpgradeOptionClicked(Token token)
    {
        throw new System.NotImplementedException();
    }

    public override void OnSettleButtonPressed()
    {
        throw new System.NotImplementedException();
    }

    public override void OnTokenCanSettle(bool canSettle, Token token)
    {
        Debug.Log("received that token can settle, dow nothing");
    }

    public override void Setup()
    {
        throw new NotImplementedException();
    }
}