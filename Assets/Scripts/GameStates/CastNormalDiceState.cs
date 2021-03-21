using System;
using UnityEngine;

public class CastNormalDiceState : GameState
{
    public CastNormalDiceState(GameController controller) : base(controller)
    {
        hudScript.OpenNormalDiceThrowRenderer(DiceValueThrown);
        hudScript.SetStateText("CastNormalDiceState");
    }

    public void DiceValueThrown(DiceThrow diceThrow)
    {
        Debug.Log("Total dice value is: " + diceThrow.GetValue());

        controller.PayoutPlayers(new DiceThrow(1, 2));
        //controller.PayoutPlayers(diceThrow);
        hudScript.CloseNormalDiceThrowRenderer();
        controller.SetState(new StartState(controller));
    }

    public override void OnBackButtonClicked()
    {
        throw new NotImplementedException();
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        throw new NotImplementedException();
    }

    public override void OnBuildUpgradeOptionClicked(Token token)
    {
        throw new NotImplementedException();
    }

    public override void OnNextButtonClicked()
    {
        controller.SetState(new StartState(controller));
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
