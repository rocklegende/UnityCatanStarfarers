using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Hand : Subject
{

    public List<Card> cards = new List<Card>();

    public void AddCard(Card card)
    {
        cards.Add(card);
        Notify(null);
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        Notify(null);
    }

    public Hand GetRandomSubhandOfSize(int size)
    {
        if (size > Count())
        {
            throw new ArgumentException(string.Format("not enough resources to make a subhand of {0} card", size));
        }
        var randomHand = new Hand();
        var handClone = SimpleClone();
        handClone.cards.Shuffle();

        for (int i = 0; i < size; i++)
        {
            var card = handClone.cards.PopAt(0);
            randomHand.AddCard(card);
        }
        return randomHand;
    }

    public bool IsSubsetOf(Hand targetHand)
    {
        foreach(var cardType in new Helper().GetAllResourceCardTypes())
        {
            if (NumberCardsOfType(cardType.GetType()) > targetHand.NumberCardsOfType(cardType.GetType()))
            {
                return false;
            }
        }

        return true;
    }

    public int Count()
    {
        return cards.Count;
    }

    public Hand SimpleClone()
    {
        var clone = new Hand();
        clone.cards = Helper.CreateCopyOfList(cards);
        return clone;
    }

    /// <summary>
    /// Returns true if both hands have the exact same number of cards of each type
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool HasSameCardsAs(Hand other)
    {
        foreach(var cardType in new Helper().GetAllResourceCardTypes())
        {
            if (NumberCardsOfType(cardType.GetType()) != other.NumberCardsOfType(cardType.GetType()))
            {
                return false;
            }
        }
        return true;
    }

    public Hand()
    {
        
    }

    public static Hand FromResourceCards(List<ResourceCard> _cards)
    {
        var hand = new Hand();
        foreach(var card in _cards)
        {
            hand.AddCard(card);
        }
        return hand;
    }

    public static Hand FromResources(int food = 0, int goods = 0, int fuel = 0, int ore = 0, int carbon = 0)
    {
        var hand = new Hand();
        for (int i = 0; i < food; i++)
        {
            hand.AddCard(new FoodCard());
        }

        for (int i = 0; i < goods; i++)
        {
            hand.AddCard(new GoodsCard());
        }

        for (int i = 0; i < ore; i++)
        {
            hand.AddCard(new OreCard());
        }

        for (int i = 0; i < fuel; i++)
        {
            hand.AddCard(new FuelCard());
        }

        for (int i = 0; i < carbon; i++)
        {
            hand.AddCard(new CarbonCard());
        }
        return hand;

    }

    public string GetStringRepresentation()
    {
        var converted = cards.Select(card => card.GetType().Name).ToArray();
        return String.Join(", ", converted);
    }

    public Hand GetRandomHand(int numCards)
    {
        return GetRandomSubhandOfSize(numCards);
    }

    public void SubtractHand(Hand h)
    {
        foreach(var card in h.cards)
        {
            RemoveCardOfType(card.GetType());
        }
        Notify(null);
    }

    public void RemoveAllCards()
    {
        cards = new List<Card>();
        Notify(null);
    }

    public void AddHand(Hand hand)
    {
        cards.AddRange(hand.cards);
        Notify(null);
    }

    public int NumberCardsOfType<T>() where T : ResourceCard {

        int sum = 0;
        List<ResourceCard> resourceCards = GetAllResourceCards(cards);
        foreach (ResourceCard card in resourceCards)
        {
            if (card is T)
            {
                sum += 1;
            }
        }

        return sum;
    }

    public int NumberCardsOfType(Type type)
    {
        int sum = 0;
        List<ResourceCard> resourceCards = GetAllResourceCards(cards);
        foreach (ResourceCard card in resourceCards)
        {
            if (card.GetType() == type)
            {
                sum += 1;
            }
        }

        return sum;
    }

    public List<ResourceCard> GetAllResourceCards(List<Card> cards)
    {
        List<ResourceCard> resourceCards = new List<ResourceCard>();
        foreach (Card card in cards)
        {
            if (card is ResourceCard)
            {
                resourceCards.Add((ResourceCard)card);
            }
        }

        return resourceCards;
    }

    public bool CanPayCost(Cost cost)
    {

        if (cost.resources.Length == 0)
        {
            return true;
        }

        List<Card> cardsCopy = cards.GetRange(0, cards.Count);

        foreach (Resource rc in cost.resources)
        {
            List<ResourceCard> resourceCards = GetAllResourceCards(cardsCopy);
            ResourceCard bla = resourceCards.Find(c => c.resource.Name == rc.Name);

            if (bla != null)
            {
                cardsCopy.Remove(bla);
            } else
            {
                return false;
            }
        }

        return true;
    }

    public Card FindCardOfResource<T>() where T : Resource
    {
        foreach (Card c in cards)
        {
            if (c is ResourceCard)
            {
                var convert = (ResourceCard)c;
                if (convert.resource is T)
                {
                    return c;
                }
            }            
        }

        return null;
    }

    public T FindCardOfType<T>() where T : Card
    {
        foreach (Card c in cards)
        {
            if (c is T)
            {
                return (T)c;
            }
        }

        return null;
    }

    public void PayCost(Cost cost)
    {
        if (!CanPayCost(cost))
        {
            throw new NotEnoughResourcesException();
        }

        foreach (Resource rc in cost.resources)
        {
            List<ResourceCard> resourceCards = GetAllResourceCards(cards);
            ResourceCard bla = resourceCards.Find(c => c.resource.Name == rc.Name);
            if (bla != null) {
                cards.Remove(bla);
            }
        }
        Notify(null);
    }

    public void RemoveCardOfType(Type type)
    {
        var cardFound = FindCardOfType(type);
        if (cardFound != null)
        {
            cards.Remove(cardFound);
        } else
        {
            throw new ArgumentException("No card of this type left in the hand.");
        }
        Notify(null);
    }

    public Card FindCardOfType(System.Type type)
    {
        return cards.Find(card => card.GetType() == type);
    } 

    public Dictionary<string, int> GetGroupedResources()
    {
        Dictionary<string, int> resourcesCount = new Dictionary<string, int>();

        foreach (Card card in cards)
        {
            if (card is ResourceCard)
            {
                ResourceCard rc = (ResourceCard)card;
                try
                {
                    resourcesCount.Add(rc.resource.Name, 1);
                }
                catch (ArgumentException e)
                {
                    resourcesCount[rc.resource.Name] += 1;
                }
            }
        }
        return resourcesCount;
    }

    
}

