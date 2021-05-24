using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.onebuckgames.UnityStarFarers;
using Photon.Realtime;
using Photon.Pun;

public class EncounterCard
{
    public DecisionTree decisionTree;
    public EncounterCard(DecisionTree decisionTree)
    {
        this.decisionTree = decisionTree;
    }

    public static EncounterCard FromRootNode(DecisionTreeNode root)
    {
        return new EncounterCard(new DecisionTree(root));
    }
}

public class EncounterActionValue
{
    public object value;

    public EncounterActionValue(object value)
    {
        this.value = value;
    }
}

public class DecisionTree
{
    public DecisionTreeNode root;

    public DecisionTree(DecisionTreeNode root)
    {
        this.root = root;
    }

    public DecisionTreeNode FindNodeWithAction<T>() where T : EncounterCardAction
    {
        // Breadth First Search
        var queue = new List<DecisionTreeNode>() { root };
        while (queue.Count > 0)
        {
            var node = queue.PopAt(0);
            if (node.action is T)
            {
                return node;
            }
            queue.AddRange(node.nextNodes);
        }

        return null;
    }
}

public class DecisionValueActionPair {
    public object decisionValue;
    public EncounterCardAction action;
    public DecisionValueActionPair(object decisionValue, EncounterCardAction action)
    {
        this.decisionValue = decisionValue;
        this.action = action;
    }
}

public class DecisionTreeNodeCreator
{
    public static DecisionTreeNode CreatePickResourcesNode(List<DecisionValueActionPair> pairs, GameController gameController)
    {
        var nextNodesList = new List<DecisionTreeNode>();
        var max = -1;
        foreach (var pair in pairs)
        {
            if ((int)pair.decisionValue > max)
            {
                max = (int)pair.decisionValue;
            }
            nextNodesList.Add(new DecisionTreeNode(null, pair.decisionValue, pair.action));
        }


        var node = new DecisionTreeNode(nextNodesList, null, new GiveupResourcesEncounterAction(gameController, max));
        return node;
    }

    public static DecisionTreeNode Create0to3ResourceNode(GameController gameController, EncounterCardAction zero, EncounterCardAction one, EncounterCardAction two, EncounterCardAction three)
    {
        var pairs = new List<DecisionValueActionPair>()
        {
            new DecisionValueActionPair(0, zero),
            new DecisionValueActionPair(1, one),
            new DecisionValueActionPair(2, two),
            new DecisionValueActionPair(3, three)
        };
        return CreatePickResourcesNode(pairs, gameController);
    }

    // Leaves are all the same practically, therefore these helper methods here

    public static DecisionTreeNode CreateShipCannotFlyNode(GameController gameController, int fameMedalGain = -1)
    {
        return new DecisionTreeNode(null, false, new ShipCannotFlyAction(gameController, fameMedalGain));
    }

    public static DecisionTreeNode CreateNoActionNode(GameController gameController)
    {
        return new DecisionTreeNode(null, false, new NoCounterGift(gameController));
    }

    public static DecisionTreeNode CreateRemoveUpgradeNode(GameController gameController, int fameMedalGain = -1)
    {
        return new DecisionTreeNode(null, false, new RemoveUpgradeAction(gameController, fameMedalGain));
    }

    public static DecisionTreeNode CreateGetUpgradeForFree(GameController gameController, int fameMedalGain = -1)
    {
        return new DecisionTreeNode(null, true, new GetOneUpgradeForFree(gameController, fameMedalGain));
    }

    public static DecisionTreeNode CreateGetTradeShipForFree(GameController gameController)
    {
        return new DecisionTreeNode(null, true, new GetTradeShipForFree(gameController));
    }

    public static DecisionTreeNode CreateLoseOneFameMedal(GameController gameController)
    {
        return new DecisionTreeNode(null, false, new LoseOneFameMedalAction(gameController));
    }

