using System;
using System.Collections.Generic;


public class EncounterCard
{
    public DecisionTree decisionTree;
    public EncounterCard(DecisionTree decisionTree)
    {
        this.decisionTree = decisionTree;
    }
}

public class EncounterCardOption
{
    int numResources;
    EncounterCardAction action;
    public EncounterCardOption(int numResources, EncounterCardAction action)
    {
        if (numResources < 0)
        {
            throw new ArgumentException("Cant initialize EncounterCardOption with negative numResources value");
        }
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

public class DecisionTreeNode
{
    public readonly DecisionTreeNode[] nextNodes;
    public readonly object decisionValue;
    public readonly EncounterCardAction action;
    public string buttonText { get; set; }
    public string text { get; set; }

    public DecisionTreeNode(DecisionTreeNode[] nextNodes, object decisionValue, EncounterCardAction action)
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
        return nextNodes.Length > 0;
    }

    public DecisionTreeNode[] GetNext()
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

public class EncounterCardStack
{
    List<EncounterCard> cards;
    public EncounterCardStack(List<EncounterCard> cards)
    {
        this.cards = cards;
    }

    public EncounterCard Pop()
    {
        cards.Shuffle();
        var card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public bool IsEmpty()
    {
        return cards.Count <= 0;
    }
}

//public class CannonFightEncounterAction : EncounterCardAction
//{
//    public Player origin;
//    public Player opponent;
//    Action<int> callback; 
//    public CannonFightEncounterAction(Player origin, Player opponent)
//    {
//        this.origin = origin;
//        this.opponent = opponent;
//    }

//    public override void ExecuteAction()
//    {
//        throw new NotImplementedException();
//        hud.OpenResourcePicker(ResourcesPicked);
//    }

//    void ResourcesPicked(Hand hand)
//    {
//        ResultFound(hand.Count());
//        hud.CloseResourcePicker();
//    }
//}

public abstract class EncounterCardAction
{
    public Action<EncounterActionValue> callback;
    public HUDScript hud;

    public EncounterCardAction(HUDScript hud)
    {
        this.hud = hud;
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
    public GiveupResourcesEncounterAction(HUDScript hud) : base(hud)
    {
        
    }

    public override void Execute()
    {
        hud.OpenResourcePicker(ResourcesPicked);
    }

    void ResourcesPicked(Hand hand)
    {
        hud.player.SubtractHand(hand);
        ResultFound(hand.Count());
        hud.CloseResourcePicker();
    }
}

public class LoseOneFameMedalAction : EncounterCardAction
{
    public LoseOneFameMedalAction(HUDScript hud) : base(hud)
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
    public WinOneFameMedalAction(HUDScript hud) : base(hud)
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
    public YesOrNoEncounterAction(HUDScript hud) : base(hud)
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
    public FightEncounterAction(HUDScript hud) : base(hud)
    {

    }

    public override void Execute()
    {
        hud.fightPanel.SetActive(true);
        var fightPanelScript = hud.fightPanel.GetComponent<FightPanelScript>();
        fightPanelScript.SetFightIsDoneCallback(FightIsDone);
        fightPanelScript.PlayFight(hud.player, hud.player, FightCategory.CANNON);
    }

    void FightIsDone(bool result)
    {
        hud.fightPanel.SetActive(false);
        ResultFound(result);
    }
}

