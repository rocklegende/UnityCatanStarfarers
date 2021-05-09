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

public class PlayModeTestHelper
{
    public PlayModeTestHelper()
    {
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

    

    public GameController GetGameController()
    {
        var gameControllerObj = GameObject.Find("GameController");
        var gameControllerScript = gameControllerObj.GetComponent<GameController>();
        return gameControllerScript;
    }

    public IEnumerator SetupDebugState(DebugStartState debugState)
    {
        var gameControllerScript = GetGameController();
        gameControllerScript.SetUpDebugState(debugState);
        yield return null;
    }

    public void ClickSpacePointButton(SpacePoint targetPoint, MapScript mapScript)
    {
        var spacePointToClick = mapScript.GetAllShownSpacePointButtons().Find(pointButton => pointButton.GetComponent<Space.SpacePointButtonScript>().spacePoint.Equals(targetPoint));
        spacePointToClick.GetComponent<Space.SpacePointButtonScript>().OnClick();
    }
}
