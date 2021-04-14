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
            Assert.AreEqual(2, player.GetFameMedalPieces());
            FameMedalGainStrategy.HandleFameMedalGain(-2, player);
            Assert.AreEqual(0, player.GetFameMedalPieces());
            FameMedalGainStrategy.HandleFameMedalGain(-2, player); //cant be below 0
            Assert.AreEqual(0, player.GetFameMedalPieces());
        }
    }
}
