using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForOpponentState : GameState
{
    public WaitForOpponentState(GameController gameController) : base(gameController)
    {
        
    }

    public override void OnBackButtonClicked()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnBuildUpgradeOptionClicked(Upgrade upgrade)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnGameDataChanged()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnLeaveState()
    {
        //throw new System.NotImplementedException();
        controller.ActivateAllInteraction(true);
    }

    public override void OnNextButtonClicked()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnSettleButtonPressed()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTokenCanSettle(bool canSettle, Token token)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        //throw new System.NotImplementedException();
    }

    public override void Setup()
    {
        controller.ActivateAllInteraction(false);
    }
}
