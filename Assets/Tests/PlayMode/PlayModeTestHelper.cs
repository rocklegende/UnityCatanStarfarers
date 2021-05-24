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

    public readonly int ColonyShipDropdownIndex = 2;
    public readonly int TradeBaseDropDownIndex = 1;
    public readonly int SpacePortDropDownIndex = 0;

    public IEnumerator StartSinglePlayerGame()
    {
        LogAssert.ignoreFailingMessages = true;
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }

        yield return LoadPlayModeSinglePlayerLobbyScene();
        yield return new WaitForSeconds(1);
    }

    public IEnumerator LoadScene(int sceneBuildNumber)
    {
        SceneManager.LoadScene(sceneBuildNumber);
        yield return null;
    }

    public IEnumerator LoadPlayModeSinglePlayerLobbyScene()
    {
        SceneManager.LoadScene(SFScenes.SINGLE_PLAYER_LOBBY_SCENE);
        yield return null;
    }

    public IEnumerator LoadGameScene()
    {
        SceneManager.LoadScene(SFScenes.GAME_SCENE);
        yield return null;
    }

    public IEnumerator LoadLobbyScene()
    {
        SceneManager.LoadScene(SFScenes.LOBBY_SCENE);
        yield return null;
    }

    void SelectDropdownOptionAtIndex(int index, GameController gameController)
    {
        var hud = gameController.GetHUDScript();
        var dropdown = hud.buildShipsDropDownRef.GetComponent<BuildDropDown>();
        Assert.True(dropdown.OptionAtIndexIsClickable(index), "Dropdown option is disabled, cannot click");
        dropdown.SelectOptionAtIndex(index);
    }



    public void SelectColonyShipFromDropdown(GameController gameController)
    {
        SelectDropdownOptionAtIndex(ColonyShipDropdownIndex, gameController);
    }

    public void SelectTradeShipFromDropdown(GameController gameController)
    {
        SelectDropdownOptionAtIndex(TradeBaseDropDownIndex, gameController);
    }

    public void SelectSpacePortFromDropdown(GameController gameController)
    {
        SelectDropdownOptionAtIndex(SpacePortDropDownIndex, gameController);
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
