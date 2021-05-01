using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;

namespace Tests
{
    [Category("No Photon")]
    public class GameStartTests
    {
        GameController gameController;
        PlayModeTestHelper testHelper;

        void DevelopmentSetup()
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

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var testHelper = new PlayModeTestHelper();
            this.testHelper = testHelper;
            yield return testHelper.LoadDefaultScene();
            gameController = testHelper.GetGameController();
        }

        [UnityTest]
        public IEnumerator PlayersHaveGameControllerAsObserversAfterSetup()
        {
            DevelopmentSetup();
            foreach (var player in gameController.players)
            {
                Assert.True(player.GetObservers().Contains(gameController));
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator CorrectMappingOfPlayersToHudElements()
        {
            DevelopmentSetup();
            Assert.True(gameController.GetHUDScript().player == gameController.mainPlayer);
            var smallPlayerViews = gameController.GetHUDScript().GetSmallPlayerViews();
            Assert.AreEqual(1, smallPlayerViews.Count);

            //we have two players, main player is at postion 0 so the other is at position 1, this players should be in the single smallPlayerView
            Assert.True(smallPlayerViews[0].GetComponent<SmallPlayerInfoView>().player == gameController.players[1]); 
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            //keeping PhotonNetwork in offline mode messes with the rest of tests
            PhotonNetwork.OfflineMode = false;
            yield return null;
        }
    }
}
