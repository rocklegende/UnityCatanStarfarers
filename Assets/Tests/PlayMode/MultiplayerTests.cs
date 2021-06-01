using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using com.onebuckgames.UnityStarFarers;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Tests
{
    public class MultiplayerTests
    {
        /*
         * PLEASE NOTE: these tests only work if another client of this application is running
         */
        PlayModeTestHelper testHelper = new PlayModeTestHelper();

        public IEnumerator LoadLobbyAndConnect()
        {
            SceneManager.LoadScene(0);
            yield return null;
            var launcher = GameObject.Find("Launcher");

            launcher.GetComponent<Launcher>().QuickMatchButtonPressed();
            yield return new WaitForSeconds(15);
        }

        [UnityTest]
        public IEnumerator MatchStartsIfRoomIsFull()
        {
            yield return LoadLobbyAndConnect();
            yield return new WaitForSeconds(15);
            
            var activeScene = SceneManager.GetActiveScene();
            Assert.True(activeScene.name == "SampleScene");
        }

        [UnityTest]
        public IEnumerator SendingResponseToCorrectPlayer()
        {
            yield return LoadLobbyAndConnect();
            var gameController = testHelper.GetGameController();
            var actionRequiredFromPlayer = gameController.GetPlayers().Find(p => p.guid != gameController.GetMainPlayer().guid);

            var action = new GiveResourceRemoteClientAction(1, actionRequiredFromPlayer);
            gameController.RemoteClientRequiresAction(SFFormatter.Serialize(action));

            var hudScript = gameController.GetHUDScript();
            Assert.True(hudScript.resourcePicker.activeInHierarchy);
            yield return new WaitForSeconds(5);
            //force select the current resource picker collection
            hudScript.resourcePicker.GetComponent<ResourcePicker>().OnSelectButtonClick();
            Assert.True(gameController.recentPhotonResponsePlayer.NickName == gameController.SFPlayerToPhotonPlayer(actionRequiredFromPlayer).NickName);

            yield return null;
        }


    }
}
