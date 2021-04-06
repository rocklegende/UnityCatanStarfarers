using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncounterCardFactory
{

    GameController gameController;

    public EncounterCardFactory(GameController gameController)
    {
        this.gameController = gameController;
    }

    public EncounterCard CreateEncounterCard1()
    {
        var noResourcesGiven = new DecisionTreeNode(null, 0, new LoseOneFameMedalAction(gameController))
        {
            text = "You gave me no resources, thank you, you win one fame medal"
        };
        var foodGift = new Hand();
        foodGift.AddCard(new FoodCard());
        var oneResourceGiven = new DecisionTreeNode(null, 1, new ReceiveResourcesAndReceiveFameMedalAction(gameController, -1, foodGift));
        var twoResourcesGiven = new DecisionTreeNode(null, 2, new ReceiveResourcesAndReceiveFameMedalAction(gameController, 1, null, 1));
        var threeResourcesGiven = new DecisionTreeNode(null, 3, new ReceiveResourcesAndReceiveFameMedalAction(gameController, 1, null, 2));

        var giveResourcesAction = new GiveupResourcesEncounterAction(gameController, 3);

        var giveResourceCard = new DecisionTreeNode(new List<DecisionTreeNode> {
            noResourcesGiven,
            oneResourceGiven,
            twoResourcesGiven,
            threeResourcesGiven }, null, giveResourcesAction)
        {
            text = "A starfarers want up to three resources, how much do you give (up to three)?"
        };

        return EncounterCard.FromRootNode(giveResourceCard);
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
            gameController,
            new LoseOneFameMedalAction(gameController),
            new ReceiveResourcesAndReceiveFameMedalAction(gameController, 0, null, 1),
            new ReceiveResourcesAndReceiveFameMedalAction(gameController, 1, null, 1),
            new GetOneUpgradeForFree(gameController, 1)
        );

        return EncounterCard.FromRootNode(giveResourceNode);
    }

    public EncounterCard CreateEncounterCard3()
    {
        var oneGoodsGift = new Hand();
        oneGoodsGift.AddCard(new GoodsCard());
        var noCounterGiftAction = new NoCounterGift(gameController);

        var giveResourceNode = DecisionTreeNodeCreator.Create0to3ResourceNode(
            gameController,
            new LoseOneFameMedalAction(gameController),
            noCounterGiftAction,
            new WinOneFameMedalAction(gameController),
            new ReceiveResourcesAndReceiveFameMedalAction(gameController, 1, oneGoodsGift) 
        );

        return EncounterCard.FromRootNode(giveResourceNode);
    }

    public EncounterCard CreateEncounterCard4()
    {
        var oneGoodsGift = new Hand();
        oneGoodsGift.AddCard(new GoodsCard());

        var returnGiftAction = new ReturnGift(gameController);
        var giveResourceNode = DecisionTreeNodeCreator.Create0to3ResourceNode(
            gameController,
            new ReceiveResourcesAndReceiveFameMedalAction(gameController, 0, oneGoodsGift),
            new ReceiveResourcesAndReceiveFameMedalAction(gameController, 1, null, 1),
            new ReceiveResourcesAndReceiveFameMedalAction(gameController, 1, null, 2),
            returnGiftAction
        );

        returnGiftAction.giveupResourcesAction = (GiveupResourcesEncounterAction)giveResourceNode.action;

        return EncounterCard.FromRootNode(giveResourceNode);
    }

    public EncounterCard CreateEncounterCard5()
    {
        var giveResourceNode = DecisionTreeNodeCreator.Create0to3ResourceNode(
            gameController,
            new ShipCannotFlyAction(gameController, 1),
            new LoseOneFameMedalAction(gameController),
            new ReceiveResourcesAndReceiveFameMedalAction(gameController, 1, null, -1),
            new GetTradeShipForFree(gameController)
        );

        return EncounterCard.FromRootNode(giveResourceNode);
    }

    public EncounterCard CreateEncounterCard6()
    {
        var giveResourceNode = DecisionTreeNodeCreator.Create0to3ResourceNode(
            gameController,
            new ShipCannotFlyAction(gameController, 1),
            new LoseOneFameMedalAction(gameController),
            new ReceiveResourcesAndReceiveFameMedalAction(gameController, 1, null, -1),
            new GetTradeShipForFree(gameController)
        );

        return EncounterCard.FromRootNode(giveResourceNode);
    }

}