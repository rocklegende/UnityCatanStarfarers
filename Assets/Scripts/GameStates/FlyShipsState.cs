using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyShipsState : GameState
{
    Token selectedToken;
    FlightStateConsistencyChecker flightStateChecker;
    public FlyShipsState(GameController controller) : base(controller)
    {
        Init();
    }

    public void Init()
    {
        hudScript.SetStateText("FlyShipsState");
        hudScript.ShowSettleButton(false);
        flightStateChecker = new FlightStateConsistencyChecker();
        OpenTokenSelectionForAllFlyableTokens();
    }

    void OpenTokenSelectionForAllFlyableTokens()
    {
        var tokens = controller.mainPlayer.tokens.Where(tok => tok.CanFly());

        mapScript.OpenTokenSelection(controller.mainPlayer.GetTokensThatCanFly(), TokenSelected);
    }

    void TokenSelected(Token token)
    {
        selectedToken = token;
        OpenSpacePointSelectionForToken(selectedToken);
        //mapScript.SettleToken(selectedToken);
    }

    void OpenSpacePointSelectionForToken(Token token)
    {
        var spacePoints = token.ReachableSpacePoints();
        mapScript.OpenSpacePointSelection(spacePoints, SpacePointSelected);
    }

    void SpacePointSelected(SpacePoint point)
    {
        mapScript.HideAllSpacePointButtons();
        var isOwnerBefore = selectedToken.owner == controller.mainPlayer;
        selectedToken.FlyTo(point);
        var isOwnerAfter = selectedToken.owner == controller.mainPlayer;

        var errors = flightStateChecker.Check(controller.mapModel);

        //if (controller.mainPlayer.GetTokensThatCanFly().Count > 0)
        //{
        //    OpenTokenSelectionForAllFlyableTokens();
        //}
    }

    void SwitchToNextState()
    {
        controller.PassTurnToNextPlayer();
        controller.SetState(new CastNormalDiceState(controller));
        mapScript.CloseTokenSelection();
    }

    public override void OnNextButtonClicked()
    {
        SwitchToNextState();
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {

    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        

    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        Debug.Log("not reacting");
    }

    public override void OnBackButtonClicked()
    {
        Debug.Log("pressed back");
    }

    public override void OnBuildUpgradeOptionClicked(Upgrade upgrade)
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

            if (controller.mainPlayer.GetTokensThatCanFly().Count > 0)
            {
                mapScript.CloseTokenSelection();
                controller.SetState(new FlyShipsState(controller));
            }
            else
            {
                // if we switch here, were triggering an update of the gamedata, this interferes with updates from previous stuff
                //SwitchToNextState();
            }

        }
    }

    void UpdateSelectedTokenFromGameData()
    {
        //make sure we have the correct selected token,
        //because if the whole game data changes we have a selected token
        //with wrong references (token owner is not the actual object anymore and so on)
        selectedToken = controller.mainPlayer.tokens.Find(tok => tok.guid == selectedToken.guid);
    }

    public override void OnGameDataChanged()
    {
        UpdateSelectedTokenFromGameData();
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