    public static DecisionTreeNode CreateReceiveResources(
        GameController gameController,
        int fameMedalGain,
        Hand giftedCards = null,
        int cardsSelectableFreely = -1
    )
    {
        return new DecisionTreeNode(null, true, new ReceiveResourcesAndReceiveFameMedalAction(gameController, fameMedalGain, giftedCards, cardsSelectableFreely));
    }

    public static DecisionTreeNode CreateRaumsprung(
        GameController gameController,
        int fameMedalGain = -1
    )
    {
        return new DecisionTreeNode(null, true, new RaumsprungAction(gameController, fameMedalGain));
    }
}
;
public class DecisionTreeNode
{
    public readonly List<DecisionTreeNode> nextNodes;
    public readonly object decisionValue;
    public readonly EncounterCardAction action;
    public string buttonText { get; set; }
    public string text { get; set; }

    /// <summary>
    /// Initialize with single action.
    /// </summary>
    /// <param name="nextNodes"></param>
    /// <param name="decisionValue"></param>
    /// <param name="action"></param>
    public DecisionTreeNode(List<DecisionTreeNode> nextNodes, object decisionValue, EncounterCardAction action)
    {
        this.nextNodes = nextNodes;
        this.decisionValue = decisionValue;
        this.action = action;
    }

    ///// <summary>
    ///// Initialize with multiple actions that will be executed in the order that the actions are given.
    ///// </summary>
    ///// <param name="nextNodes"></param>
    ///// <param name="decisionValue"></param>
    ///// <param name="actions"></param>
    //public DecisionTreeNode(List<DecisionTreeNode> nextNodes, object decisionValue, List<EncounterCardAction> actions)
    //{
    //    this.nextNodes = nextNodes;
    //    this.decisionValue = decisionValue;
    //    this.actions = actions;
    //}

    public bool HasNext()
    {
        if (nextNodes == null)
        {
            return false;
        }
        return nextNodes.Count > 0;
    }

    public List<DecisionTreeNode> GetNext()
    {
        return nextNodes;
    }

    public bool GetsTriggeredByValue(object value)
    {
        // special case if node gets triggered by multiple values
        if (value is int && decisionValue is List<int>)
        {
            var decisionValues = (List<int>)decisionValue;
            var intValue = (int)value;
            return decisionValues.Contains(intValue);
        }
        return decisionValue.Equals(value);
    }

    public DialogOption[] GetDialogOptions()
    {
        if (GetNext() == null)
        {
            return new DialogOption[] { };
        }
        var options = new List<DialogOption>();
        foreach (var nextNode in nextNodes)
        {
            options.Add(new DialogOption("jo", nextNode.buttonText, nextNode.decisionValue));
        }

        return options.ToArray();
    }
}

public abstract class EncounterCardStack
{
    protected List<EncounterCard> cards;
    public EncounterCardStack(List<EncounterCard> cards)
    {
        this.cards = cards;
    }

    public abstract void Refill();

    public EncounterCard Pop()
    {
        return cards.ShuffleAndPop();
    }

    public bool IsEmpty()
    {
        return cards.Count <= 0;
    }
}

public class DefaultEncounterCardStack : EncounterCardStack
{
    GameController gameController;
    public DefaultEncounterCardStack(GameController gameController) : base(new List<EncounterCard>())
    {
        this.gameController = gameController;
        this.cards = GenerateCards(gameController);
    }

    List<EncounterCard> GenerateCards(GameController gameController)
    {
        var factory = new EncounterCardFactory(gameController);
        return new List<EncounterCard>()
        {
            factory.CreateEncounterCard1(),
            factory.CreateEncounterCard2(),
        };
    }

    public override void Refill()
    {
        this.cards = GenerateCards(gameController);
        this.cards.Shuffle();
    }
}

public abstract class EncounterCardAction
{
    public Action<EncounterActionValue> callback;
    public HUDScript hud;
    public MapScript mapScript;
    public GameController gameController;
    public object PredeterminedResult;

