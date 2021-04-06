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

namespace Tests
{
    public class EncounterCardTests
    {
        //[Test]
        //public void EncounterCard1Test()
        //{
        //    var gameController = new GameController();
        //    var factory = new EncounterCardFactory(gameController);
        //    var encounterCard = factory.CreateEncounterCard1();

        //    var traverser = new EncounterCardTraverser(encounterCard);
        //    Assert.True(traverser.currentnode == GiveResourceNode());
        //    traverser.InputReceived(3);
        //    Assert.True(traverser.currentNode == GetOneFameMedal());
        //    traverser.InputReceived(true);
        //    Assert.True(traverser.currentNode == null);

        //}

        [Test]
        public void ShipCannotFlyAction()
        {
            //var gameController = new GameController();
            var action = new ShipCannotFlyAction(null);

            var token = new ColonyBaseToken();
            action.DidSelectToken(token);

            Assert.True(token.IsDisabled());
        }

    }
}
