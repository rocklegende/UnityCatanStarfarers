using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using com.onebuckgames.UnityStarFarers;
using Photon.Pun;

public class PlayModeTestHelper
{
    public PlayModeTestHelper()
    {
    }

    public IEnumerator StartSinglePlayerGame()
    {
        // make sure we leave room if we were inside one from a previous test
        PhotonNetwork.LeaveRoom();

        yield return LoadLobbyScene();
        yield return new WaitForSeconds(2);
    }

    public IEnumerator LoadScene(int sceneBuildNumber)
    {
        SceneManager.LoadScene(sceneBuildNumber);
        yield return null;
    }

    public IEnumerator LoadDefaultScene()
    {
        SceneManager.LoadScene(SFScenes.GAME_SCENE);
        yield return null;
    }

    public IEnumerator LoadLobbyScene()
    {
        SceneManager.LoadScene(SFScenes.LOBBY_SCENE);
        yield return null;
    }

    public void DevelopmentSetup(GameController gameController)
    {
        var map = new DefaultMapGenerator().GenerateRandomMap();

        var player1 = new Player(new SFColor(Color.white));
        player1.name = "Tim";
        var player2 = new Player(new SFColor(Color.black));
        player2.name = "Paul";
        var players = new List<Player> { player1, player2
            };
        var mainPlayer = players[0];

        gameController.DevelopmentGameStartInformationGenerated(map, players, mainPlayer);
    }



    public GameController GetGameController()
    {
        var gameControllerObj = GameObject.Find("GameController");
        var gameControllerScript = gameControllerObj.GetComponent<GameController>();
        return gameControllerScript;
    }

    public void ClickSpacePointButton(SpacePoint targetPoint, MapScript mapScript)
    {
        var allSpacePointButtonScripts = mapScript.gameObject.GetComponentsInChildren<Space.SpacePointButtonScript>();
        var allSpacePointButtonScriptsList = new List<Space.SpacePointButtonScript>();
        foreach(var script in allSpacePointButtonScripts)
        {
            allSpacePointButtonScriptsList.Add(script);
        }

        var spacePointToClick =
            allSpacePointButtonScriptsList
            .Find(
                script => script.spacePoint.Equals(targetPoint)
            );
        Assert.True(spacePointToClick != null, String.Format("Couldt find SpacePoint button for point {0}", targetPoint.ToString()));
        spacePointToClick.GetComponent<Space.SpacePointButtonScript>().OnClick();
    }
}