    public EncounterCardAction(GameController gameController)
    {
        this.gameController = gameController;
        this.mapScript = gameController.GetMapScript();
        this.hud = gameController.GetHUDScript();
    }

    public DecisionDialog GetDecisionDialog()
    {
        return hud.decisionDialog.GetComponent<DecisionDialog>();
    }

    public void ExecuteTemplateMethod()
    {
        if (PredeterminedResult != null)
        {
            ResultFound(PredeterminedResult);
        } else
        {
            Execute();
        }
    }

    public abstract void Execute();

    public void SetCallback(Action<EncounterActionValue> callback)
    {
        this.callback = callback;
    }

    public void ResultFound(object value)
    {
        callback(new EncounterActionValue(value));
    }
}

public class GiveupResourcesEncounterAction : EncounterCardAction
{
    int limit;
    int onlySelectableAtValue;
    public Hand pickedHand;
    public GiveupResourcesEncounterAction(GameController gameController, int limit, int onlySelectableAtValue = -1) : base(gameController)
    {
        this.limit = limit;
        this.onlySelectableAtValue = onlySelectableAtValue;
    }

    public override void Execute()
    {
        hud.OpenResourcePicker(ResourcesPicked, limit, onlySelectableAtValue);
        GetDecisionDialog().SetText(String.Format("A starfarers want up to {0} resources, how much do you give (up to {0})?", limit));
    }



    void ResourcesPicked(Hand hand)
    {
        pickedHand = hand;
        hud.player.SubtractHand(hand);
        hud.CloseResourcePicker();
        ResultFound(hand.Count());

    }
}

public class GetResourceFromBankEncounterAction : EncounterCardAction
{
    int numResources;
    public GetResourceFromBankEncounterAction(GameController gameController, int numResources) : base(gameController)
    {
        this.numResources = numResources;
    }

    public override void Execute()
    {
        hud.OpenResourcePicker(ResourcesPicked);
        hud.resourcePicker.GetComponent<ResourcePicker>().SetTotalMaximum(numResources);
    }

    void ResourcesPicked(Hand hand)
    {
        hud.CloseResourcePicker();
        hud.player.SubtractHand(hand);
        ResultFound(hand.Count());

    }
}

public class NoCounterGift : EncounterCardAction
{
    public NoCounterGift(GameController gameController) : base(gameController)
    {

    }



    public override void Execute()
    {
        ResultFound(true);
    }
}

public class PirateLeaves : EncounterCardAction
{
    public PirateLeaves(GameController gameController) : base(gameController)
    {

    }



    public override void Execute()
    {
        ResultFound(true);
    }
}

public class ReturnGift : EncounterCardAction
{
    public GiveupResourcesEncounterAction giveupResourcesAction;
    public ReturnGift(GameController gameController) : base(gameController)
    {

    }



    public override void Execute()
    {
        gameController.mainPlayer.AddHand(giveupResourcesAction.pickedHand);
        ResultFound(true);
    }
}

public class ReceiveResourcesAndReceiveFameMedalAction : EncounterCardAction
{
    int fameMedalGain;
    Hand giftedCards;
    int cardsSelectableFreely;
    DecisionDialog decisionDialog;

    public ReceiveResourcesAndReceiveFameMedalAction(
        GameController gameController,
        int fameMedalGain,
        Hand giftedCards = null,
        int cardsSelectableFreely = -1
        ) : base(gameController)
    {
        this.fameMedalGain = fameMedalGain;
        this.giftedCards = giftedCards;
        this.cardsSelectableFreely = cardsSelectableFreely;
        this.decisionDialog = GetDecisionDialog();
    }

    void ProcessFameMedalGain()
    {
        FameMedalGainStrategy.HandleFameMedalGain(fameMedalGain, gameController.mainPlayer);
    }

    bool CanSelectCardsFreely()
    {
        return giftedCards == null && cardsSelectableFreely != -1;
    }

