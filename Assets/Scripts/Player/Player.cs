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
    List<AbstractFriendshipCard> friendShipCards;
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
        friendShipCards = new List<AbstractFriendshipCard>();
        FriendShipChips = 0;
        this.notifier = notifier;
    }

    void DataChanged()
    {
        notifier.app.Notify(SFNotification.player_data_changed, notifier, new object[] { this });
    }

    //public void ChangeTradingRatio

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

    public void AddFriendShipCard(AbstractFriendshipCard card)
    {
        friendShipCards.Add(card);
        DataChanged();
    }

    public void ChangeFoodTradingRatio(int newRatio)
    {
        if (newRatio < 2 || newRatio > 3)
        {
            throw new ArgumentException("");
        }
        rules.foodTradingRatio = newRatio;       
    }

    public void ChangeGoodsTradingRatio(int newRatio)
    {
        if (newRatio < 1 || newRatio > 2)
        {
            throw new ArgumentException("");
        }
        rules.goodsTradingRatio = newRatio;
    }

    public void ChangeFuelTradingRatio(int newRatio)
    {
        if (newRatio < 2 || newRatio > 3)
        {
            throw new ArgumentException("");
        }
        rules.fuelTradingRatio = newRatio;
    }

    public void ChangeCarbonTradingRatio(int newRatio)
    {
        if (newRatio < 2 || newRatio > 3)
        {
            throw new ArgumentException("");
        }
        rules.carbonTradingRatio = newRatio;
    }

    public void ChangeOreTradingRatio(int newRatio)
    {
        if (newRatio < 2 || newRatio > 3)
        {
            throw new ArgumentException("");
        }
        rules.oreTradingRatio = newRatio;
    }

    public int GetFoodTradingRatio()
    {
        return rules.foodTradingRatio;
    }

    public int GetGoodsTradingRatio()
    {
        return rules.goodsTradingRatio;
    }
    public int GetFuelTradingRatio()
    {
        return rules.fuelTradingRatio;
    }
    public int GetOreTradingRatio()
    {
        return rules.oreTradingRatio;
    }
    public int GetCarbonTradingRatio()
    {
        return rules.carbonTradingRatio;
    }
}

public class TradingRules {

    public int foodTradingRatio = 3;
    public int goodsTradingRatio = 2;
    public int oreTradingRatio = 3;
    public int fuelTradingRatio = 3;
    public int carbonTradingRatio = 3;

    public int productionBonusFood = 0;
    public int productionBonusFuel = 0;
    public int productionBonusGoods = 0;
    public int productionBonusOre = 0;
    public int productionBonusCarbon = 0;

    public void IncreaseProductionBonusGoods()
    {
        if (productionBonusGoods == 0)
        {
            productionBonusGoods += 1;
        }
    }

    public void IncreaseProductionBonusFood()
    {

    }

    public void IncreaseProductionBonusOre()
    {

    }

    public void IncreaseProductionBonusCarbon()
    {

    }

    public void IncreaseProductionBonusFuel()
    {

    }
}



