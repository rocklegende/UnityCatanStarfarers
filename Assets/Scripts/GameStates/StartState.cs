using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : GameState
{
    public StartState(GameController controller) : base(controller)
    {
        hudScript.SetStateText("StartState Player " + controller.currentPlayerAtTurn);
        hudScript.ShowSettleButton(false);
    }



    public override void OnNextButtonClicked()
    {
        controller.SetState(new CastShipDiceState(controller));
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        Debug.Log("nothing happening here");
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        Debug.Log("nothing happening here");
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        // TODO: very ugly here, in the dropdown an actual token instance is present for each option
        // and this token instance is used to just know what the token costs etc, building the token however creates a new instance
        // of that token
        controller.SetState(new SelectPositionForShipState(controller, token));
    }

    public override void OnBackButtonClicked()
    {
        Debug.Log("pressed back");
    }

    public override void OnBuildUpgradeOptionClicked(Token token)
    {
        controller.mainPlayer.BuildUpgrade(token);
    }

    public override void OnSettleButtonPressed()
    {
        throw new System.NotImplementedException();
    }

    public override void OnTokenCanSettle(bool canSettle, Token token)
    {
        Debug.Log("Do nothing");//throw new NotImplementedException();
    }

    public override void Setup()
    {
        throw new NotImplementedException();
    }
}