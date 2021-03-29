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
    int playersToChooseFrom;
    HUDScript hudScript;

    public TakeResourceFromOpponent(HUDScript hudScript, Player beneficialPlayer, Action isDoneCallback, int numResources, int playersToChooseFrom) : base(isDoneCallback)
    {
        this.beneficialPlayer = beneficialPlayer;
        this.numResources = numResources;
        this.hudScript = hudScript;
        this.playersToChooseFrom = playersToChooseFrom;
    }

    public override void StartAction()
    {
        var playersWithoutMainPlayer = hudScript.players.Where(player => player.color != beneficialPlayer.color).ToArray();

        var selectionView = hudScript.playerSelectionView.GetComponent<PlayerSelectionView>();

        if (playersToChooseFrom == 1)
        {
            selectionView.multiselect = false;
        } else
        {
            selectionView.multiselect = true;
            selectionView.multiSelectMaxSelectable = playersToChooseFrom;
        }

        hudScript.OpenPlayerSelection(playersWithoutMainPlayer, PlayersPicked);
        // generalState.Show("Player A takes resources from opponents")
    }

    void PlayersPicked(List<Player> players)
    {

        foreach(var player in players)
        {
            var handToPay = player.hand.GetRandomHand(numResources);
            player.PayToOtherPlayer(beneficialPlayer, handToPay);
        }
        hudScript.playerSelectionView.SetActive(false);
        IsDone();
    }
}
