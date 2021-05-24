using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [Category("No Photon")]
    public class BuildingStuff
    {
        GameController gameController;
        PlayModeTestHelper testHelper;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            LogAssert.ignoreFailingMessages = true;
            var testHelper = new PlayModeTestHelper();
            this.testHelper = testHelper;
            yield return testHelper.StartSinglePlayerGame();
            gameController = testHelper.GetGameController();
            gameController.SetUpDebugState(new ShipBuildingOneColonyShipAndOneSpacePort(gameController));
        }

        

        [UnityTest]
        public IEnumerator BuildANewColonyShip()
        {
            var hud = gameController.GetHUDScript();
            hud.player.AddHand(Hand.FromResources(5,5,5,5,5));
            //open the build panel
            hud.BuildShipsButton.onClick.Invoke();

            testHelper.SelectColonyShipFromDropdown(gameController);

            //select point
            var mapScript = gameController.GetMapScript();
            var clickingPoint = new SpacePoint(4, 10, 0);
            testHelper.ClickSpacePointButton(clickingPoint, mapScript);

            //assert we have a colonyship at that point now
            var buildedTok = gameController.mapModel.TokenAtPoint(clickingPoint);
            Assert.True(buildedTok != null, "Couldnt find token at position " + clickingPoint.ToString());
            Assert.True(buildedTok is ColonyBaseToken, "Builded tok is not Colony!");
            Assert.True(buildedTok.owner == gameController.mainPlayer, "Builded tok does not belong to main player!");

            yield return null;
        }

        [UnityTest]
        public IEnumerator BuildANewTradeShip()
        {
            var hud = gameController.GetHUDScript();
            hud.player.AddHand(Hand.FromResources(5, 5, 5, 5, 5));
            //open the build panel
            hud.BuildShipsButton.onClick.Invoke();
            testHelper.SelectTradeShipFromDropdown(gameController);

            //select point
            var mapScript = gameController.GetMapScript();
            var clickingPoint = new SpacePoint(4, 10, 0);
            testHelper.ClickSpacePointButton(clickingPoint, mapScript);

            //assert we have a colonyship at that point now
            var buildedTok = gameController.mapModel.TokenAtPoint(clickingPoint);
            Assert.True(buildedTok != null, "Couldnt find token at position " + clickingPoint.ToString());
            Assert.True(buildedTok is TradeBaseToken, "Builded tok is not Tradeship!");
            Assert.True(buildedTok.owner == gameController.mainPlayer, "Builded tok does not belong to main player!");

            yield return null;
        }
    }
}