    bool GetsCardsGifted()
    {
        return giftedCards != null && cardsSelectableFreely == -1;
    }

    System.Collections.IEnumerator GiftCardsCoroutine(int waitTimeBeforeReturning)
    {
        hud.player.AddHand(giftedCards);
        decisionDialog.SetText(
            String.Format("You get the following resources gifted: {0}, and gain {1} fame medals.", giftedCards.GetStringRepresentation(), fameMedalGain)
        );
        yield return new WaitForSeconds(waitTimeBeforeReturning);
        finished();
    }

    void ProcessReceivingResources()
    {
        if (GetsCardsGifted())
        {
            hud.StartCoroutine(GiftCardsCoroutine(4));
        }

        if (CanSelectCardsFreely())
        {
            hud.OpenResourcePicker(ResourcesPicked, cardsSelectableFreely, cardsSelectableFreely);
        }

    }

    void ResourcesPicked(Hand hand)
    {
        hud.CloseResourcePicker();
        hud.player.AddHand(hand);
        finished();
    }

    void finished()
    {
        ResultFound(true);
    }

    public override void Execute()
    {
        ProcessFameMedalGain();
        ProcessReceivingResources();

        if (CanSelectCardsFreely())
        {
            decisionDialog.SetText(
                String.Format("You can select {0} free resources, and gain {1} fame medals. Please Select {0} resources.", cardsSelectableFreely, fameMedalGain)
            );
        }

        if (GetsCardsGifted())
        {
            decisionDialog.SetText(
                String.Format("You get the following resources gifted: {0}, and gain {1} fame medals.", giftedCards.GetStringRepresentation(), fameMedalGain)
            );
        }

    }
}

public class ShipCannotFlyAction : EncounterCardAction
{
    int fameMedalGain;
    public ShipCannotFlyAction(GameController gameController, int fameMedalGain = 0) : base(gameController) {
        this.fameMedalGain = fameMedalGain;
    }

    public override void Execute()
    {
        FameMedalGainStrategy.HandleFameMedalGain(fameMedalGain, gameController.mainPlayer);
        var flyableTokensOfMainPlayer = gameController.mainPlayer.tokens
            .Where(tok => tok.HasShipTokenOnTop()).ToList();
        hud.decisionDialog.SetActive(false);
        mapScript.OpenTokenSelection(flyableTokensOfMainPlayer, DidSelectToken);
    }

    public void DidSelectToken(Token token)
    {
        Debug.Log("Some token was selected, disabling it for flying");
        token.Disable();
        gameController.mainPlayer.AddFameMedals(fameMedalGain);
        ResultFound(true);
    }
}

public class GetTradeShipForFree : EncounterCardAction
{
    //TODO: read the exact rules on this card, when is it possible to build this ship?
    public GetTradeShipForFree(GameController gameController) : base(gameController)
    {

    }

    public override void Execute()
    {
        gameController.mainPlayer.AddGiftedToken(new TradeBaseToken());
        ResultFound(true);
    }
}

public class GetOneUpgradeForFree : EncounterCardAction
{
    int fameMedalGain;
    public readonly List<Upgrade> selectableUpgrades;
    public GetOneUpgradeForFree(GameController gameController, int fameMedalGain = 0) : base(gameController)
    {
        this.fameMedalGain = fameMedalGain;
        selectableUpgrades = gameController.mainPlayer.ship.GetUpgradesThatAreNotFull();
    }

    public override void Execute()
    {
        FameMedalGainStrategy.HandleFameMedalGain(fameMedalGain, gameController.mainPlayer);
        if (selectableUpgrades.Count == 0)
        {
            ResultFound(true); // player has no space for addtional tokens, therefore we end this action here
            return;
        }

        hud.OpenUpgradeSelection(selectableUpgrades, UpgradeSelected, 1);
    }

