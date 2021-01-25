using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Hand
{

    public List<Card> cards;

    public void AddCard(Card card)
    {
        cards.Add(card);
        //TODO: notify that hand changed
    }

    public Hand()
    {
    }
}
