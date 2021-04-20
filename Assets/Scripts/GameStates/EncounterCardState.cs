using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCardState : GameState
{
    public EncounterCardState(GameController controller) : base(controller)
    {
        Init();
    }

    public void Init()
    {
        hudScript.SetStateText("EncounterCardState");
        hudScript.ShowSettleButton(false);
        //TODO insert this line: controller.encounterCardHandler.GetComponent<EncounterCardHandler>().PlayNextCard();
    }

    void EncounterCardFinished()
    {

    }

    public override void OnNextButtonClicked()
    {
        //
        Debug.Log("not reacting");
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        //
        Debug.Log("not reacting");
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        //
        Debug.Log("not reacting");
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
        Debug.Log("not reacting");
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

    public override void Setup()
    {
        throw new NotImplementedException();
    }
}