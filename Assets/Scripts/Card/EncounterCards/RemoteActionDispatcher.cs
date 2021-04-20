using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using com.onebuckgames.UnityStarFarers;

public class RemoteActionDispatcher
{
    GameController gameController;
    Action isDoneCallback;
    bool isWaitingForResponse = false;
    Dictionary<Player, bool> playerRespondedDict = new Dictionary<Player, bool>();
    public RemoteActionDispatcher(GameController gameController)
    {
        this.gameController = gameController;
    }

    public void RequestActionFromPlayers(List<Player> players, RemoteClientAction action, Action isDoneCallback)
    {
        if (isWaitingForResponse)
        {
            throw new ArgumentException("Dispatcher is still waiting for responses, aborting");
        }
        isWaitingForResponse = true;
        this.isDoneCallback = isDoneCallback;
        foreach (var player in players)
        {
            Debug.Log("num players: " + players.Count);
            Debug.Log("playersRespondedDict #1: " + playerRespondedDict.ToList().Count);
            playerRespondedDict[player] = false;
            Debug.Log("Requesting response from player: " + player.name);
            Debug.Log("playersRespondedDict #2: " + playerRespondedDict.ToList().Count);
            gameController.RunRPC(
                "RemoteClientRequiresAction",
                gameController.SFPlayerToPhotonPlayer(player),
                ResponseReceivedFromPlayer,
                SFFormatter.Serialize(action)
            );
        }
    }

    public void ResponseReceivedFromPlayer(Player player)
    {
        Debug.Log("Received response from player: " + player.name);
        Debug.Log("playersRespondedDict #3: " + playerRespondedDict.ToList().Count);
        playerRespondedDict[player] = true;
        Debug.Log("playersRespondedDict #4: " + playerRespondedDict.ToList().Count);
        if (AllPlayersResponded())
        {
            OnAllResponsesReceived();
        }
    }

    public bool AllPlayersResponded()
    {
        Debug.Log("playersRespondedDict #5: " + playerRespondedDict.ToList().Count);
        foreach (var entry in playerRespondedDict)
        {
            if (entry.Value == false)
            {
                return false;
            }
        }
        return true;
    }

    public void OnAllResponsesReceived()
    {
        Debug.Log("Received response from every player!!!!");
        isDoneCallback();
        Reset();
    }

    void Reset()
    {
        isDoneCallback = null;
        isWaitingForResponse = false;
        playerRespondedDict = new Dictionary<Player, bool>();
    }
}
