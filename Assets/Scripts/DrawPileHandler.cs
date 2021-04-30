using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;


public abstract class DrawPileStack
{
    protected List<ResourceCard> cards = new List<ResourceCard>();

    public abstract void Add(ResourceCard card);

    public int CardsLeft()
    {
        return cards.Count;
    }

    public ResourceCard[] DrawCardsFromTop(int amount, bool shuffleBefore = false)
    {
        if (amount > CardsLeft())
        {
            throw new ArgumentException("Not enough cards left for that operation");
        }

        var returnList = new List<ResourceCard>();
        if (shuffleBefore)
        {
            cards.Shuffle();
        }
        for (int i = 0; i < amount; i++)
        {
            var card = cards.PopAt(0);
            returnList.Add(card);
        }
        return returnList.ToArray();
    }

    public List<ResourceCard> GetCards()
    {
        return this.cards;
    }
}

public class SingleResourceDrawPileStack : DrawPileStack
{
    public Type cardType { get; set; }

    public SingleResourceDrawPileStack(Type cardType) {
        this.cardType = cardType;
    }

    public override void Add(ResourceCard card)
    {
        if (card.GetType() != this.cardType)
        {
            throw new ArgumentException("Can only add cards of type " + this.cardType + " to this pile.");
        }
        cards.Add(card);
    }
}

public class MultipleResourcesDrawPileStack : DrawPileStack
{
    public MultipleResourcesDrawPileStack()
    {
    }

    public override void Add(ResourceCard card)
    {
        cards.Add(card);
    }
}

public interface Observer {
    void SubjectDataChanged(object[] data);
}

[Serializable]
public abstract class Subject {

    [NonSerialized]
    protected List<Observer> observers = new List<Observer>();

    [OnDeserialized]
    private void OnDeserialized()
    {
        observers = new List<Observer>();
    }

    public List<Observer> GetObservers()
    {
        return observers;
    }

    public void RegisterObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
        observers.Remove(observer);
    }

    protected void Notify(object[] data)
    {

        foreach (var observer in Helper.CreateCopyOfList(observers))
        {
            observer.SubjectDataChanged(data);
        }
    }

} 

public class AvailablePiles : Subject
{
    int cardsOfEachTypeInHiddenPile = 8;
    int cardsOfEachTypeInOpenPile = 10;
    List<SingleResourceDrawPileStack> openDrawPiles;
    public MultipleResourcesDrawPileStack hiddenDrawPile;

    public AvailablePiles()
    {
        Init();
    }

    void Init()
    {
        hiddenDrawPile = new MultipleResourcesDrawPileStack();
        openDrawPiles = new List<SingleResourceDrawPileStack>();
        foreach (var card in new Helper().GetAllResourceCardTypes())
        {
            for(int i = 0; i < cardsOfEachTypeInHiddenPile; i++)
            {
                hiddenDrawPile.Add(card);
            }

            var stack = new SingleResourceDrawPileStack(card.GetType());
            for (int i = 0; i < cardsOfEachTypeInOpenPile; i++)
            {
                stack.Add(card);
            }
            openDrawPiles.Add(stack);
        }
        Notify(null);
    }

    public Hand GetOpenResourcesCombinedHand()
    {
        Hand hand = new Hand();
        foreach (var pile in openDrawPiles)
        {
            foreach(var card in pile.GetCards())
            {
                hand.AddCard(card);
            }
        }
        return hand;
    }

    SingleResourceDrawPileStack FindOpenResourceCardStack(Type resourceCardType)
    {
        foreach (var stack in openDrawPiles)
        {
            if (stack.cardType == resourceCardType)
            {
                return stack;
            }
        }
        return null;
    }

    public ResourceCard[] DrawCardsFromHiddenDrawPile(int amount)
    {
        var cards = hiddenDrawPile.DrawCardsFromTop(amount, true);
        Notify(null);
        return cards;
    }

    public ResourceCard[] DrawCardsOfOpenDrawPile(int amount, Type resourceCardType)
    {
        var stack = FindOpenResourceCardStack(resourceCardType);
        if (stack != null)
        {
            var cards = stack.DrawCardsFromTop(amount);
            Notify(null);
            return cards;
        } else
        {
            throw new ArgumentException("Could not find stack with that resource type");
        }
    }
}

public class DrawPileHandler : MonoBehaviour
{

    public AvailablePiles availablePiles;
    public GameObject drawPileView;

    public GameObject simpleHandRenderer;
    void Start()
    {
        availablePiles = new AvailablePiles();
        drawPileView.GetComponent<DrawPileView_>().Init(availablePiles, this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