    void UpgradeSelected(List<int> selectedIndexes)
    {
        foreach (var index in selectedIndexes)
        {
            hud.player.BuildUpgradeWithoutCost(selectableUpgrades[index]);
        }
        hud.CloseSelection();
        ResultFound(true);
    }
}

public class GetResourceFromEveryPlayer : EncounterCardAction
{
    int numResources;
    public GetResourceFromEveryPlayer(GameController gameController, int numResources = 1) : base(gameController)
    {
        this.numResources = numResources;
    }

    public override void Execute()
    {
        var mainPlayer = gameController.mainPlayer;
        var action = new RemoteClientAction(RemoteClientActionType.GIVE_RESOURCE, new object[] { numResources }, mainPlayer);

        var otherPlayers = GetTargetPlayers(gameController.players);
        if (otherPlayers.Count == 0)
        {
            ResultFound(true);
            return;
        }

        var dispatcher = new DefaultRemoteActionDispatcher(gameController);
        dispatcher.SetAction(action);
        dispatcher.SetTargets(otherPlayers);
        dispatcher.MakeRequest((response) => { }, ResponseReceived);
    }

    public List<Player> GetTargetPlayers(List<Player> players)
    {
        var targetPlayers = Helper.CreateCopyOfList(players);
        targetPlayers.Remove(gameController.mainPlayer);

        foreach (var otherPlayer in targetPlayers)
        {
            if (otherPlayer.hand.Count() < numResources)
            {
                targetPlayers.Remove(otherPlayer);
            }
        }
        return targetPlayers;
    }

    public void ResponseReceived(Dictionary<string, RemoteActionCallbackData> dict)
    {
        Debug.Log("ResultFromEveryPlayerWasFound!");
        ResultFound(true);
    }
}

public class RemoveUpgradeAction : EncounterCardAction
{
    int fameMedalGain;
    List<Upgrade> selectableUpgrades;
    public Action doneCallback;
    public RemoveUpgradeAction(GameController gameController, int fameMedalGain = -1) : base(gameController)
    {
        this.fameMedalGain = fameMedalGain;
        selectableUpgrades = gameController.mainPlayer.ship.GetRemovableUpgrades();
    }

    public override void Execute()
    {
        FameMedalGainStrategy.HandleFameMedalGain(fameMedalGain, gameController.mainPlayer);
        if (selectableUpgrades.Count == 0)
        {
            ResultFound(true); // player has no tokens to lose, therefore we end this action here
            return;
        }

        hud.OpenUpgradeSelection(selectableUpgrades, UpgradeTokenSelected, 1);
    }

    void UpgradeTokenSelected(List<int> selectedIndexes)
    {
        foreach (var index in selectedIndexes)
        {
            gameController.mainPlayer.RemoveUpgrade(selectableUpgrades[index]);
        }
        hud.CloseSelection();
        ResultFound(true);
        doneCallback?.Invoke();
    }
}

public class EncounterFinishedAction : EncounterCardAction
{
    public EncounterFinishedAction(GameController gameController) : base(gameController)
    {

    }

    public override void Execute()
    {
        var decisionDialogScript = hud.decisionDialog.GetComponent<DecisionDialog>();
        decisionDialogScript.SetText("Encounter is finished");
        decisionDialogScript.SetCallback(DialogOptionChosen);
        decisionDialogScript.SetOptions(new DialogOption[] {
            new DialogOption("ok", "Ok", true)
        });
    }

    void DialogOptionChosen(DialogOption option)
    {
        ResultFound(option.value);
    }
}

public class LoseOneFameMedalAction : EncounterCardAction
{
    public LoseOneFameMedalAction(GameController gameController) : base(gameController)
    {

    }

    public override void Execute()
    {
        hud.player.RemoveFameMedal();
        var decisionDialogScript = hud.decisionDialog.GetComponent<DecisionDialog>();
        decisionDialogScript.SetCallback(DialogOptionChosen);
        decisionDialogScript.SetOptions(new DialogOption[] {
            new DialogOption("ok", "Ok", true)
        });
    }

