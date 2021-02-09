using System;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    Color color;
    public List<Token> tokens;
    int fameMedalPieces;
    public SpaceShip ship;
    public Hand hand;
    TradingRules rules;
    AbstractFriendshipCard[] friendShipCards;
    int FriendShipChips;
    SFElement notifier;

    public Player(Color color, SFElement notifier)
    {
        this.color = color;
        fameMedalPieces = 0;
        ship = new SpaceShip();
        hand = new Hand();
        rules = new TradingRules();
        tokens = new List<Token> {};
        friendShipCards = new AbstractFriendshipCard[] {};
        FriendShipChips = 0;
        this.notifier = notifier;
    }

    void DataChanged()
    {
        notifier.app.Notify(SFNotification.player_data_changed, notifier, new object[] { this });
    }

    public void AddCard(Card card)
    {
        hand.AddCard(card);
        DataChanged();
    }

    public void AddToken(Token token)
    {
        tokens.Add(token);
        DataChanged();
    }

    public bool CanBuildToken(Token token)
    {
        return hand.CanPayCost(token.cost);
    }

    public void BuildToken(Token token, SpacePoint position = null)
    {
        hand.PayCost(token.cost);
        if (position != null)
        {
            token.position = position;
        }
        AddToken(token);
    }

    public void BuildUpgrade(Token token)
    {
        ship.Add(token);
        hand.PayCost(token.cost);
        DataChanged();
    }

    public int GetVictoryPoints()
    {
        int sum = 0;

        sum += GetVictoryPointsFromTokens();
        sum += GetVictoryPointsFromFameMedals();
        sum += GetVictoryPointsFromFriendships();

        return sum;
    }

    public int GetVictoryPointsFromTokens()
    {
        int sum = 0;
        foreach (Token tok in tokens)
        {
            sum += tok.GetVictoryPoints();
        }
        return sum;
    }

    public int GetVictoryPointsFromFameMedals()
    {
        return fameMedalPieces / 2;
    }

    public int GetVictoryPointsFromFriendships()
    {
        return 2 * FriendShipChips;
    }

    public void RemoveFameMedal()
    {
        if (fameMedalPieces > 0)
        {
            fameMedalPieces -= 1;
        }
        DataChanged();
    }

    public void AddFameMedal()
    {
        fameMedalPieces += 1;
        DataChanged();
    }

    public void AddFriendShipChip()
    {
        FriendShipChips += 1;
        DataChanged();
    }
}

public class TradingRules {

    //TODO

}



