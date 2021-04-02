﻿using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Hand : Subject
{

    public List<Card> cards = new List<Card>();

    public void AddCard(Card card)
    {
        cards.Add(card);
        Notify();
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
        Notify();
    }

    public int Count()
    {
        return cards.Count;
    }

    public Hand()
    {
        
    }

    public string GetStringRepresentation()
    {
        var converted = cards.Select(card => card.GetType().Name).ToArray();
        return String.Join(", ", converted);
    }

    public Hand GetRandomHand(int numCards)
    {
        if (numCards > Count())
        {
            throw new ArgumentException("Not enough cards left for that operation");
        }

        var returnHand = new Hand();
        cards.Shuffle();
        for (int i = 0; i < numCards; i++)
        {
            var card = cards[i];
            returnHand.AddCard(card);
        }
        return returnHand;
    }

    public void SubtractHand(Hand h)
    {
        foreach(var card in h.cards)
        {
            RemoveCardOfType(card.GetType());
        }
        Notify();
    }

    public void RemoveAllCards()
    {
        cards = new List<Card>();
        Notify();
    }

    public void AddHand(Hand hand)
    {
        cards.AddRange(hand.cards);
        Notify();
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
        Notify();
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
        Notify();
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