    void DialogOptionChosen(DialogOption option)
    {
        ResultFound(option.value);
    }
}

public class LoseHandAction : EncounterCardAction
{

    public MakeTradeAction makeTradeAction;

    public LoseHandAction(GameController gameController) : base(gameController)
    {

    }

    public override void Execute()
    {
        if (makeTradeAction != null)
        {
            gameController.mainPlayer.SubtractHand(makeTradeAction.outputHand);
        }
    }
}

public class ShakeShipAction : EncounterCardAction
{
    public ShakeShipAction(GameController gameController) : base(gameController)
    {
        
    }

    public override void Execute()
    {
        hud.ShowShipDiceThrowPanel(ShipValueThrown);
    }

    void ShipValueThrown(ShipDiceThrow shipDiceThrow)
    {
        //hud.CloseShipDiceThrowPanel();
        ResultFound(shipDiceThrow.GetRawValue());
    }
}

public class MakeTradeAction : EncounterCardAction
{
    int numCardsToGive;
    int numCardsToReceive;
    public Hand inputHand;
    public Hand outputHand;

    public MakeTradeAction(GameController gameController, int numCardsToGive, int numCardsToReceive) : base(gameController)
    {
        this.numCardsToGive = numCardsToGive;
        this.numCardsToReceive = numCardsToReceive;
    }

    public override void Execute()
    {
        hud.OpenTradePanelForExactTrade(numCardsToGive, numCardsToReceive, TradeMade);
    }

    public void TradeMade(Hand inputHand, Hand outputHand)
    {
        hud.CloseTradePanel();
        this.inputHand = inputHand;
        this.outputHand = outputHand;
        ResultFound(true);
    }
}


public class WinOneFameMedalAction : EncounterCardAction
{
    public WinOneFameMedalAction(GameController gameController) : base(gameController)
    {

    }

    public override void Execute()
    {
        hud.player.AddFameMedal();
        var decisionDialogScript = hud.decisionDialog.GetComponent<DecisionDialog>();
        decisionDialogScript.SetCallback(DialogOptionChosen);
        decisionDialogScript.SetOptions(new DialogOption[] {
            new DialogOption("ok", "Ok", true)
        });
    }

    void DialogOptionChosen(DialogOption option)
    {
        ResultFound(option.value);
    }

}

public class HasMoreBoostersThanNeighborAction : EncounterCardAction
{
    FightEncounterOpponent opponent;
    public HasMoreBoostersThanNeighborAction(GameController gameController, FightEncounterOpponent opponent) : base(gameController)
    {
        this.opponent = opponent;
    }

    public override void Execute()
    {
        var opponentPlayerIndex = EncounterCardHelper.GetOpponentPlayerIndex(opponent, gameController.currentPlayerAtTurnIndex, gameController.players.Count);
        var opponentPlayer = gameController.players[opponentPlayerIndex];
        ResultFound(HasEqualOrMoreBoosters(gameController.mainPlayer, opponentPlayer));
    }

    bool HasEqualOrMoreBoosters(Player origin, Player playerToCompareTo)
    {
        return origin.ship.Boosters >= playerToCompareTo.ship.Boosters;
    }
}

public class YesOrNoEncounterAction : EncounterCardAction
{
    public YesOrNoEncounterAction(GameController gameController) : base(gameController)
    {

    }

    public override void Execute()
    {
        var decisionDialogScript = hud.decisionDialog.GetComponent<DecisionDialog>();
        decisionDialogScript.SetCallback(DialogOptionChosen);
        decisionDialogScript.SetOptions(new DialogOption[] {
            new DialogOption("yes", "Yes", true),
            new DialogOption("no", "No", false),
        });
    }

    void DialogOptionChosen(DialogOption option)
    {
        ResultFound(option.value);
    }
}

