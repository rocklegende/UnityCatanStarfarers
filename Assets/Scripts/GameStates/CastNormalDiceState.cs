using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.onebuckgames.UnityStarFarers;

public class On7RollStrategy
{
    GameController gameController;
    public On7RollStrategy(GameController gameController)
    {
        this.gameController = gameController;
    }

    public static int NumCardsToDump(int numCardsOnHand)
    {
        return numCardsOnHand / 2;
    }

    public void Execute()
    {
        var players = gameController.players;
        var remoteClientAction = new RemoteClientAction(
            RemoteClientActionType.SEVEN_ROLL_DISCARD,
            null,
            gameController.mainPlayer
        );
        var dispatcher = new RemoteActionDispatcher(gameController);
        dispatcher.RequestActionFromPlayers(
            players.Where(player => player.ExceedsDiscardLimit()).ToList(),
            remoteClientAction,
            PlayersDumpedCards
        );
    }

    void PlayersDumpedCards(Dictionary<string, RemoteActionCallbackData> dict)
    {
        var selectablePlayers = gameController.players.Where(p => p.hand.Count() > 0 && p.name != gameController.mainPlayer.name).ToList();
        gameController.GetHUDScript().OpenPlayerSelection(
            selectablePlayers,
            ((selectedIndexes) => {
                if (selectedIndexes.Count > 0)
                {
                    var selectedPlayer = selectablePlayers[selectedIndexes[0]];
                    selectedPlayer.PayToOtherPlayer(gameController.mainPlayer, selectedPlayer.hand.GetRandomSubhandOfSize(1));
                }
                PlayerPickedCardFromOpponent();
            }),
            1);
    }

    void PlayerPickedCardFromOpponent()
    {
        GivePlayersCardsFromDrawPile(gameController.drawPileHandler.availablePiles, gameController.players, gameController.currentPlayerAtTurnIndex);
    }

    void GivePlayersCardsFromDrawPile(AvailablePiles availablePiles, List<Player> players, int currentPlayerIndex)
    {
        foreach (var index in Helper.NextPlayersClockwise(currentPlayerIndex, players.Count))
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

        SwitchToNextState();
    }

    void SwitchToNextState()
    {
        hudScript.CloseNormalDiceThrowRenderer();
        controller.SetState(new BuildAndTradeState(controller));
    }

    public override void OnBackButtonClicked()
    {
        //throw new NotImplementedException();
    }

    public override void OnBuildShipOptionClicked(Token token)
    {
        //throw new NotImplementedException();
    }

    public override void OnBuildUpgradeOptionClicked(Upgrade upgrade)
    {
        //throw new NotImplementedException();
    }

    public override void OnNextButtonClicked()
    {
        if (GameConstants.isDevelopment)
        {
            SwitchToNextState();
        } else
        {
            Debug.Log("Clicked next button in cast normal dice state, not reacting to this");
        }
    }

    public override void OnSettleButtonPressed()
    {
        //throw new NotImplementedException();
    }

    public override void OnSpacePointClicked(SpacePoint point, GameObject spacePointObject)
    {
        //throw new NotImplementedException();
    }

    public override void OnTokenCanSettle(bool canSettle, Token token)
    {
        //throw new NotImplementedException();
    }

    public override void OnTokenClicked(Token tokenModel, GameObject tokenGameObject)
    {
        //throw new NotImplementedException();
    }

    public override void Setup()
    {
        //throw new NotImplementedException();
    }

    public override void OnGameDataChanged()
    {
        //throw new NotImplementedException();
    }
}
