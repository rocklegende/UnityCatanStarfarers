using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSetupState : GameState
{
    public InitialSetupState(GameController controller) : base(controller)
    {
        hudScript.SetStateText("Setup of " + controller.GetCurrentPlayerAtTurnIndex());
        hudScript.ShowSettleButton(false);

        var freeGameStartSettlingSpots = controller.mapModel.applyFilter(
            controller.mapModel.gamestartSettlingSpots,
            new IsSpacePointFreeFilter()
        );
        mapScript.OpenSpacePointSelection(freeGameStartSettlingSpots, (selectedSpacePoint) =>
        {
            controller.mainPlayer.BuildTokenWithoutCost(
                controller.mapModel,
                new ColonyBaseToken().GetType(),
                selectedSpacePoint
            );
            
        });
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

    public override void OnNextButtonClicked()
    {
        controller.IFinishedMyTurn();
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
        //throw new System.NotImplementedException();
    }
}
