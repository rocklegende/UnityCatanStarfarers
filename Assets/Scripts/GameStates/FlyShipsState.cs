using System;
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
    }

    

    public override void OnNextButtonClicked()
    {
        controller.PassTurnToNextPlayer();
        controller.SetState(new CastNormalDiceState(controller));
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        //TODO: dont remove and create buttons, very cost inefficient and slow, instead create
        //all possible spacepoints and just hide and show the one we need right now
        mapScript.RemoveAllSpacePointButtons(); 
        selectedToken.FlyTo(point, mapScript.map);

        var errors = flightStateChecker.Check(controller.mapModel, controller.players);
        Debug.Log("");

    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        if (tokenModel.owner == controller.mainPlayer)
        {
            selectedToken = tokenModel;
            if (selectedToken.CanFly())
            {
                var filters = selectedToken.GetFlightEndPointsFilters();
                //var filters = new SpacePointFilter[] {
                //    new IsValidSpacePointFilter(),
                //    new IsSpacePointFreeFilter(),
                //    new IsStepsAwayFilter(tokenModel.position, selectedToken.GetStepsLeft())
                //};
                mapScript.ShowSpacePointsFulfillingFilters(filters);
            }
        } else
        {
            Debug.Log("Clicked token that is not owned by the main player");
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

    public override void Setup()
    {
        throw new NotImplementedException();
    }
}