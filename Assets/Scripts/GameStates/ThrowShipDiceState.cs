using System;
using UnityEngine;

public class CastShipDiceState : GameState
{
    GameController controller;
    public CastShipDiceState(GameController controller) : base(controller)
    {
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
        throw new NotImplementedException();
    }

    public override void OnSettleButtonPressed()
    {
        throw new NotImplementedException();
    }

    public override void OnShipDiceThrown(ShipDiceThrow shipDiceThrow)
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
}
