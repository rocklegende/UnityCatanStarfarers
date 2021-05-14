using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

//public class EncounterCardTraverser
//{
//    List<DecisionTreeNode> previousNodes = new List<DecisionTreeNode>();
//    DecisionTreeNode currentNode;
//    EncounterCard card;
//    public EncounterCardTraverser(EncounterCard card)
//    {
//        this.card = card;
//        this.currentNode = card.decisionTree.root;
//    }

//    public void Start()
//    {
//        //open decision dialog
//        ExecuteNode(currentNode);
//    }

//    public void InputReceived(object obj)
//    {


//        if (currentNode.HasNext())
//        {

//            var nextNode = currentNode.GetNext().Find(node => node.GetsTriggeredByValue(value.value));
//            StepToNextNode(nextNode);
//            ExecuteNode(nextNode);
//            //foreach (var node in currentNode.GetNext())
//            //{
//            //    if (node.GetsTriggeredByValue(value.value))
//            //    {
//            //        currentDecisionNode = node;
//            //        OpenDialogForDecisionNode(currentDecisionNode);
//            //        break;
//            //    }
//            //}
//        }
//    }

//    void StepToNextNode(DecisionTreeNode node)
//    {
//        previousNodes.Add(currentNode);
//        currentNode = node;
//    }

//    void ExecuteNode(DecisionTreeNode node)
//    {
//        node.callback = InputReceived;
//        node.Execute();
//    }
//}

public class MockGameController : IGameController
{
    List<Player> players;
    public MockGameController()
    {
        this.players = new List<Player>
        {
            new Player(new SFColor(Color.black)),
            new Player(new SFColor(Color.green)),
            new Player(new SFColor(Color.red))
        };
    }

    public int GetCurrentPlayerAtTurn()
    {
        return 0;
    }

    public HUDScript GetHUDScript()
    {
        return null;
    }

    public Player GetMainPlayer()
    {
        return players[0];
    }

    public MapScript GetMapScript()
    {
        return null;
    }

    public List<Player> GetPlayers()
    {
        return players;
    }
}

namespace Tests
{
    public class EncounterCardTests
    {   

        [Test]
        public void GetsTriggeredByValueTest()
        {
            var node = new DecisionTreeNode(null, new List<int>() { 0, 1, 2 }, null);
            Assert.True(node.GetsTriggeredByValue(0));
            Assert.True(node.GetsTriggeredByValue(1));
            Assert.True(node.GetsTriggeredByValue(2));
            Assert.False(node.GetsTriggeredByValue(3));
        }

        [Test]
        public void GetsTriggeredByValueTest2()
        {
            var node = new DecisionTreeNode(null, false, null);
            Assert.True(node.GetsTriggeredByValue(false));
        }

        [Test]
        public void FameMedalGainStrategyTest()
        {
            var player = new TestHelper().CreateGenericPlayer();
            player.AddFameMedals(2);
            Assert.AreEqual(2, player.FameMedalPieces);
            FameMedalGainStrategy.HandleFameMedalGain(-2, player);
            Assert.AreEqual(0, player.FameMedalPieces);
            FameMedalGainStrategy.HandleFameMedalGain(-2, player); //cant be below 0
            Assert.AreEqual(0, player.FameMedalPieces);
        }

        [Test]
        public void GiveTwoResourcesHasEnoughResources()
        {
            var player = new TestHelper().CreateGenericPlayer();
            Assert.AreEqual(0, player.hand.Count());
            Assert.False(GiveTwoResourcesAction.HasEnoughResources(player));
            player.AddCard(CardType.CARBON);
            player.AddCard(CardType.CARBON);
            Assert.AreEqual(2, player.hand.Count());
            Assert.True(GiveTwoResourcesAction.HasEnoughResources(player));
        }

        [Test]
        public void RobThemActionHasEnoughResources()
        {
            var player = new TestHelper().CreateGenericPlayer();
            Assert.AreEqual(0, player.hand.Count());
            Assert.False(RobPlayersAction.HasEnoughResources(player));
            player.AddCard(CardType.CARBON);
            Assert.AreEqual(1, player.hand.Count());
            Assert.True(RobPlayersAction.HasEnoughResources(player));
        }

        [Test]
        public void OneForTwoTradeHasEnoughResources()
        {
            var player = new TestHelper().CreateGenericPlayer();
            Assert.AreEqual(0, player.hand.Count());
            Assert.False(OneForTwoTradeAction.HasEnoughResources(player));
            player.AddCard(CardType.CARBON);
            Assert.AreEqual(1, player.hand.Count());
            Assert.True(OneForTwoTradeAction.HasEnoughResources(player));
        }

