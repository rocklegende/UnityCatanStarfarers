using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        foreach(var pair in pairs)
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
}
;
public class DecisionTreeNode
{
    public readonly List<DecisionTreeNode> nextNodes;
    public readonly object decisionValue;
    public readonly EncounterCardAction action;
    public string buttonText { get; set; }
    public string text { get; set; }

    public DecisionTreeNode(List<DecisionTreeNode> nextNodes, object decisionValue, EncounterCardAction action)
    {
        this.nextNodes = nextNodes;
        this.decisionValue = decisionValue;
        this.action = action;
    }

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

public class  EncounterCardStack
{
    List<EncounterCard> cards;
    public EncounterCardStack(List<EncounterCard> cards)
    {
        this.cards = cards;
    }

    public EncounterCard Pop()
    {
        return cards.ShuffleAndPop();
    }

    public bool IsEmpty()
    {
        return cards.Count <= 0;
    }
}

public abstract class EncounterCardAction
{
    public Action<EncounterActionValue> callback;
    public HUDScript hud;
    public MapScript mapScript;
    public GameController gameController;

    public EncounterCardAction(GameController gameController)
    {
        this.gameController = gameController;
        this.mapScript = gameController.Map.GetComponent<MapScript>();
        this.hud = gameController.HUD.GetComponent<HUDScript>();
    }

    public DecisionDialog GetDecisionDialog()
    {
        return hud.decisionDialog.GetComponent<DecisionDialog>();
    }

    public abstract void Execute();

    public void SetCallback(Action<EncounterActionValue> callback)
    {
        this.callback = callback;
    }

    protected void ResultFound(object value)
    {
        callback(new EncounterActionValue(value));
    }
}

public class GiveupResourcesEncounterAction : EncounterCardAction
{
    int limit;
    public Hand pickedHand;
    public GiveupResourcesEncounterAction(GameController gameController, int limit) : base(gameController)
    {
        this.limit = limit;
    }

    public override void Execute()
    {
        hud.OpenResourcePicker(ResourcesPicked, limit);
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
        if (fameMedalGain > 0)
        {
            for (int i = 0; i < fameMedalGain; i++)
            {
                hud.player.AddFameMedal();
            }
        } else
        {
            for (int j = 0; j < -fameMedalGain; j++)
            {
                hud.player.RemoveFameMedal();
            }
        }
        
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
    public GetOneUpgradeForFree(GameController gameController, int fameMedalGain = 0) : base(gameController)
    {
        this.fameMedalGain = fameMedalGain;
    }

    public override void Execute()
    {
        hud.OpenUpgradeSelection(new List<Token>() {
            new BoosterUpgradeToken(),
            new CannonUpgradeToken(),
            new FreightPodUpgradeToken()
        }, UpgradeTokenSelected);

        
        for (int i = 0; i < fameMedalGain; i++)
        {
            if (fameMedalGain > 0)
            {
                hud.player.AddFameMedal();
            } else 
            {
                hud.player.RemoveFameMedal();
            }
        }

        hud.decisionDialog.GetComponent<DecisionDialog>().SetText("You get a free upgrade, please choose one");
    }

    void UpgradeTokenSelected(List<SFModel> selectedObjects)
    {
        foreach (var obj in selectedObjects)
        {
            if (obj is Token)
            {
                var token = (Token)obj;
                hud.player.BuildUpgradeWithoutCost(token);
                
            }
            else
            {
                throw new System.ArgumentException("selected element is not Token, that should not happen!");
            }
        }
        hud.CloseSelection();
        ResultFound(true);
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
        }) ;
    }

    void DialogOptionChosen(DialogOption option)
    {
        ResultFound(option.value);
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

public class FightEncounterAction : EncounterCardAction
{
    FightCategory category;
    public FightEncounterAction(GameController gameController, FightCategory category) : base(gameController)
    {
        this.category = category;
    }

    public override void Execute()
    {
        hud.fightPanel.SetActive(true);
        var fightPanelScript = hud.fightPanel.GetComponent<FightPanelScript>();
        fightPanelScript.SetFightIsDoneCallback(FightIsDone);
        fightPanelScript.PlayFight(hud.player, hud.player, category);
    }

    void FightIsDone(bool result)
    {
        hud.fightPanel.SetActive(false);
        ResultFound(result);
    }
}

