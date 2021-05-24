using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Photon.Pun;
using Photon.Realtime;
using com.onebuckgames.UnityStarFarers;

namespace Tests
{
    [Category("No Photon")]
    public class RoomPropertiesUpdate
    {
        PlayModeTestHelper testHelper;
        GameController gameController;
        string MapKey = "map";
        string PlayersKey = "players";
        string PlayerWhoMadeLastChangeKey = "playerIdWhoMadeLastChange";
        // A Test behaves as an ordinary method
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            testHelper = new PlayModeTestHelper();

            yield return testHelper.StartSinglePlayerGame();
            gameController = testHelper.GetGameController();
            yield return null;

        }

        private void FakeRoomPropertiesUpdate(
            string PlayerWhoMadeLastChange,
            List<Player> newPlayerData,
            Map newMap
        )
        {
            var propertiesThatChanged = new Dictionary<object, object>();
            var mapAsBytes = SFFormatter.Serialize(newMap);
            var playersAsBytes = SFFormatter.Serialize(newPlayerData);

            propertiesThatChanged.Add(PlayerWhoMadeLastChangeKey, PlayerWhoMadeLastChange);
            propertiesThatChanged.Add(MapKey, mapAsBytes);
            propertiesThatChanged.Add(PlayersKey, playersAsBytes);
            //return propertiesThatChanged;

            gameController.OnRoomPropertiesUpdateFromOwnDict(propertiesThatChanged);
        }

        [UnityTest]
        public IEnumerator WeDontUpdateIfWeHaveMadeTheLastUpdate()
        {
            var previosNumUpdatesMap = gameController.numMapUpdated;
            var previosNumUpdatesPlayers = gameController.numPlayersUpdated;

            FakeRoomPropertiesUpdate(
                gameController.mainPlayer.guid,
                TestHelper.CreateGenericPlayers3(),
                new DefaultMapGenerator().GenerateRandomMap()
            );

            Assert.AreEqual(previosNumUpdatesMap, gameController.numMapUpdated);
            Assert.AreEqual(previosNumUpdatesPlayers, gameController.numPlayersUpdated);
            yield return null;
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator WeUpdateIfWeHaveNOTMadeTheLastUpdate()
        {            
            var previosNumUpdatesMap = gameController.numMapUpdated;
            var previosNumUpdatesPlayers = gameController.numPlayersUpdated;

            FakeRoomPropertiesUpdate(
                "xxxxxxxxxxxxxxx",
                TestHelper.CreateGenericPlayers3(),
                new DefaultMapGenerator().GenerateRandomMap()
            );

            Assert.AreEqual(previosNumUpdatesMap + 1, gameController.numMapUpdated);
            Assert.AreEqual(previosNumUpdatesPlayers + 1, gameController.numPlayersUpdated);
            yield return null;
        }

        [UnityTest]
        public IEnumerator HudDropDownsUpdateCorrectlyAfterRoomPropertiesChanged()
        {
            var newMap = new DefaultMapGenerator().GenerateRandomMap();

            var newPlayerData = new Player(gameController.mainPlayer.color);
            newPlayerData.name = gameController.mainPlayer.name;
            newPlayerData.BuildTokenWithoutCost(
                newMap,
                new ColonyBaseToken().GetType(),
                new SpacePoint(new HexCoordinates(5, 5), 1),
                new SpacePortToken().GetType()
            );
            newPlayerData.AddHand(Hand.FromResources(5, 5, 5, 5, 5));

            var previousMapModel = gameController.mapModel;

            FakeRoomPropertiesUpdate(
                "xxxxxxxxxxxxxxx",
                new List<Player>() { newPlayerData },
                newMap
            );

            yield return new WaitForSeconds(3);

            var dropdown = gameController.GetHUDScript().buildShipsDropDownRef.GetComponent<BuildDropDown>();
            Assert.True(dropdown.OptionAtIndexIsClickable(testHelper.ColonyShipDropdownIndex), "Dropdown option for colonyship is disabled, cannot click");

            Assert.AreSame(previousMapModel, gameController.mapModel);
            yield return null;
        }

    }
}
