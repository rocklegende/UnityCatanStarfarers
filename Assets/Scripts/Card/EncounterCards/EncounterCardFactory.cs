using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncounterCardFactory
{

    HUDScript hudScript;

    public EncounterCardFactory(HUDScript hudScript)
    {
        this.hudScript = hudScript;
    }

    public DecisionTreeNode GetEndNode()
    {
        return new DecisionTreeNode(null, true, new EncounterFinishedAction(hudScript));
    }

    public DecisionTreeNode[] GetEndNodes()
    {
        return new DecisionTreeNode[] { GetEndNode() };
    }

    public EncounterCard CreateEncounterCard1()
    {
        var noResourcesGiven = new DecisionTreeNode(GetEndNodes(), 0, new LoseOneFameMedalAction(hudScript))
        {
            text = "You gave me no resources, thank you, you win one fame medal"
        };
        var foodGift = new Hand();
        foodGift.AddCard(new FoodCard());
        var oneResourceGiven = new DecisionTreeNode(null, 1, new ReceiveResourcesAndReceiveFameMedalAction(hudScript, -1, foodGift));
        var twoResourcesGiven = new DecisionTreeNode(null, 2, new ReceiveResourcesAndReceiveFameMedalAction(hudScript, 1, null, 1));
        var threeResourcesGiven = new DecisionTreeNode(null, 3, new ReceiveResourcesAndReceiveFameMedalAction(hudScript, 1, null, 2));

        var giveResourcesAction = new GiveupResourcesEncounterAction(hudScript, 3);

        var giveResourceCard = new DecisionTreeNode(new DecisionTreeNode[] {
            noResourcesGiven,
            oneResourceGiven,
            twoResourcesGiven,
            threeResourcesGiven }, null, giveResourcesAction)
        {
            text = "A starfarers want up to three resources, how much do you give (up to three)?"
        };

        var decisionTree = new DecisionTree(giveResourceCard);

        var encounter = new EncounterCard(decisionTree);

        return encounter;
    }

    public EncounterCard CreateEncounterCard2()
    {
        //var noResourcesGiven = new DecisionTreeNode(null, 0, new LoseOneFameMedalAction(hudScript))
        //{
        //    text = "You lose one FameMedal"
        //};

        //var oneResourceGiven = new DecisionTreeNode(null, 1, new ReceiveResourcesAndReceiveFameMedalAction(hudScript, 0, null, 1))
        //{
        //    text = "You lose get one resource for free"
        //};

        //var twoResourcesGiven = new DecisionTreeNode(null, 2, new ReceiveResourcesAndReceiveFameMedalAction(hudScript, 1, null, 1))
        //{
        //    text = "You get a free fame medal, and one resource of your choice"
        //};

        //var threeResourcesGiven = new DecisionTreeNode(null, 3, new GetOneUpgradeForFree(hudScript, 1))
        //{
        //    text = "You get a free upgrade, you lucky bastard"
        //};

        //var giveResourceCard = new DecisionTreeNode(new DecisionTreeNode[] {
        //    noResourcesGiven,
        //    oneResourceGiven,
        //    twoResourcesGiven,
        //    threeResourcesGiven }, null, new GiveupResourcesEncounterAction(hudScript, 3))
        //{
        //    text = "A starfarers want up to three resources, how much do you give (up to three)?"
        //};


        var giveResourceNode = DecisionTreeNodeCreator.Create0to3ResourceNode(
            hudScript,
            new LoseOneFameMedalAction(hudScript),
            new ReceiveResourcesAndReceiveFameMedalAction(hudScript, 0, null, 1),
            new ReceiveResourcesAndReceiveFameMedalAction(hudScript, 1, null, 1),
            new GetOneUpgradeForFree(hudScript, 1)
        );

        var decisionTree = new DecisionTree(giveResourceNode);

        var encounter = new EncounterCard(decisionTree);

        return encounter;
    }

}