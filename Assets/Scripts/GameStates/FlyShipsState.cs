using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyShipsState : GameState
{
    GameController controller;
    Token selectedToken;
    HUDScript hudScript;
    MapScript mapScript;
    public FlyShipsState(GameController controller) : base(controller)
    {
        this.controller = controller;
        hudScript = controller.HUD.GetComponent<HUDScript>();
        mapScript = controller.Map.GetComponent<MapScript>();
        Init();
    }

    public void Init()
    {
        hudScript.SetStateText("FlyShipsState");
        hudScript.ShowSettleButton(false);
    }

    public override void OnNextButtonClicked()
    {
        // TODO: pass turn to next player
        controller.SetState(new StartState(controller));
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        mapScript.RemoveAllSpacePointButtons();
        selectedToken.FlyTo(point, mapScript.map);
        if (mapScript.map.TokenCanSettle(selectedToken, new Player[] { controller.player }))
        {
            hudScript.ShowSettleButton(true);
        } else
        {
            hudScript.ShowSettleButton(false);
        }
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        selectedToken = tokenModel;
        if (selectedToken.CanFly())
        {
            var filters = new SpacePointFilter[] {
                    new IsValidSpacePointFilter(),
                    new IsSpacePointFreeFilter(),
                    new IsStepsAwayFilter(tokenModel.position, selectedToken.GetStepsLeft())
                };
            controller.Map.GetComponent<MapScript>().ShowSpacePointsFulfillingFilters(filters);
        }
    }

    public override void OnShipDiceThrown(ShipDiceThrow shipDiceThrow)
    {
        throw new NotImplementedException();
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        Debug.Log("not reacting");
    }

    public override void OnBackButtonClicked()
    {
        Debug.Log("pressed back");
    }

    public override void OnBuildUpgradeOptionClicked(Token token)
    {
        Debug.Log("not reacting");
    }

    public override void OnSettleButtonPressed()
    {
        if (selectedToken != null)
        {
            selectedToken.settle();
            hudScript.ShowSettleButton(false);
            selectedToken = null;
        }
    }
}