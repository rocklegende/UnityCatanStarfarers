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
        controller.currentPlayerAtTurn += 1;
        if (controller.currentPlayerAtTurn == controller.players.Length)
        {
            controller.currentPlayerAtTurn = 0;
        }

        controller.SetState(new StartState(controller));
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        //TODO: dont remove and create buttons, very cost inefficient, instead create
        //all possible spacepoints and just hide and show them
        mapScript.RemoveAllSpacePointButtons(); 
        selectedToken.FlyTo(point, mapScript.map);
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        if (tokenModel.owner == controller.mainPlayer)
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
        } else
        {
            Debug.Log("Clicked token that is not owned by the main player");
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
            mapScript.SettleToken(selectedToken);
            hudScript.ShowSettleButton(false);
            selectedToken = null;
        }
    }

    public override void OnTokenCanSettle(bool canSettle, Token token)
    {
        if (canSettle)
        {
            hudScript.ShowSettleButton(true);
        }
        else
        {
            hudScript.ShowSettleButton(false);
        }
    }
}