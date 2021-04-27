using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public abstract class ImmediateAction
{
    protected Action isDoneCallback;
    public ImmediateAction(Action isDoneCallback)
    {
        this.isDoneCallback = isDoneCallback;
    }

    public void IsDone()
    {
        isDoneCallback();
    }

    public abstract void StartAction();
}


public class TakeResourceFromOpponent : ImmediateAction
{
    Player beneficialPlayer;
    int numResources;
    int numPlayersToRobFrom;
    HUDScript hudScript;
    List<Player> selectablePlayers;

    public TakeResourceFromOpponent(HUDScript hudScript, Player beneficialPlayer, Action isDoneCallback, int numResources, int numPlayersToRobFrom) : base(isDoneCallback)
    {
        this.beneficialPlayer = beneficialPlayer;
        this.numResources = numResources;
        this.hudScript = hudScript;
        this.numPlayersToRobFrom = numPlayersToRobFrom;
    }

    public override void StartAction()
    {
        selectablePlayers = hudScript.players
            .Where(player => player.color != beneficialPlayer.color && player.hand.Count() > 0)
            .ToList();
        hudScript.OpenPlayerSelection(selectablePlayers, IndexesPicked, numPlayersToRobFrom);
        // TODO: generalState.Show("Player A takes resources from opponents")
    }

    void IndexesPicked(List<int> indexes)
    {
        foreach(var index in indexes)
        {
            var player = selectablePlayers[index];
            var handToPay = player.hand.GetRandomHand(numResources);
            player.PayToOtherPlayer(beneficialPlayer, handToPay);
        }
        hudScript.playerSelectionView.SetActive(false);
        IsDone();
    }
}
