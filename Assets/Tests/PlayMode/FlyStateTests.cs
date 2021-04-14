using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using com.onebuckgames.UnityStarFarers;
using Photon.Pun;

namespace Tests
{
    public class FlyStateTests
    {
        GameController gameController;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return LoadDefaultScene();
            yield return SetupDebugState();
            gameController = GetGameController();
        }

        public IEnumerator LoadDefaultScene()
        {
            SceneManager.LoadScene(SFScenes.GAME_SCENE);
            yield return null;
        }

        public GameController GetGameController()
        {
            var gameControllerObj = GameObject.Find("GameController");
            var gameControllerScript = gameControllerObj.GetComponent<GameController>();
            return gameControllerScript;
        }

        public IEnumerator SetupDebugState()
        {
            var gameControllerScript = GetGameController();
            gameControllerScript.SetUpDebugState(new TwoTradeShipAndOneSpacePort(gameControllerScript));
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestFlyToCertainSpot()
        {
            var mapScript = gameController.GetMapScript();
            var allTokenObjects = mapScript.GetAllTokenObjects();
            var firstClickable = allTokenObjects
                .Where(gobj => gobj.GetComponent<Space.TokenScript>().tokenModel.CanFly())
                .ToList()
                .First();

            //simulate click on token by calling OnClick directly
            firstClickable.GetComponent<Space.TokenScript>().OnClick(); 
            var tokenModelFirstClickable = firstClickable.GetComponent<Space.TokenScript>().tokenModel;

            //simulate click on spacepointtoken
            var targetSpacePoint = new SpacePoint(11, 5, 0);
            var spacePointToClick = mapScript.GetAllShownSpacePointButtons().Find(point => point.GetComponent<Space.SpacePointButtonScript>().spacePoint.Equals(targetSpacePoint));
            spacePointToClick.GetComponent<Space.SpacePointButtonScript>().OnClick();

            tokenModelFirstClickable.position.Equals(targetSpacePoint);

            yield return null;
        }

    }
}