        [Test]
        public void PlayersDontDiscardUpgradesIfNotAboveThreshhold()
        {
            var limit = 8;
            var player = new TestHelper().CreateGenericPlayer();

            var playersWhoNeedToDiscard = DiscardIfMoreThanLimitUpgradesAction
                .GetPlayersWithMoreThanLimit(new List<Player>() { player }, limit);
            Assert.True(playersWhoNeedToDiscard.Count == 0);

            for(int i = 0; i < 3; i++)
            {
                player.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
            }

            for (int i = 0; i < 3; i++)
            {
                player.BuildUpgradeWithoutCost(new CannonUpgradeToken());
            }

            for (int i = 0; i < 2; i++)
            {
                player.BuildUpgradeWithoutCost(new CannonUpgradeToken());
            }

            playersWhoNeedToDiscard = DiscardIfMoreThanLimitUpgradesAction
                .GetPlayersWithMoreThanLimit(new List<Player>() { player }, limit);
            Assert.True(playersWhoNeedToDiscard.Count == 0, "Player should not discard, he has exactly 8 upgrades and not above");

            player.BuildUpgradeWithoutCost(new CannonUpgradeToken());
            playersWhoNeedToDiscard = DiscardIfMoreThanLimitUpgradesAction
                .GetPlayersWithMoreThanLimit(new List<Player>() { player }, limit);
            Assert.True(playersWhoNeedToDiscard[0] == player, "Player should now be forced to discard, since he has 9 upgrades (9 > 8)");

        }

        [Test]
        public void GiveFameMedalToPlayerWithMostFreightPodsWorksCorrectlyAllSame()
        {
            var players = TestHelper.CreateGenericPlayers3();

            // 0 - 0 - 0
            var player1FameMedalBefore = players[0].FameMedalPieces;
            var player2FameMedalBefore = players[1].FameMedalPieces;
            var player3FameMedalBefore = players[2].FameMedalPieces;

            MostFreightPodsAction.GiveFameMedalToPlayersWithMostFreightPods(players);

            var player1FameMedalAfter = players[0].FameMedalPieces;
            var player2FameMedalAfter = players[1].FameMedalPieces;
            var player3FameMedalAfter = players[2].FameMedalPieces;

            Assert.AreEqual(player1FameMedalBefore + 1, player1FameMedalAfter);
            Assert.AreEqual(player2FameMedalBefore + 1, player2FameMedalAfter);
            Assert.AreEqual(player3FameMedalBefore + 1, player3FameMedalAfter);
        }

        [Test]
        public void GiveFameMedalToPlayerWithMostFreightPodsWorksCorrectlyOneWinner()
        {
            var players = TestHelper.CreateGenericPlayers3();

            //1 - 0 - 0
            players[0].BuildUpgradeWithoutCost(new FreightPodUpgradeToken());

            var player1FameMedalBefore = players[0].FameMedalPieces;
            var player2FameMedalBefore = players[1].FameMedalPieces;
            var player3FameMedalBefore = players[2].FameMedalPieces;

            MostFreightPodsAction.GiveFameMedalToPlayersWithMostFreightPods(players);

            var player1FameMedalAfter = players[0].FameMedalPieces;
            var player2FameMedalAfter = players[1].FameMedalPieces;
            var player3FameMedalAfter = players[2].FameMedalPieces;

            Assert.AreEqual(player1FameMedalBefore + 1, player1FameMedalAfter);
            Assert.AreEqual(player2FameMedalBefore, player2FameMedalAfter);
            Assert.AreEqual(player3FameMedalBefore, player3FameMedalAfter);
        }

        [Test]
        public void GiveFameMedalToPlayerWithMostFreightPodsWorksCorrectlyTwoWinner()
        {
            var players = TestHelper.CreateGenericPlayers3();

            // 1 - 1 - 0
            players[0].BuildUpgradeWithoutCost(new FreightPodUpgradeToken());
            players[1].BuildUpgradeWithoutCost(new FreightPodUpgradeToken());

            var player1FameMedalBefore = players[0].FameMedalPieces;
            var player2FameMedalBefore = players[1].FameMedalPieces;
            var player3FameMedalBefore = players[2].FameMedalPieces;

            MostFreightPodsAction.GiveFameMedalToPlayersWithMostFreightPods(players);

            var player1FameMedalAfter = players[0].FameMedalPieces;
            var player2FameMedalAfter = players[1].FameMedalPieces;
            var player3FameMedalAfter = players[2].FameMedalPieces;

            Assert.AreEqual(player1FameMedalBefore + 1, player1FameMedalAfter);
            Assert.AreEqual(player2FameMedalBefore + 1, player2FameMedalAfter);
            Assert.AreEqual(player3FameMedalBefore, player3FameMedalAfter);
        }

    }
}
