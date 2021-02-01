using System;
using UnityEngine;

public class Player
{
    Color color;
    Token[] tokens;
    int fameMedalPieces;
    public SpaceShip ship;
    public Hand hand;
    TradingRules rules;
    AbstractFriendshipCard[] friendShipCards;
    int FriendShipChips;

    public Player(Color color)
    {
        this.color = color;
        fameMedalPieces = 0;
        ship = new SpaceShip();
        hand = new Hand();
        rules = new TradingRules();
        tokens = new Token[] {};
        friendShipCards = new AbstractFriendshipCard[] {};
        FriendShipChips = 0;
    }

    public void AddCard(Card card)
    {
        hand.AddCard(card);
    }

    public bool CanBuildToken(Token token)
    {
        return hand.CanPayCost(token.cost);
    }

    public void BuildToken(Token token)
    {
        hand.PayCost(token.cost);
    }

    public void BuildUpgrade(Token token)
    {
        ship.Add(token);
        hand.PayCost(token.cost); //TODO: could fail if not enough resources
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

    public void AddFameMedal()
    {
        fameMedalPieces += 1;
    }

    public void AddFriendShipChip()
    {
        FriendShipChips += 1;
    }
}

public class TradingRules {

    //TODO

}



