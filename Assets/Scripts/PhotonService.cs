using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using com.onebuckgames.UnityStarFarers;
using System;

public class PhotonService : MonoBehaviourPunCallbacks
{
    public SFGameClient_ sfGameClient;

    public void SaveGameStateToProps(SFGameStateInfo gameStateInfo)
    {
        var message = "PHOTONSERVICE!";

        foreach (var player in gameStateInfo.players)
        {
            message += "Playername: " + player.name + "; VP: " + player.GetVictoryPoints();
        }
        Debug.Log(message);
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        if (gameStateInfo.players != null)
        {
            hashtable.Add("players", SFFormatter.Serialize(gameStateInfo.players));
        }
        if (gameStateInfo.mapmodel != null)
        {
            hashtable.Add("map", SFFormatter.Serialize(gameStateInfo.mapmodel));
        }
        if (gameStateInfo.playerToActTurnOrderPosition != -1)
        {
            hashtable.Add("currentPlayerAtTurn", gameStateInfo.playerToActTurnOrderPosition);
        }
        if (gameStateInfo.state != null)
        {
            hashtable.Add("state", gameStateInfo.state);
        }
        if (gameStateInfo.turnNumber != -1)
        {
            hashtable.Add("turnNumber", gameStateInfo.turnNumber);
        }
        if (gameStateInfo.turnType != null)
        {
            hashtable.Add("turnType", gameStateInfo.turnType);
        }
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

        //var hashtable = new ExitGames.Client.Photon.Hashtable();
        //hashtable.Add("key", 1);
        //PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

        long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        Debug.Log("send timestamp: " + milliseconds);

    }

    public SFGameStateInfo GetGameStateFromProps(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        var gameStateInfo = new SFGameStateInfo();
        if (propertiesThatChanged.ContainsKey("players"))
        {
            gameStateInfo.players = (List<Player>)SFFormatter.Deserialize((byte[])propertiesThatChanged["players"]);
        }
        if (propertiesThatChanged.ContainsKey("map"))
        {
            gameStateInfo.mapmodel = (Map)SFFormatter.Deserialize((byte[])propertiesThatChanged["map"]);
        }
        if (propertiesThatChanged.ContainsKey("currentPlayerAtTurn"))
        {
            gameStateInfo.playerToActTurnOrderPosition = (int)propertiesThatChanged["currentPlayerAtTurn"];
        }
        if (propertiesThatChanged.ContainsKey("state"))
        {
            gameStateInfo.state = (string)propertiesThatChanged["state"];
        }
        if (propertiesThatChanged.ContainsKey("turnNumber"))
        {
            gameStateInfo.turnNumber = (int)propertiesThatChanged["turnNumber"];
        }
        if (propertiesThatChanged.ContainsKey("turnType"))
        {
            gameStateInfo.turnType = (TurnType)propertiesThatChanged["turnType"];
        }
        //var message = "PHOTONSERVICE 2!";

        //foreach (var player in gameStateInfo.players)
        //{
        //    message += "Playername: " + player.name + "; VP: " + player.GetVictoryPoints();
        //}
        //Debug.Log(message);
        //var hashtable = new ExitGames.Client.Photon.Hashtable();
        return gameStateInfo;
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        Debug.Log("received timestamp: " + milliseconds);
        var gameState = GetGameStateFromProps(propertiesThatChanged);
        sfGameClient.GameStateInfoChanged(gameState);
    }

}