public class DrawNewCardAction : EncounterCardAction
{
    bool refillStackBefore;
    public DrawNewCardAction(GameController gameController, bool refillStackBefore = false) : base(gameController)
    {
        this.refillStackBefore = refillStackBefore;
    }

    public override void Execute()
    {
        ResultFound(true);
        if (refillStackBefore)
        {
            gameController.encounterCardHandler.encounterCardStack.Refill();
        }

        gameController.encounterCardHandler.PlayNextCard();
    }
}

public class MostFreightPodsAction : EncounterCardAction
{
    public MostFreightPodsAction(GameController gameController) : base(gameController)
    {
        
    }

    public override void Execute()
    {
        GiveFameMedalToPlayersWithMostFreightPods(gameController.players);
        ResultFound(true);
    }

    public static void GiveFameMedalToPlayersWithMostFreightPods(List<Player> players)
    {
        if (players.Count == 0)
        {
            return;
        }
        List<Player> SortedList = players.OrderByDescending(player => player.ship.FreightPods).ToList();
        List<Player> PlayersWithMostFreightPods = new List<Player>();
        int maxFreightPods = SortedList[0].ship.FreightPods;
        foreach(var player in SortedList)
        {
            if (player.ship.FreightPods == maxFreightPods)
            {
                PlayersWithMostFreightPods.Add(player);
            }
        }
        // Payout
        foreach (var player in PlayersWithMostFreightPods)
        {
            player.AddFameMedal();
        }
    }
}

public class DiscardIfMoreThanLimitUpgradesAction : EncounterCardAction
{
    int limit;
    public DiscardIfMoreThanLimitUpgradesAction(GameController gameController, int limit) : base(gameController)
    {
        this.limit = limit;
    }

    public override void Execute()
    {
        var action = new RemoteClientAction(
            RemoteClientActionType.GIVEUP_UPGRADE,
            new object[] { 1 },
            gameController.mainPlayer
        );

        //var dispatcher = new RemoteActionDispatcher(gameController);
        //dispatcher.RequestActionFromPlayers(
            
        //    action,
        //    AllPlayersMadeDecision
        //);
        var dispatcher = new DefaultRemoteActionDispatcher(gameController);
        dispatcher.SetTargets(GetPlayersWithMoreThanLimit(gameController.players, this.limit));
        dispatcher.SetAction(action);
        dispatcher.MakeRequest((response) => { }, AllPlayersMadeDecision);
    }

    public static List<Player> GetPlayersWithMoreThanLimit(List<Player> players, int lim)
    {
        return players.Where(player => player.ship.UpgradesCountWithoutBonuses() > lim).ToList();
    }

    public void AllPlayersMadeDecision(Dictionary<string, RemoteActionCallbackData> dict)
    {
        ResultFound(true);
    }
}

public class RaumsprungAction : EncounterCardAction
{
    Token selectedToken;
    int fameMedalGain;
    int prevSteps;
    public RaumsprungAction(GameController gameController, int fameMedalGain = -1) : base(gameController)
    {
        this.fameMedalGain = fameMedalGain;
    }

    public override void Execute()
    {
        hud.decisionDialog.SetActive(false);
        FameMedalGainStrategy.HandleFameMedalGain(fameMedalGain, gameController.mainPlayer);
        var selectableTokens = gameController.mainPlayer.GetTokensThatCanFly();
        if (selectableTokens.Count == 0)
        {
            ResultFound(true);
            return;
        }
        mapScript.OpenTokenSelection(selectableTokens, TokenSelectedForRaumsprung);
    }

    void TokenSelectedForRaumsprung(Token token)
    {
        selectedToken = token;
        prevSteps = selectedToken.GetStepsLeft();
        selectedToken.addSteps(100);

        var pointsToChooseFrom = selectedToken.ReachableSpacePoints();
        mapScript.OpenSpacePointSelection(pointsToChooseFrom, MoveSelectedToken);
    }

