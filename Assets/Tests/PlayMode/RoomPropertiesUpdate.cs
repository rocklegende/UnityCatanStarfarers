using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;
using Photon.Realtime;

namespace Tests
{
    [Category("No Photon")]
    public class RoomPropertiesUpdate
    {
        GameController gameController;
        string MapKey = "map";
        string PlayersKey = "players";
        string PlayerWhoMadeLastChangeKey = "playerIdWhoMadeLastChange";
        // A Test behaves as an ordinary method
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var testHelper = new PlayModeTestHelper();
            yield return testHelper.StartSinglePlayerGame();
            gameController = testHelper.GetGameController();
            yield return null;
        }

        [UnityTest]
        public IEnumerator WeDontUpdateIfWeHaveMadeTheLastUpdate()
        {
            var propertiesThatChanged = new Dictionary<object, object>();
            var mapAsBytes = SFFormatter.Serialize(new DefaultMapGenerator().GenerateRandomMap());
            var playersAsBytes = SFFormatter.Serialize(TestHelper.CreateGenericPlayers3());

            propertiesThatChanged.Add(PlayerWhoMadeLastChangeKey, gameController.mainPlayer.guid);
            propertiesThatChanged.Add(MapKey, mapAsBytes);
            propertiesThatChanged.Add(PlayersKey, playersAsBytes);

            var previosNumUpdatesMap = gameController.numMapUpdated;
            var previosNumUpdatesPlayers = gameController.numPlayersUpdated;
            gameController.OnRoomPropertiesUpdateFromOwnDict(propertiesThatChanged);
            Assert.AreEqual(previosNumUpdatesMap, gameController.numMapUpdated);
            Assert.AreEqual(previosNumUpdatesPlayers, gameController.numPlayersUpdated);
            yield return null;
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator WepdateIfWeHaveNOTMadeTheLastUpdate()
        {
            var testHelper = new PlayModeTestHelper();
            yield return testHelper.StartSinglePlayerGame();
            var gameController = testHelper.GetGameController();

            var propertiesThatChanged = new Dictionary<object, object>();
            var mapAsBytes = SFFormatter.Serialize(new DefaultMapGenerator().GenerateRandomMap());
            var playersAsBytes = SFFormatter.Serialize(TestHelper.CreateGenericPlayers3());

            propertiesThatChanged.Add(PlayerWhoMadeLastChangeKey, "xxxxxxxxxxxxx");
            propertiesThatChanged.Add(MapKey, mapAsBytes);
            propertiesThatChanged.Add(PlayersKey, playersAsBytes);

            var previosNumUpdatesMap = gameController.numMapUpdated;
            var previosNumUpdatesPlayers = gameController.numPlayersUpdated;
            gameController.OnRoomPropertiesUpdateFromOwnDict(propertiesThatChanged);
            Assert.AreEqual(previosNumUpdatesMap + 1, gameController.numMapUpdated);
            Assert.AreEqual(previosNumUpdatesPlayers + 1, gameController.numPlayersUpdated);
        }
    }
}
