using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using com.onebuckgames.UnityStarFarers;

public abstract class RemoteActionDispatcher
{
    protected GameController gameController;
    protected Action<Dictionary<string, RemoteActionCallbackData>> allResponsesReceivedCallback;
    protected Action<RemoteActionCallbackData> singleResponseReceivedCallback;

    protected bool isWaitingForResponse = false;
    protected Dictionary<string, RemoteActionCallbackData> playerRespondedDict = new Dictionary<string, RemoteActionCallbackData>();
    protected List<Player> targetPlayers;
    protected RemoteClientAction action;

    public RemoteActionDispatcher(GameController gameController)
    {
        this.gameController = gameController;
    }

    public void SetTargets(List<Player> targetPlayers)
    {
        this.targetPlayers = targetPlayers;
    }

    public List<Player> GetTargets()
    {
        return this.targetPlayers;
    }

    public void SetAction(RemoteClientAction action)
    {
        this.action = action;
    }

    public void MakeRequest(
        Action<RemoteActionCallbackData> singleResponseReceivedCallback,
        Action<Dictionary<string, RemoteActionCallbackData>> allResponsesReceivedCallback
    )
    {
        if (isWaitingForResponse)
        {
            throw new ArgumentException("Dispatcher is still waiting for responses from a previous, aborting here..");
        }

        if (targetPlayers == null)
        {
            throw new ArgumentException("No targets are set, please set target players via SetTargets method");
        }

        if (action == null)
        {
            throw new ArgumentException("No action is set, please set target players via SetAction method");
        }

        

        if (action.isBlockingInteraction)
        {
            gameController.GetHUDScript().waitingForOtherPlayersPopup.SetActive(true);
            gameController.ActivateAllInteraction(false);
        }

        isWaitingForResponse = true;
        this.allResponsesReceivedCallback = allResponsesReceivedCallback;
        this.singleResponseReceivedCallback = singleResponseReceivedCallback;
        if (targetPlayers.Count == 0)
        {
            playerRespondedDict = new Dictionary<string, RemoteActionCallbackData>();
            OnAllResponsesReceived();
            return;
        }
        RequestTemplateMethod();
    }

    protected abstract void RequestTemplateMethod();


    protected void ResponseReceivedFromPlayer(RemoteActionCallbackData data)
    {
        Debug.Log("Received response from player: " + data.player.name);
        Debug.Log("playersRespondedDict #3: " + playerRespondedDict.ToList().Count);
        playerRespondedDict[data.player.name] = data;
        Debug.Log("playersRespondedDict #4: " + playerRespondedDict.ToList().Count);
        var singleResponseDict = new Dictionary<string, RemoteActionCallbackData>();
        singleResponseReceivedCallback(data);
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
        if (action.isBlockingInteraction)
        {
            gameController.GetHUDScript().waitingForOtherPlayersPopup.SetActive(false);
            gameController.ActivateAllInteraction(true);
        }
        allResponsesReceivedCallback?.Invoke(playerRespondedDict);
        Reset();
    }

    void Reset()
    {
        allResponsesReceivedCallback
            = null;
        isWaitingForResponse = false;
        playerRespondedDict = new Dictionary<string, RemoteActionCallbackData>();
    }
}

public class DefaultRemoteActionDispatcher : RemoteActionDispatcher {

    public DefaultRemoteActionDispatcher(GameController gameController) : base(gameController)
    {
        this.gameController = gameController;
    }

    protected override void RequestTemplateMethod()
    {
        foreach (var player in targetPlayers)
        {
            Debug.Log("num players: " + targetPlayers.Count);
            Debug.Log("playersRespondedDict #1: " + playerRespondedDict.ToList().Count);
            playerRespondedDict[player.name] = null;
            Debug.Log("Requesting response from player: " + player.name);
            Debug.Log("playersRespondedDict #2: " + playerRespondedDict.ToList().Count);
            gameController.RunRPC(
                "RemoteClientRequiresAction",
                gameController.gameClient.SFPlayerToPhotonPlayer(player),
                ResponseReceivedFromPlayer,
                SFFormatter.Serialize(action)
            );
        }
    }
}

public class MockRemoteActionDispatcher : RemoteActionDispatcher
{

    public MockRemoteActionDispatcher(GameController gameController) : base(gameController)
    {
        this.gameController = gameController;
    }



    protected override void RequestTemplateMethod()
    {
        //foreach (var player in targetPlayers)
        //{
        //    Debug.Log("num players: " + targetPlayers.Count);
        //    Debug.Log("playersRespondedDict #1: " + playerRespondedDict.ToList().Count);
        //    playerRespondedDict[player.name] = null;
        //    Debug.Log("Requesting response from player: " + player.name);
        //    Debug.Log("playersRespondedDict #2: " + playerRespondedDict.ToList().Count);
        //    gameController.RunRPC(
        //        "RemoteClientRequiresAction",
        //        gameController.SFPlayerToPhotonPlayer(player),
        //        ResponseReceivedFromPlayer,
        //        SFFormatter.Serialize(action)
        //    );
        //}
        var player = new Player(new SFColor(Color.red));
        player.name = "Thomas";
        singleResponseReceivedCallback(new RemoteActionCallbackData(player, true));
    }
}

public class NoResponseRemoteActionDispatcher : RemoteActionDispatcher
{

    public NoResponseRemoteActionDispatcher(GameController gameController) : base(gameController)
    {
        this.gameController = gameController;
    }

    protected override void RequestTemplateMethod()
    {

    }

    public void FakeResponseFromAllPlayers(object data)
    {
        var fakeResponseDict = new Dictionary<string, RemoteActionCallbackData>();
        foreach( var target in targetPlayers)
        {
            fakeResponseDict.Add(target.name, new RemoteActionCallbackData(target, data));
        }

        playerRespondedDict = fakeResponseDict;
        OnAllResponsesReceived();
    }

    public void FakeResponseFromSinglePlayer(string playername, object data)
    {
        var playerWithThatName = targetPlayers.Find(target => target.name == playername);
        if (playerWithThatName == null)
        {
            throw new System.ArgumentException(string.Format("Could not find player with name: {0}", playername));
        }
        singleResponseReceivedCallback(new RemoteActionCallbackData(playerWithThatName, data));
    }


}

public class PositiveResponseRemoteActionDispatcher : RemoteActionDispatcher
{

    public PositiveResponseRemoteActionDispatcher(GameController gameController) : base(gameController)
    {
        this.gameController = gameController;
    }

    protected override void RequestTemplateMethod()
    {
        foreach (var player in targetPlayers)
        {
            singleResponseReceivedCallback(new RemoteActionCallbackData(player, true));
        }
        
    }
}

public class NegativeResponseRemoteActionDispatcher : RemoteActionDispatcher
{

    public NegativeResponseRemoteActionDispatcher(GameController gameController) : base(gameController)
    {
        this.gameController = gameController;
    }

    protected override void RequestTemplateMethod()
    {
        foreach (var player in targetPlayers)
        {
            singleResponseReceivedCallback(new RemoteActionCallbackData(player, false));
        }

    }
}
