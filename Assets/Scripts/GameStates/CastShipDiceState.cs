﻿using System;
using UnityEngine;

public class CastShipDiceState : GameState
{
    public CastShipDiceState(GameController controller) : base(controller)
    {
        hudScript.ShowShipDiceThrowPanel(ShipValueThrown);
        hudScript.SetStateText("CastShipDiceState");
    }

    public void ShipValueThrown(ShipDiceThrow shipDiceThrow)
    {
        Debug.Log("Total ship dice value is: " + shipDiceThrow.GetRawValue() + "trigger encounter card" + shipDiceThrow.TriggersEncounterCard());

        controller.mainPlayer.AddRangeToFlyableTokens(shipDiceThrow.GetFlyValue());
        //if (shipDiceThrow.TriggersEncounterCard())
        //{
        //    controller.SetState(new EncounterCardState(controller));
        //} else
        //{
        //    controller.SetState(new FlyShipsState(controller));
        //}
        //Debug
        controller.SetState(new FlyShipsState(controller));

        hudScript.CloseShipDiceThrowPanel();
    }

    

    public override void OnBackButtonClicked()
    {
        throw new NotImplementedException();
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        throw new NotImplementedException();
    }

    public override void OnBuildUpgradeOptionClicked(Upgrade upgrade)
    {
        throw new NotImplementedException();
    }

    public override void OnNextButtonClicked()
    {
        Debug.Log("Clicked next button in throw ship dice state, not doing anything");
        if (GameConstants.isDevelopment)
        {
            hudScript.CloseShipDiceThrowPanel();
            controller.SetState(new FlyShipsState(controller));
        }
    }

    public override void OnSettleButtonPressed()
    {
        throw new NotImplementedException();
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        throw new NotImplementedException();
    }

    public override void OnTokenCanSettle(bool canSettle, Token token)
    {
        throw new NotImplementedException();
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        throw new NotImplementedException();
    }

    public override void Setup()
    {
        throw new NotImplementedException();
    }
}
