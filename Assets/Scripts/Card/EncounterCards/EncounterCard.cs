using System;
using System.Collections.Generic;


public abstract class EncounterCard
{
    public EncounterCard()
    {
    }

    public abstract void Execute();

    public abstract DialogOption[] GetDialogOptions();
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

public abstract class EncounterCardAction
{
    public abstract void ExecuteAction(Player player);
}

public class GiveResourcesEncounterCard : EncounterCard
{
    protected string text;
    protected EncounterCardOption[] options;
    public GiveResourcesEncounterCard(string text, EncounterCardOption[] options)
    {
        this.text = text;
        this.options = options;
    }

    public override void Execute()
    {
        throw new NotImplementedException();
    }

    public override DialogOption[] GetDialogOptions()
    {
        return new DialogOption[]
        {
            new DialogOption("first", "1", 1),
            new DialogOption("first", "2", 2),
            new DialogOption("first", "3", 3),
        };
    }
}

public class YesOrNoEncounterCard : EncounterCard
{
    public YesOrNoActionTree tree;
    public YesOrNoEncounterCard(string text, YesOrNoActionTree tree)
    {
        this.tree = tree;
    }

    public override void Execute()
    {
        throw new NotImplementedException();
    }

    public override DialogOption[] GetDialogOptions()
    {
        return new DialogOption[] {
            new DialogOption("yesOption", "Yes", true),
            new DialogOption("noOption", "No", false),
        };
    }
}

public class YesOrNoActionTree
{
    public YesOrNoActionTreeNode root;

    public YesOrNoActionTree(YesOrNoActionTreeNode root)
    {
        this.root = root;
    }
}

public class YesOrNoActionTreeNode
{
    public string text;
    public EncounterCardAction action;
    public YesOrNoActionTreeNode yes;
    public YesOrNoActionTreeNode no;

    public YesOrNoActionTreeNode(string text, EncounterCardAction action, YesOrNoActionTreeNode yesNode, YesOrNoActionTreeNode noNode)
    {
        this.text = text;
        this.action = action;
        this.yes = yesNode;
        this.no = noNode;
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

public class CannonFightEncounterAction : EncounterCardAction
{
    public Player origin;
    public Player opponent;
    public CannonFightEncounterAction(Player origin, Player opponent)
    {
        this.origin = origin;
        this.opponent = opponent;
    }

    public override void ExecuteAction(Player player)
    {
        throw new NotImplementedException();
    }
}