    void MoveSelectedToken(SpacePoint newPosition)
    {
        selectedToken.FlyTo(newPosition);
        selectedToken.SetStepsLeft(prevSteps); //set original steps after flying the free raumsprung
        hud.decisionDialog.SetActive(true);
        ResultFound(true);
    }
}

public class RobPlayersAction : YesOrNoEncounterAction
{
    public RobPlayersAction(GameController gameController) : base(gameController)
    {

    }

    public static bool HasEnoughResources(Player player)
    {
        return player.hand.Count() >= 1;
    }

    public override void Execute()
    {
        base.Execute();
        if (!HasEnoughResources(gameController.mainPlayer)) 
        {
            Debug.Log("Main player has not enough resources to execute the rob them action, automatically choosing false");
            ResultFound(false);
        }
    }
}

public class OneForTwoTradeAction : YesOrNoEncounterAction
{
    public OneForTwoTradeAction(GameController gameController) : base(gameController)
    {

    }

    public static bool HasEnoughResources(Player player)
    {
        return player.hand.Count() >= 1;
    }

    public override void Execute()
    {
        base.Execute();
        if (!HasEnoughResources(gameController.mainPlayer))
        {
            Debug.Log("Main player has not enough resources to execute the one for two trade action, automatically choosing false");
            ResultFound(false);
        }
    }
}

public class GiveTwoResourcesAction : YesOrNoEncounterAction
{
    public GiveTwoResourcesAction(GameController gameController) : base(gameController)
    {

    }

    public static bool HasEnoughResources(Player player)
    {
        return player.hand.Count() >= 2;
    }

    public override void Execute()
    {
        base.Execute();
        if (!HasEnoughResources(gameController.mainPlayer))
        {
            Debug.Log("Main player has not enough resources to execute the one for two trade action, automatically choosing false");
            ResultFound(false);
        }
    }
}

public enum FightEncounterOpponent
{
    FIRST_LEFT,
    SECOND_LEFT,
    FIRST_RIGHT,
    SECOND_RIGHT
}

public class EncounterCardHelper
{
    public static int GetOpponentPlayerIndex(FightEncounterOpponent opponent, int currentPlayerIndex, int numberOfPlayers)
    {
        switch (opponent)
        {
            case FightEncounterOpponent.FIRST_RIGHT:
                return Helper.NextPlayerClockwise(currentPlayerIndex, numberOfPlayers);
            case FightEncounterOpponent.FIRST_LEFT:
                return Helper.PreviousPlayerClockwise(currentPlayerIndex, numberOfPlayers);
            case FightEncounterOpponent.SECOND_LEFT:
                return Helper.PreviousPlayerClockwiseStepsAway(currentPlayerIndex, numberOfPlayers, 2);
            case FightEncounterOpponent.SECOND_RIGHT:
                return Helper.NextPlayerClockwiseStepsAway(currentPlayerIndex, numberOfPlayers, 2);
        }
        return -1;
    }
}

public class FightEncounterAction : EncounterCardAction
{
    FightCategory category;
    FightEncounterOpponent opponent;
    public FightEncounterAction(GameController gameController, FightCategory category, FightEncounterOpponent opponent) : base(gameController)
    {
        this.category = category;
        this.opponent = opponent;
    }

    public override void Execute()
    {
        var opponentIndex = EncounterCardHelper.GetOpponentPlayerIndex(opponent, gameController.currentPlayerAtTurnIndex, gameController.players.Count);
        if (opponentIndex == -1)
        {
            throw new ArgumentException("Getting opponent index failed");
        }

        hud.fightPanel.SetActive(true);
        var fightPanelScript = hud.fightPanel.GetComponent<FightPanelScript>();
        fightPanelScript.SetFightIsDoneCallback(FightIsDone);
        fightPanelScript.PlayFight(gameController.mainPlayer, gameController.players[opponentIndex], category);
    }

    void FightIsDone(bool result)
    {
        hud.fightPanel.SetActive(false);
        ResultFound(result);
    }
}

