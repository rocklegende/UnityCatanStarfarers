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

    public EncounterCard CreateEncounterCard7()
    {

        var disabledShip = new DecisionTreeNode(null, false, new ShipCannotFlyAction(gameController));

        var returnGiftAction = new ReturnGift(gameController);
        var giftReturn = new DecisionTreeNode(null, true, returnGiftAction);

        var pirateLeaves = new DecisionTreeNode(null, false, new PirateLeaves(gameController));

        var fightWithRightNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { giftReturn, disabledShip },
            true,
            new FightEncounterAction(gameController, FightCategory.CANNON, FightEncounterOpponent.FIRST_RIGHT));


        //TODO: would be better to have a stronger connection between the possible outcomes of the action and the paths.
        //The action decides which paths are possible
        //If we have a yes or no action (BinaryAction) then only two paths are possible
        //Maybe we should have a BinaryDecisionTreeNode(yesNode, noNode) and a MultipleDecisionTreeNode or something like that.
        var yesOrNo = new DecisionTreeNode(new List<DecisionTreeNode>() { pirateLeaves, fightWithRightNeighor }, new List<int>() { 0, 1, 2, 3 }, new YesOrNoEncounterAction(gameController));

        var giveResourceAction = new GiveupResourcesEncounterAction(gameController, 3);
        var giveResourceNode = new DecisionTreeNode(new List<DecisionTreeNode>() { yesOrNo }, null, giveResourceAction);

        returnGiftAction.giveupResourcesAction = giveResourceAction; //assign the action to that action so we know what we gave away

        return EncounterCard.FromRootNode(giveResourceNode);
    }

    public EncounterCard CreateEncounterCard8()
    {
        //TODO: remove duplication, largely the same as EncounterCard7, difference is the opponent (LEFT and RIGHT)
        var disabledShip = DecisionTreeNodeCreator.CreateShipCannotFlyNode(gameController);

        var returnGiftAction = new ReturnGift(gameController);
        var giftReturn = new DecisionTreeNode(null, true, returnGiftAction);

        var pirateLeaves = DecisionTreeNodeCreator.CreateNoActionNode(gameController);

        var fightWithRightNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { giftReturn, disabledShip },
            true,
            new FightEncounterAction(gameController, FightCategory.CANNON, FightEncounterOpponent.FIRST_LEFT));

        var yesOrNo = new DecisionTreeNode(new List<DecisionTreeNode>() { pirateLeaves, fightWithRightNeighor }, new List<int>() { 0, 1, 2, 3 }, new YesOrNoEncounterAction(gameController));

        var giveResourceAction = new GiveupResourcesEncounterAction(gameController, 3);
        var giveResourceNode = new DecisionTreeNode(new List<DecisionTreeNode>() { yesOrNo }, null, giveResourceAction);
        returnGiftAction.giveupResourcesAction = (GiveupResourcesEncounterAction)giveResourceNode.action; //assign the action to that action so we know what we gave away

        return EncounterCard.FromRootNode(giveResourceNode);
    }

    public EncounterCard CreateEncounterCard9()
    {
        var tradeShipForFree = DecisionTreeNodeCreator.CreateGetTradeShipForFree(gameController);
        var removeUpgrade = DecisionTreeNodeCreator.CreateRemoveUpgradeNode(gameController, 1);

        var giveResourceNode = new DecisionTreeNode(null, true, new GiveupResourcesEncounterAction(gameController, 2, 2));
        var fightWithNeighor = new DecisionTreeNode(
             new List<DecisionTreeNode>() { tradeShipForFree, removeUpgrade },
             false,
             new FightEncounterAction(gameController, FightCategory.CANNON, FightEncounterOpponent.FIRST_RIGHT));

        //TODO: the result of this should automatically be false if the player doesnt even have 2 resources
        var give2ResourcesYesOrNo = new DecisionTreeNode(new List<DecisionTreeNode>() { giveResourceNode, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController));

        return EncounterCard.FromRootNode(give2ResourcesYesOrNo);
    }

    public EncounterCard CreateEncounterCard10()
    {
        var giftedHand = new Hand();
        giftedHand.AddCard(new CarbonCard());
        giftedHand.AddCard(new CarbonCard());

        //TODO: design should be like this:
        //DecisionTreeNode has specific text that will be displayed in the decision dialog.
        //DecisionTreeNode has multiple actions that can be fullfilled (for example LoseOneFameMedal, ShipCannotFly)

        var beute = DecisionTreeNodeCreator.CreateReceiveResources(gameController, 1, giftedHand);
        var removeUpgrade = DecisionTreeNodeCreator.CreateShipCannotFlyNode(gameController, 1);

        var giveResourceNode = new DecisionTreeNode(null, true, new GiveupResourcesEncounterAction(gameController, 2, 2));
        var fightWithNeighor = new DecisionTreeNode(
             new List<DecisionTreeNode>() { beute, removeUpgrade },
             false,
             new FightEncounterAction(gameController, FightCategory.CANNON, FightEncounterOpponent.SECOND_RIGHT));

        //TODO: the result of this should automatically be false if the player doesnt even have 2 resources
        var give2ResourcesYesOrNo = new DecisionTreeNode(new List<DecisionTreeNode>() { giveResourceNode, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController));

        return EncounterCard.FromRootNode(give2ResourcesYesOrNo);
    }

    public EncounterCard CreateEncounterCard11()
    {
        var giftedHand = new Hand();
        giftedHand.AddCard(new OreCard());
        giftedHand.AddCard(new OreCard());
        var beute = DecisionTreeNodeCreator.CreateReceiveResources(gameController, 1, giftedHand);
        var removeUpgrade = DecisionTreeNodeCreator.CreateRemoveUpgradeNode(gameController);

        var disableNode = new DecisionTreeNode(null, 2, new ShipCannotFlyAction(gameController));

        var giveResourceNode = new DecisionTreeNode(new List<DecisionTreeNode>() { disableNode }, true, new GiveupResourcesEncounterAction(gameController, 2, 2));
        var fightWithNeighor = new DecisionTreeNode(
             new List<DecisionTreeNode>() { beute, removeUpgrade },
             false,
             new FightEncounterAction(gameController, FightCategory.CANNON, FightEncounterOpponent.FIRST_LEFT));

        //TODO: the result of this should automatically be false if the player doesnt even have 2 resources
        var give2ResourcesYesOrNo = new DecisionTreeNode(new List<DecisionTreeNode>() { giveResourceNode, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController));

        return EncounterCard.FromRootNode(give2ResourcesYesOrNo);
    }

    public EncounterCard CreateEncounterCard12() {

        var loseFameMedal = new DecisionTreeNode(null, new List<int>() { 1, 2 }, new LoseOneFameMedalAction(gameController));
        var loseHandAction = new LoseHandAction(gameController);
        var loseTradedResources = new DecisionTreeNode(null, new List<int>() { 3 }, loseHandAction);
        var noAction = new DecisionTreeNode(null, new List<int>() { 4, 5 }, new NoCounterGift(gameController));

        var shakeShip = new DecisionTreeNode(new List<DecisionTreeNode>() { loseFameMedal, loseTradedResources, noAction }, true, new ShakeShipAction(gameController));

        var makeTradeAction = new MakeTradeAction(gameController, 1, 2);
        var makeTrade = new DecisionTreeNode(new List<DecisionTreeNode>() { shakeShip }, true, makeTradeAction);

        var winFameMedal = new DecisionTreeNode(null, false, new WinOneFameMedalAction(gameController));

        var oneForTwoTrade = new DecisionTreeNode(
            new List<DecisionTreeNode>() { winFameMedal, makeTrade }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.Encounter1For2Trade()
        };

        loseHandAction.makeTradeAction = makeTradeAction;

        return EncounterCard.FromRootNode(oneForTwoTrade);
    }

    public EncounterCard CreateEncounterCard13()
    {
        var loseFameMedal = new DecisionTreeNode(null, new List<int>() { 1, 2 }, new NoCounterGift(gameController));
        var loseHandAction = new LoseHandAction(gameController);
        var loseTradedResources = new DecisionTreeNode(null, new List<int>() { 3 }, loseHandAction);
        var noAction = new DecisionTreeNode(null, new List<int>() { 4, 5 }, new LoseOneFameMedalAction(gameController));

        var shakeShip = new DecisionTreeNode(new List<DecisionTreeNode>() { loseFameMedal, loseTradedResources, noAction }, true, new ShakeShipAction(gameController));

        var makeTradeAction = new MakeTradeAction(gameController, 1, 2);
        var makeTrade = new DecisionTreeNode(new List<DecisionTreeNode>() { shakeShip }, true, makeTradeAction);

        var noMedal = new DecisionTreeNode(null, false, new NoCounterGift(gameController));

        var oneForTwoTrade = new DecisionTreeNode(
            new List<DecisionTreeNode>() { noMedal, makeTrade }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.Encounter1For2Trade()
        };

        loseHandAction.makeTradeAction = makeTradeAction;

        return EncounterCard.FromRootNode(oneForTwoTrade);

    }
    public EncounterCard CreateEncounterCard14()
    {
        var loseFameMedal = new DecisionTreeNode(null, new List<int>() { 1, 2 }, new LoseOneFameMedalAction(gameController));
        var robButLoseFameMedal = new DecisionTreeNode(null, new List<int>() { 3, 4 }, new GetResourceFromEveryPlayer(gameController)); //TODO action not implemented yet
        var rob = new DecisionTreeNode(null, new List<int>() { 5 }, new GetResourceFromEveryPlayer(gameController)); //TODO: action not implemented yet

        var shakeShip = new DecisionTreeNode(new List<DecisionTreeNode>() { loseFameMedal, robButLoseFameMedal, rob }, true, new ShakeShipAction(gameController));

        var giveUpResources = new DecisionTreeNode(new List<DecisionTreeNode>() { shakeShip }, true, new GiveupResourcesEncounterAction(gameController, 1, 1));

        var winMedal = new DecisionTreeNode(null, false, new WinOneFameMedalAction(gameController));

        var robThem = new DecisionTreeNode(
            new List<DecisionTreeNode>() { winMedal, giveUpResources }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterRobPlayersFor1Card()
        };

        return EncounterCard.FromRootNode(robThem);
    }
    public EncounterCard CreateEncounterCard15()
    {
        var loseFameMedal = new DecisionTreeNode(null, new List<int>() { 4, 5 }, new LoseOneFameMedalAction(gameController));
        var robButLoseFameMedal = new DecisionTreeNode(null, new List<int>() { 2, 3 }, new GetResourceFromEveryPlayer(gameController)); //TODO action not implemented yet
        var rob = new DecisionTreeNode(null, new List<int>() { 1 }, new GetResourceFromEveryPlayer(gameController)); //TODO: action not implemented yet

        var shakeShip = new DecisionTreeNode(new List<DecisionTreeNode>() { loseFameMedal, robButLoseFameMedal, rob }, true, new ShakeShipAction(gameController));

        var giveUpResources = new DecisionTreeNode(new List<DecisionTreeNode>() { shakeShip }, true, new GiveupResourcesEncounterAction(gameController, 1, 1));

        var winMedal = new DecisionTreeNode(null, false, new WinOneFameMedalAction(gameController));

        var robThem = new DecisionTreeNode(
            new List<DecisionTreeNode>() { winMedal, giveUpResources }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterRobPlayersFor1Card()
        };

        return EncounterCard.FromRootNode(robThem);

    }


    public EncounterCard CreateEncounterCard16()
    {
        var removeUpgrade = DecisionTreeNodeCreator.CreateRemoveUpgradeNode(gameController);
        var receiveUpgrade = DecisionTreeNodeCreator.CreateGetUpgradeForFree(gameController);

        var fightWithNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { receiveUpgrade, removeUpgrade },
            false,
            new FightEncounterAction(gameController, FightCategory.CANNON, FightEncounterOpponent.FIRST_RIGHT)
        );
        var hasMoreBoosters = new DecisionTreeNode(
            new List<DecisionTreeNode>() { fightWithNeighor, DecisionTreeNodeCreator.CreateNoActionNode(gameController) },
            true,
            new HasMoreBoostersThanNeighborAction(gameController, FightEncounterOpponent.FIRST_RIGHT)
        );


        var youWantToFlee = new DecisionTreeNode(
            new List<DecisionTreeNode>() { hasMoreBoosters, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterYouWantToFlee()
        };

        return EncounterCard.FromRootNode(youWantToFlee);
    }

    public EncounterCard CreateEncounterCard17()
    {
        var removeUpgrade = DecisionTreeNodeCreator.CreateRemoveUpgradeNode(gameController, 1);
        var giftedHand = new Hand();
        giftedHand.AddCard(new OreCard());
        giftedHand.AddCard(new OreCard());
        var beute = DecisionTreeNodeCreator.CreateReceiveResources(gameController, 1, giftedHand);

        var fleeingSucceeded = DecisionTreeNodeCreator.CreateNoActionNode(gameController);

        var fightWithNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { removeUpgrade, beute },
            false,
            new FightEncounterAction(gameController, FightCategory.CANNON, FightEncounterOpponent.SECOND_LEFT)
        );
        var hasMoreBoosters = new DecisionTreeNode(
            new List<DecisionTreeNode>() { fightWithNeighor, fleeingSucceeded },
            true,
            new HasMoreBoostersThanNeighborAction(gameController, FightEncounterOpponent.SECOND_LEFT)
        );

        var youWantToFlee = new DecisionTreeNode(
            new List<DecisionTreeNode>() { hasMoreBoosters, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterYouWantToFlee()
        };

        return EncounterCard.FromRootNode(youWantToFlee);
    }

    public EncounterCard CreateEncounterCard18()
    {
        var cannotFly = DecisionTreeNodeCreator.CreateShipCannotFlyNode(gameController);
        var getTradeShip = DecisionTreeNodeCreator.CreateGetTradeShipForFree(gameController);

        var fleeingSucceeded = new DecisionTreeNode(null, true, new LoseOneFameMedalAction(gameController));

        var fightWithNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { cannotFly, getTradeShip },
            false,
            new FightEncounterAction(gameController, FightCategory.CANNON, FightEncounterOpponent.FIRST_LEFT)
        );
        var hasMoreBoosters = new DecisionTreeNode(
            new List<DecisionTreeNode>() { fightWithNeighor, fleeingSucceeded },
            true,
            new HasMoreBoostersThanNeighborAction(gameController, FightEncounterOpponent.FIRST_LEFT)
        );


        var youWantToFlee = new DecisionTreeNode(
            new List<DecisionTreeNode>() { hasMoreBoosters, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterYouWantToFlee()
        };

        return EncounterCard.FromRootNode(youWantToFlee);
    }

    public EncounterCard CreateEncounterCard19()
    {
        var removeUpgrade = DecisionTreeNodeCreator.CreateRemoveUpgradeNode(gameController);
        var getTradeShip = DecisionTreeNodeCreator.CreateGetTradeShipForFree(gameController);

        var fightWithNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { removeUpgrade, getTradeShip },
            true,
            new FightEncounterAction(gameController, FightCategory.BOOSTER, FightEncounterOpponent.SECOND_RIGHT)
        );
        var noAction = DecisionTreeNodeCreator.CreateNoActionNode(gameController);

        var youWantToFlee = new DecisionTreeNode(
            new List<DecisionTreeNode>() { noAction, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterSomeoneHasProblemsWithShip()
        };

        return EncounterCard.FromRootNode(youWantToFlee);
    }

    public EncounterCard CreateEncounterCard20()
    {
        var removeUpgrade = DecisionTreeNodeCreator.CreateRemoveUpgradeNode(gameController, 1);
        var getResourceFromOpponents = new DecisionTreeNode(null, true, new GetResourceFromEveryPlayer(gameController)); //TODO

        var fightWithNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { removeUpgrade, getResourceFromOpponents },
            true,
            new FightEncounterAction(gameController, FightCategory.BOOSTER, FightEncounterOpponent.FIRST_LEFT)
        );
        var loseOneMedal = DecisionTreeNodeCreator.CreateLoseOneFameMedal(gameController);


        var youWantToFlee = new DecisionTreeNode(
            new List<DecisionTreeNode>() { loseOneMedal, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterSomeoneHasProblemsWithShip()
        };

        return EncounterCard.FromRootNode(youWantToFlee);
    }

    public EncounterCard CreateEncounterCard21()
    {
        var removeUpgrade = DecisionTreeNodeCreator.CreateRemoveUpgradeNode(gameController);
        var getUpgradeForFree = DecisionTreeNodeCreator.CreateGetUpgradeForFree(gameController, 1);

        var fightWithNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { removeUpgrade, getUpgradeForFree },
            true,
            new FightEncounterAction(gameController, FightCategory.BOOSTER, FightEncounterOpponent.FIRST_LEFT)
        );
        var loseOneMedal = new DecisionTreeNode(
            null,
            false,
            new LoseOneFameMedalAction(gameController)
        );


        var youWantToFlee = new DecisionTreeNode(
            new List<DecisionTreeNode>() { loseOneMedal, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterSomeoneHasProblemsWithShip()
        };

        return EncounterCard.FromRootNode(youWantToFlee);
    }

    public EncounterCard CreateEncounterCard22()
    {

        var giftedHand = new Hand();
        giftedHand.AddCard(new GoodsCard());
        giftedHand.AddCard(new GoodsCard());
        var beute = DecisionTreeNodeCreator.CreateReceiveResources(gameController, 1, giftedHand);
        var cannotFly = DecisionTreeNodeCreator.CreateShipCannotFlyNode(gameController);

        var fightWithNeighor = new DecisionTreeNode(
            new List<DecisionTreeNode>() { cannotFly, beute },
            true,
            new FightEncounterAction(gameController, FightCategory.BOOSTER, FightEncounterOpponent.FIRST_LEFT)
        );
        var noAction = DecisionTreeNodeCreator.CreateNoActionNode(gameController);


        var youWantToFlee = new DecisionTreeNode(
            new List<DecisionTreeNode>() { noAction, fightWithNeighor }, null, new YesOrNoEncounterAction(gameController))
        {
            text = GameText.EncounterOtherStarfarerIsAttackedByPilot()
        };

        return EncounterCard.FromRootNode(youWantToFlee);
    }

}