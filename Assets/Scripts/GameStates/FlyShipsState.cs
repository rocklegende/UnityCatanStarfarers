using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyShipsState : GameState
{
    GameController controller;
    Token selectedToken;
    public FlyShipsState(GameController controller) : base(controller)
    {
        this.controller = controller;
        Init();
    }

    public void Init()
    {
        controller.HUD.GetComponent<HUDScript>().SetStateText("FlyShipsState");
    }

    public override void OnNextButtonClicked()
    {
        Debug.Log("jo1");
        // TODO: pass turn to next player
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        controller.Map.GetComponent<MapScript>().RemoveAllSpacePointButtons();
        selectedToken.FlyTo(point, controller.Map.GetComponent<MapScript>().map);
        //selectedToken.resetFlyability();
        controller.SetState(new FlyShipsState(controller));
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
        }
    }
}