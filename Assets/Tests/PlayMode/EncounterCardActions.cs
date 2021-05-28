﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using com.onebuckgames.UnityStarFarers;
using Photon.Pun;
using Photon.Realtime;

namespace Tests
{
    [Category("No Photon")]
    public class EncounterCardActions
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
            gameController.SetUpDebugState(new TwoTradeShipAndOneSpacePort(gameController));
            
        }

        [UnityTest]
        public IEnumerator Receiving_GIVE_RESOURCES_FromRemoteClientOpensResourcePicker()
        {
            var action = new RemoteClientAction(RemoteClientActionType.GIVE_RESOURCE, new object[] { 1 }, gameController.mainPlayer);
            gameController.RemoteClientRequiresAction(SFFormatter.Serialize(action));

            var hudScript = gameController.GetHUDScript();
            Assert.True(hudScript.resourcePicker.activeInHierarchy);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Receiving_SEVEN_ROLL_FromRemoteClientOpensDiscardResourcePicker()
        {
            gameController.mainPlayer.AddHand(Hand.FromResources(4, 4, 4, 4));

            var action = new RemoteClientAction(RemoteClientActionType.SEVEN_ROLL_DISCARD, null, gameController.mainPlayer);
            gameController.RemoteClientRequiresAction(SFFormatter.Serialize(action));

            var hudScript = gameController.GetHUDScript();
            Assert.True(hudScript.resourcePicker.activeInHierarchy);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Receiving_GIVEUP_UPGRADE_FromRemoteClientOpensUpgradeSelection()
        {
            gameController.mainPlayer.BuildUpgradeWithoutCost(new FreightPodUpgradeToken());

            var action = new RemoteClientAction(RemoteClientActionType.GIVEUP_UPGRADE, new object[] { 1 }, gameController.mainPlayer);
            gameController.RemoteClientRequiresAction(SFFormatter.Serialize(action));

            var hudScript = gameController.GetHUDScript();
            Assert.True(hudScript.multiSelectionView.activeInHierarchy);

            yield return null;
        }

        [UnityTest]
        public IEnumerator Receiving_TRADE_OFFER_FromRemoteClientOpensUpgradeSelection()
        {
            var dispatcher = new DefaultRemoteActionDispatcher(gameController);
            var targets = new List<Player>() { gameController.mainPlayer };
            var action = new RemoteClientAction(
                RemoteClientActionType.TRADE_OFFER,
                new object[] { new TradeOffer(new Hand(), new Hand(), new TestHelper().CreateGenericPlayer())
            }, gameController.mainPlayer);
            dispatcher.SetTargets(targets);
            dispatcher.SetAction(action);
            dispatcher.MakeRequest((singleResponse) => { }, (allResponses) => { });

            var hudScript = gameController.GetHUDScript();
            Assert.True(hudScript.tradeOfferView.activeInHierarchy);

            yield return null;
        }
    }
}
