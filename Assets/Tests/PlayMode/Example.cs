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
    [Category("No Photon")]
    public class Example
    {

        GameController gameController;
        PlayModeTestHelper testHelper;
        EncounterCardFactory encounterCardFactory;
        EncounterCardHandler encounterCardHandler;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            var testHelper = new PlayModeTestHelper();
            this.testHelper = testHelper;
            
            yield return testHelper.StartSinglePlayerGame();
            gameController = testHelper.GetGameController();
            gameController.SetUpDebugState(new TwoTradeShipAndOneSpacePort(gameController));
            encounterCardHandler = gameController.encounterCardHandler;
            encounterCardFactory = new EncounterCardFactory(gameController);
            gameController.mainPlayer.AddHand(Hand.FromResources(5, 5, 5, 5, 5));
        }

        DecisionDialog GetDecisionDialog(GameController gameController)
        {
            return gameController.GetHUDScript().decisionDialog.GetComponent<DecisionDialog>();
        }

        [UnityTest]
        public IEnumerator HudUpdatesImmediatelyOnPlayerDataChange()
        {
            var previousVPText = gameController.GetHUDScript().vpText.text;
            gameController.mainPlayer.AddFameMedals(2);
            Assert.AreNotEqual(previousVPText, gameController.GetHUDScript().vpText.text);
            yield return null;
        }


        void PressButtonInsideDecisionDialog(string buttonText, DecisionDialog decisionDialog)
        {
            var button = decisionDialog.GetButtonWithText(buttonText);
            if (button == null)
            {
                throw new System.ArgumentException(string.Format("Did not find button with that text: {0}", buttonText));
            }
            button.onClick.Invoke();
        }


        public EncounterCard GetEncounterCard(int num, EncounterCardFactory factory)
        {
            if (num == 1)
            {
                return factory.CreateEncounterCard1();
            }
            if (num == 2)
            {
                return factory.CreateEncounterCard2();
            }
            if (num == 3)
            {
                return factory.CreateEncounterCard3();
            }
            if (num == 4)
            {
                return factory.CreateEncounterCard4();
            }
            if (num == 5)
            {
                return factory.CreateEncounterCard5();
            }
            if (num == 6)
            {
                return factory.CreateEncounterCard6();
            }
            if (num == 7)
            {
                return factory.CreateEncounterCard7();
            }
            if (num == 8)
            {
                return factory.CreateEncounterCard8();
            }
            if (num == 9)
            {
                return factory.CreateEncounterCard9();
            }
            if (num == 10)
            {
                return factory.CreateEncounterCard10();
            }
            if (num == 11)
            {
                return factory.CreateEncounterCard11();
            }

            if (num == 12)
            {
                return factory.CreateEncounterCard12();
            }

            if (num == 13)
            {
                return factory.CreateEncounterCard13();
            }

            if (num == 14)
            {
                return factory.CreateEncounterCard14();
            }

            if (num == 15)
            {
                return factory.CreateEncounterCard15();
            }

            if (num == 16)
            {
                return factory.CreateEncounterCard16();
            }

            if (num == 17)
            {
                return factory.CreateEncounterCard17();
            }

            if (num == 18)
            {
                return factory.CreateEncounterCard18();
            }

            if (num == 19)
            {
                return factory.CreateEncounterCard19();
            }

            if (num == 20)
            {
                return factory.CreateEncounterCard20();
            }

            if (num == 21)
            {
                return factory.CreateEncounterCard21();
            }

            if (num == 22)
            {
                return factory.CreateEncounterCard22();
            }

            return null;
        }

        [UnityTest]
        public IEnumerator PlayersHaveHudScriptAsObserversAfterSetup()
        {
            var hudScript = gameController.GetHUDScript();
            var players = hudScript.players;
            foreach(var player in players)
            {
                Assert.True(player.GetObservers().Contains(hudScript));
            }
            yield return null;
        }

        [UnityTest]
        public IEnumerator TradeOfferPanelDisplaysTradeCorrectly()
        {
            var hudScript = gameController.GetHUDScript();


            var giveHandInTradeOffer = Hand.FromResources(1, 2, 3);
            var receiveHandInTradeOffer = Hand.FromResources(3, 3, 3);
            var tradeOffer = new TradeOffer(giveHandInTradeOffer, receiveHandInTradeOffer, gameController.mainPlayer);
            hudScript.DisplayTradeOffer(tradeOffer, (decision) => { });
            Assert.True(hudScript.tradeOfferView.activeInHierarchy);
            var tradeOfferViewScript = hudScript.tradeOfferView.GetComponent<TradeOfferView>();
            var displayedGiveHand = tradeOfferViewScript.GiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>().GetDisplayedHand();
            var displayedReceiveHand = tradeOfferViewScript.ReceiveResourceStackRenderer.GetComponent<ResourceCardStackRenderer>().GetDisplayedHand();

            Assert.True(displayedGiveHand.HasSameCardsAs(receiveHandInTradeOffer));
            Assert.True(displayedReceiveHand.HasSameCardsAs(giveHandInTradeOffer));

            yield return null;
        }

        [UnityTest]
        public IEnumerator ResourcePickerAutomaticallyClosesWhenMakingSelection()
        {
            var hudScript = gameController.GetHUDScript();
            hudScript.OpenDiscardResourcePicker((hand) => { Assert.False(hudScript.resourcePicker.activeInHierarchy); });
            Assert.True(hudScript.resourcePicker.activeInHierarchy);
            yield return new WaitForSeconds(4);
            hudScript.resourcePicker.GetComponent<ResourcePicker>().selectButton.onClick.Invoke();
        }

        [UnityTest]
        public IEnumerator PlayerSelectionAutomaticallyClosesWhenMakingSelection()
        {
            var players = TestHelper.CreateGenericPlayers3();
            var hudScript = gameController.GetHUDScript();
            hudScript.OpenPlayerSelection(players, (hand) => { Assert.False(hudScript.multiSelectionView.activeInHierarchy); });
            Assert.True(hudScript.multiSelectionView.activeInHierarchy);
            yield return new WaitForSeconds(4);
            hudScript.multiSelectionView.GetComponent<MultiSelection>().selectButton.onClick.Invoke();
        }

        [UnityTest]
        public IEnumerator TestEncounterCard16_no_no()
        {
            EncounterCard cardToPlay = GetEncounterCard(16, encounterCardFactory);
            var fightNode = cardToPlay.decisionTree.FindNodeWithAction<FightEncounterAction>();
            fightNode.action.PredeterminedResult = false;


            encounterCardHandler.PlayEncounterCard(cardToPlay);
            
            Assert.True(encounterCardHandler.currentDecisionNode.action is YesOrNoEncounterAction);

            PressButtonInsideDecisionDialog("No", GetDecisionDialog(gameController));

            Assert.True(encounterCardHandler.currentDecisionNode.action is RemoveUpgradeAction);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestEncounterCard16_no_yes()
        {
            // Tests if we get a Booster upgrade from taking the no yes path of EncounterCard 16

            EncounterCard cardToPlay = GetEncounterCard(16, encounterCardFactory);
            var fightNode = cardToPlay.decisionTree.FindNodeWithAction<FightEncounterAction>();
            fightNode.action.PredeterminedResult = true;


            encounterCardHandler.PlayEncounterCard(cardToPlay);

            Assert.True(encounterCardHandler.currentDecisionNode.action is YesOrNoEncounterAction);

            PressButtonInsideDecisionDialog("No", GetDecisionDialog(gameController));

            Assert.True(encounterCardHandler.currentDecisionNode.action is GetOneUpgradeForFree);
            var getUpgradeAction = (GetOneUpgradeForFree) encounterCardHandler.currentDecisionNode.action;

            int prevBoosters = gameController.mainPlayer.ship.Boosters;
            //select to upgrade booster
            var boosterIndex = getUpgradeAction.selectableUpgrades.FindIndex(tok => tok is BoosterUpgradeToken);
            var multiSelection = gameController.GetHUDScript().multiSelectionView.GetComponent<MultiSelection>();
            multiSelection.selectCallback(new List<int>() { boosterIndex });

            Assert.AreEqual(prevBoosters + 1, gameController.mainPlayer.ship.Boosters);

            yield return null;
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestEncounterCard12_no()
        {      
            EncounterCard cardToPlay = GetEncounterCard(12, encounterCardFactory);

            encounterCardHandler.PlayEncounterCard(cardToPlay);

            Assert.True(encounterCardHandler.currentDecisionNode.action is YesOrNoEncounterAction);

            var playerBefore = gameController.mainPlayer.SimpleClone();
            PressButtonInsideDecisionDialog("No", GetDecisionDialog(gameController));

            Assert.True(new WonOneFameMedalComparer().comparePlayerStates(playerBefore, gameController.mainPlayer));

            yield return null;
        }

        Hand PlayEncounterCard12_PressYes_MakeTrade()
        {
            EncounterCard cardToPlay = GetEncounterCard(12, encounterCardFactory);

            encounterCardHandler.PlayEncounterCard(cardToPlay);

            PressButtonInsideDecisionDialog("Yes", GetDecisionDialog(gameController));

            // fake the manual input by just calling the callback with a predefined hand
            var inputHand = Hand.FromResources(1);
            var outputHand = Hand.FromResources(0, 2);
            gameController.GetHUDScript().tradePanel.GetComponent<TradePanelScript>().callback(inputHand, outputHand);

            return outputHand;
        }

        void FakeShipDiceThrow(ShipDiceThrow shipDiceThrow)
        {
            gameController.GetHUDScript().shipDiceThrowRenderer.GetComponent<ShipDiceThrowRenderer>().callback(shipDiceThrow);
        }

        [UnityTest]
        public IEnumerator TestEncounterCard12_yes_and_rolled_1()
        {
            PlayEncounterCard12_PressYes_MakeTrade();
            gameController.mainPlayer.AddFameMedal();
            var playerBefore = gameController.mainPlayer.SimpleClone();
            FakeShipDiceThrow(new ShipDiceThrow(1, 0));

            Assert.True(new LostOneFameMedalComparer().comparePlayerStates(playerBefore, gameController.mainPlayer));
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestEncounterCard12_yes_and_rolled_2()
        {
            PlayEncounterCard12_PressYes_MakeTrade();

            gameController.mainPlayer.AddFameMedal();
            var playerBefore = gameController.mainPlayer.SimpleClone();
            FakeShipDiceThrow(new ShipDiceThrow(2, 0));

            Assert.True(new LostOneFameMedalComparer().comparePlayerStates(playerBefore, gameController.mainPlayer));
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestEncounterCard12_yes_and_rolled_3()
        {
            var receivedHand = PlayEncounterCard12_PressYes_MakeTrade();
            var handAfterTrade = gameController.mainPlayer.hand.SimpleClone();
            FakeShipDiceThrow(new ShipDiceThrow(2, 1));

            var handAfterDiceRoll = gameController.mainPlayer.hand;

            handAfterTrade.SubtractHand(receivedHand); //test if the received hand will be removed from the player if a 3 is rolled
            Assert.True(handAfterTrade.HasSameCardsAs(handAfterDiceRoll));

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestEncounterCard12_yes_and_rolled_4()
        {
            PlayEncounterCard12_PressYes_MakeTrade();

            gameController.mainPlayer.AddFameMedal();
            var playerBefore = gameController.mainPlayer.SimpleClone();
            FakeShipDiceThrow(new ShipDiceThrow(2, 2));

            Assert.False(new LostOneFameMedalComparer().comparePlayerStates(playerBefore, gameController.mainPlayer));
            //nothing should happen here with fame medal or something else
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestEncounterCard12_yes_and_rolled_5()
        {
            PlayEncounterCard12_PressYes_MakeTrade();

            gameController.mainPlayer.AddFameMedal();
            var playerBefore = gameController.mainPlayer.SimpleClone();
            FakeShipDiceThrow(new ShipDiceThrow(2, 3));

            Assert.False(new LostOneFameMedalComparer().comparePlayerStates(playerBefore, gameController.mainPlayer));
            //nothing should happen here with fame medal or something else
            yield return null;
        }


    }
}
