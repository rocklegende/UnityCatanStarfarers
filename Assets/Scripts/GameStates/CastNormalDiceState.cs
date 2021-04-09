using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class On7RollStrategy
{
    public On7RollStrategy()
    {

    }

    public void Execute(Player[] players, int currentPlayerIndex, AvailablePiles availablePiles)
    {
        foreach (var player in players) {

            if (player.hand.Count() > player.GetDiscardLimit())
            {
                //player needs to dump cards
                //TODO: hudOfThatPlayer.RequestThrowingAwayResources
            }
        }

        //TODO: players[currentPlayerIndex].ChooseCardFromOpponent(1);

        foreach (var index in Helper.NextPlayersClockwise(currentPlayerIndex, players.Length))
        {
            var c = availablePiles.hiddenDrawPile.DrawCardsFromTop(1);
            players[index].AddCard(c[0]);
            //TODO: this could fail if no resources are left maybe
        }
    }

    
}

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

        //TODO: remove comment
        //if (diceThrow.GetValue() == 7)
        //{
        //    controller.On7Rolled();
        //} else
        //{
        //    controller.PayoutPlayers(new DiceThrow(1, 2));
        //    //controller.PayoutPlayers(diceThrow);
        //}
        controller.PayoutPlayers(new DiceThrow(1, 2));

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
