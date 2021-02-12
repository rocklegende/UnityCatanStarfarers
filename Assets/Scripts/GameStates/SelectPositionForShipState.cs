﻿using System;
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
        controller.HUD.GetComponent<HUDScript>().SetStateText("SelectPositionForShipState");
        controller.HUD.GetComponent<HUDScript>().ShowSettleButton(false);
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

    public override void OnShipDiceThrown(ShipDiceThrow shipDiceThrow)
    {
        throw new NotImplementedException();
    }

    public override void OnNextButtonClicked()
    {
        controller.SetState(new FlyShipsState(controller));
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        controller.Map.GetComponent<MapScript>().RemoveAllSpacePointButtons();
        controller.player.BuildToken(token, point);
        controller.SetState(new StartState(controller));
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {

        if (tokenModel is ColonyBaseToken)
        {
            tokenModel.attachToken(new SpacePortToken());
            //controller.app.Notify(SFNotification.token_data_changed, controller); //TODO: send notification from inside the token
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

    public override void OnSettleButtonPressed()
    {
        throw new System.NotImplementedException();
    }
}