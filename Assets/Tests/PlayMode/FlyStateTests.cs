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

namespace Tests
{
    [Category("No Photon")]
    public class FlyStateTests
    {
        GameController gameController;
        PlayModeTestHelper testHelper;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var testHelper = new PlayModeTestHelper();
            this.testHelper = testHelper;
            yield return testHelper.StartSinglePlayerGame();
            gameController = testHelper.GetGameController();
            //gameController.SetUpDebugState(new TwoTradeShipAndOneSpacePort(gameController));
            var colony = gameController.mainPlayer.BuildTokenWithoutCost(
                gameController.mapModel,
                new ColonyBaseToken().GetType(),
                new SpacePoint(new HexCoordinates(5, 5), 1),
                new SpacePortToken().GetType()
            );

            gameController.mainPlayer.BuildColonyShipForFree(
                gameController.mapModel,
                new SpacePoint(5, 5, 0)
            );

            gameController.mainPlayer.BuildTradeShipForFree(
                gameController.mapModel,
                new SpacePoint(new HexCoordinates(5, 5).W(), 0)
            );

            var contains = gameController.mapModel.tokensOnMap.Contains(colony);

            gameController.State = new FlyShipsState(gameController);
            yield return new WaitForSeconds(3);
        }

        [UnityTest]
        public IEnumerator TestFlyToFreeSpacePoint()
        {
            var mapScript = gameController.GetMapScript();
            var allTokenObjects = mapScript.GetAllTokenObjects();
            var firstClickable = allTokenObjects
                .Where(gobj => GetTokenScript(gobj).tokenModel.CanFly())
                .ToList()
                .First();


            //be sure we have enough steps to fly to that point
            var tokenModelFirstClickable = GetTokenScript(firstClickable).tokenModel;
            tokenModelFirstClickable.addSteps(100);

            //simulate click on token by calling OnClick directly
            GetTokenScript(firstClickable).OnClick();
            yield return new WaitForSeconds(10);

            //simulate click on spacepointtoken
            var targetSpacePoint = new SpacePoint(11, 5, 0);
            testHelper.ClickSpacePointButton(targetSpacePoint, mapScript);

            Assert.True(tokenModelFirstClickable.position.Equals(targetSpacePoint));

            yield return null;
        }

        Space.TokenScript GetTokenScript(GameObject gobj)
        {
            return gobj.GetComponent<Space.TokenScript>();
        }

        [UnityTest]
        public IEnumerator TestFlyToTradeBaseAndSettle()
        {
            var WaitTimeBetweenSteps = 0; //use this to debug this test

            var mapScript = gameController.GetMapScript();
            var allTokenObjects = mapScript.GetAllTokenObjects();
            var firstClickableTradeShip = allTokenObjects
                .Where(gobj => GetTokenScript(gobj).tokenModel.CanFly() && GetTokenScript(gobj).tokenModel is TradeBaseToken)
                .ToList()
                .First();

            //be sure we have enough steps to fly to that tradestation
            var tokenModelFirstClickable = GetTokenScript(firstClickableTradeShip).tokenModel;
            tokenModelFirstClickable.addSteps(100);

            //simulate click on token by calling OnClick directly
            var prevHighlightedTokens = gameController.GetMapScript().GetHighlightedTokens();
            GetTokenScript(firstClickableTradeShip).OnClick();
            gameController.mainPlayer.BuildUpgradeWithoutCost(new FreightPodUpgradeToken());

            yield return new WaitForSeconds(WaitTimeBetweenSteps);

            //simulate click on spacepoint which is directly at center of a trade station
            var firstTradeStation = gameController.mapModel.GetTradeStations()[0];
            var targetSpacePoint = firstTradeStation.GetCenter();
            var prevNumCards = firstTradeStation.tradingCards.Count;
            testHelper.ClickSpacePointButton(targetSpacePoint, mapScript);

            yield return new WaitForSeconds(WaitTimeBetweenSteps);

            //simulate click on settle button
            var settleButton = gameController.GetHUDScript().settleButton;
            var friendshipCardSelection = gameController.GetHUDScript().friendShipCardSelection;
            Assert.True(tokenModelFirstClickable.position.Equals(targetSpacePoint));
            Assert.True(settleButton.activeInHierarchy, "Settle button is not visible!"); //settle button is visible
            settleButton.GetComponent<Button>().onClick.Invoke();

            yield return new WaitForSeconds(WaitTimeBetweenSteps);

            //select first card of friendshipcards
            Assert.True(friendshipCardSelection.activeInHierarchy, "Friendshipcard selection is not visible!");
            var friendshipCardSelectionScript = friendshipCardSelection.GetComponent<FriendShipCardSelector>();
            friendshipCardSelectionScript.selectButton.onClick.Invoke();
            yield return new WaitForSeconds(WaitTimeBetweenSteps);

            Assert.AreEqual(prevNumCards - 1, firstTradeStation.tradingCards.Count, "Picking trading card didnt remove the card from the tradeStation cards");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestFlyToResourceGroupAndSettle()
        {
            var WaitTimeBetweenSteps = 5; //use this to debug this test

            var mapScript = gameController.GetMapScript();
            var hudScript = gameController.GetHUDScript();
            var previousVp = gameController.mainPlayer.GetVictoryPoints();

            var allTokenObjects = mapScript.GetAllTokenObjects();
            var firstClickableColonyShip = allTokenObjects
                .Where(gobj => GetTokenScript(gobj).tokenModel.CanFly() && GetTokenScript(gobj).tokenModel is ColonyBaseToken)
                .ToList()
                .First();

            //be sure we have enough steps to fly to that tradestation
            var tokenModelFirstClickable = GetTokenScript(firstClickableColonyShip).tokenModel;
            tokenModelFirstClickable.addSteps(100); 

            //simulate click on token by calling OnClick directly
            GetTokenScript(firstClickableColonyShip).OnClick();
            TestHelper.SetUpgradesForPlayer(gameController.mainPlayer, 5);

            yield return new WaitForSeconds(WaitTimeBetweenSteps);

            //simulate click on spacepoint which is directly at center of a trade station
            var firstResourceGroup = (ResourceTileGroup)gameController.mapModel.tileGroups.Find(group => group is ResourceTileGroup);
            var targetSpacePoint = firstResourceGroup.GetSettlePoints()[0];
            testHelper.ClickSpacePointButton(targetSpacePoint, mapScript);

            yield return new WaitForSeconds(WaitTimeBetweenSteps);

            //simulate click on settle button
            var settleButton = gameController.GetHUDScript().settleButton;
            Assert.True(settleButton.activeInHierarchy, "Settle button is not visible!");
            settleButton.GetComponent<Button>().onClick.Invoke();

            yield return new WaitForSeconds(WaitTimeBetweenSteps);

            var tokenStaysOnSameSpotAfterSettling = tokenModelFirstClickable.position.Equals(targetSpacePoint);
            Assert.True(tokenStaysOnSameSpotAfterSettling, "Token changed position after settling on resource tile group. It should stay the same");
            Assert.True(firstResourceGroup.IsRevealed());
            Assert.True(firstResourceGroup.GetPirateTokens().Count == 0, "Error: after settling resource group has still at least 1 pirate token left");
            Assert.AreNotEqual(previousVp.ToString(), hudScript.vpText.text, "Hud doesnt seem to update correcty. VPs presented are still the same value.");

            yield return null;
        }

    }
}
