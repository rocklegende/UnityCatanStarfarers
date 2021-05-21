using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using com.onebuckgames.UnityStarFarers;

public class RemoteActionDispatcher
{
    GameController gameController;
    Action<Dictionary<string, RemoteActionCallbackData>> isDoneCallback;
    bool isWaitingForResponse = false;
    Dictionary<string, RemoteActionCallbackData> playerRespondedDict = new Dictionary<string, RemoteActionCallbackData>();
    public RemoteActionDispatcher(GameController gameController)
    {
        this.gameController = gameController;
    }

    public void RequestActionFromPlayers(List<Player> players, RemoteClientAction action, Action<Dictionary<string, RemoteActionCallbackData>> isDoneCallback)
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
            playerRespondedDict[player.name] = null;
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

    void ResponseReceivedFromPlayer(RemoteActionCallbackData data)
    {
        Debug.Log("Received response from player: " + data.player.name);
        Debug.Log("playersRespondedDict #3: " + playerRespondedDict.ToList().Count);
        playerRespondedDict[data.player.name] = data;
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
            if (entry.Value == null)
            {
                return false;
            }
        }
        return true;
    }

    public void OnAllResponsesReceived()
    {
        Debug.Log("Received response from every player!!!!");
        isDoneCallback(playerRespondedDict);
        Reset();
    }

    void Reset()
    {
        isDoneCallback = null;
        isWaitingForResponse = false;
        playerRespondedDict = new Dictionary<string, RemoteActionCallbackData>();
    }
}
