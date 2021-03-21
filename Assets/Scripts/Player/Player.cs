using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class TokenStorage {

    List<Token> availableTokens;
    int numSpaceShips = 4;
    int numSpacePorts = 3;
    int numTrade = 4;
    int numColony = 6;

    public TokenStorage()
    {
        availableTokens = new List<Token>();
        for (int i = 0; i < numSpaceShips; i++)
        {
            availableTokens.Add(new ShipToken());
        }

        for (int i = 0; i < numSpacePorts; i++)
        {
            availableTokens.Add(new SpacePortToken());
        }

        for (int i = 0; i < numColony; i++)
        {
            availableTokens.Add(new ColonyBaseToken());
        }

        for (int i = 0; i < numTrade; i++)
        {
            availableTokens.Add(new TradeBaseToken());
        }
    }

    public Token RetrieveTokenOfType(Type type)
    {
        var tokens = availableTokens.Where(tok => tok.GetType() == type).ToArray();
        if (tokens.Length == 0)
        {
            throw new ArgumentException("Could not find token of that type");
        }
        var retrieved = tokens[0];
        availableTokens.Remove(retrieved);
        return retrieved;
    }

    public Token[] GetTokensOfType(Type type)
    {
        var tokens = availableTokens.Where(tok => tok.GetType() == type).ToArray();
        return tokens;
    }

    public void AddToken(Token token)
    {
        if (token.attachedToken != null)
        {
            // add tokens seperately if there is an attached token
            availableTokens.Add(token.attachedToken);
            token.detachToken();
            availableTokens.Add(token);
        } else
        {
            availableTokens.Add(token);
        }
    }
}

public class Player
{
    public Color color;
    public List<Token> tokens; // tokens on gameboard
    public TokenStorage tokenStorage;
    int fameMedalPieces;
    public SpaceShip ship;
    public Hand hand;
    TradingRules rules;
    List<AbstractFriendshipCard> friendShipCards;
    int FriendShipChips;
    SFElement notifier;
    int foodProduceBonus;
    int goodsProduceBonus;
    int oreProduceBonus;
    int carbonProduceBonus;
    int fuelProduceBonus;
    int pirateTokenBeatenAwards = 0;
    int twoCardsBonusThreshold = 8;
    int oneCardBonusThreshold = 12;

    public Player(Color color, SFElement notifier)
    {
        this.color = color;
        fameMedalPieces = 0;
        ship = new SpaceShip();
        hand = new Hand(DataChanged);
        rules = new TradingRules();
        tokens = new List<Token> {};
        tokenStorage = new TokenStorage();
        friendShipCards = new List<AbstractFriendshipCard>();
        FriendShipChips = 0;
        this.notifier = notifier;
    }

    void DataChanged()
    {
        notifier.app.Notify(SFNotification.player_data_changed, notifier, new object[] { this });
    }

    public void AddRangeToFlyableTokens(int range)
    {
        if (range < 0)
        {
            throw new ArgumentException("range cant be negative");
        } else
        {
            foreach(var flyToken in GetFlyableTokens())
            {
                flyToken.addSteps(range);
            }
        }
    }

    Token[] GetFlyableTokens()
    {
        var list = new List<Token>();
        foreach (var token in tokens)
        {
            if (token.IsFlyable())
            {
                list.Add(token);
            }
        }
        return list.ToArray();
    }

    public int GetBonusForResource(Resource resource)
    {
        if (resource is CarbonResource)
        {
            return carbonProduceBonus;
        }
        else if (resource is FuelResource)
        {
            return fuelProduceBonus;
        }
        else if (resource is FoodResource)
        {
            return foodProduceBonus;
        }
        else if (resource is GoodsResource)
        {
            return goodsProduceBonus;
        }
        else if (resource is OreResource)
        {
            return oreProduceBonus;
        }
        else
        {
            throw new ArgumentException("No resource of that type available in the game.");
        }
    }

    public int GetFameMedalPieces()
    {
        return fameMedalPieces;
    }

    public void AddCard(Card card)
    {
        hand.AddCard(card);
        DataChanged();
    }

    public void AddHand(Hand h)
    {
        hand.AddHand(h);
        DataChanged(); //TODO maybe let hand decide when it changed and react on that
    }

    public void SubtractHand(Hand h)
    {
        hand.SubtractHand(h);
        DataChanged();
    }

    public void AddToken(Token token)
    {
        tokens.Add(token);
        token.owner = this;
        DataChanged();
    }

    public bool CanBuildToken(Token token)
    {
        //token.CanBeBuild()
        return hand.CanPayCost(token.cost);
    }

    /// <summary>
    /// DEPRECATED METHOD, PLEASE USE BuildToken2
    /// </summary>
    /// <param name="token"></param>
    /// <param name="position"></param>
    public void BuildToken(Token token, SpacePoint position = null)
    {
        // deprecated
        var tokenFromStorage = tokenStorage.RetrieveTokenOfType(token.GetType());
        hand.PayCost(tokenFromStorage.cost);
        if (position != null)
        {
            tokenFromStorage.position = position;
        }
        tokenFromStorage.SetColor(color);
        AddToken(tokenFromStorage);
    }

    public Token BuildToken2(Type baseType, SpacePoint position, Type attachedType = null, bool isFree = false)
    {
        var baseTokenFromStorage = tokenStorage.RetrieveTokenOfType(baseType);
        if (!isFree)
        {
            hand.PayCost(baseTokenFromStorage.cost);
        }
        if (position != null)
        {
            baseTokenFromStorage.position = position;
        }
        if (attachedType != null)
        {
            var attachedTokenFromStorage = tokenStorage.RetrieveTokenOfType(attachedType);
            baseTokenFromStorage.attachToken(attachedTokenFromStorage);
        }

        baseTokenFromStorage.SetColor(color);
        AddToken(baseTokenFromStorage);
        return baseTokenFromStorage;
    }

    public void BuildTokenWithoutCost(Type baseType, SpacePoint position, Type attachedType = null)
    {
        BuildToken2(baseType, position, attachedType, true);
    }

    public void BuildUpgrade(Token token)
    {
        ship.Add(token);
        hand.PayCost(token.cost);
        DataChanged();
    }

    public void BuildUpgradeWithoutCost(Token token)
    {
        ship.Add(token);
        DataChanged();
    }

    public int GetVictoryPoints()
    {
        int sum = 0;

        sum += GetVictoryPointsFromTokens();
        sum += GetVictoryPointsFromFameMedals();
        sum += GetVictoryPointsFromFriendships();
        sum += GetVictoryPointsFromPirateTokensBeaten();

        return sum;
    }

    public int GetVictoryPointsFromPirateTokensBeaten()
    {
        return pirateTokenBeatenAwards;
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

    /// <summary>
    /// Returns the number of cards this player receives upon a new turn.
    /// </summary>
    /// <returns></returns>
    public int LowPointsBonus()
    {
        var vp = GetVictoryPoints();
        if (vp < twoCardsBonusThreshold)
        {
            return 2;
        } else if (vp >= twoCardsBonusThreshold && vp < oneCardBonusThreshold){
            return 1;
        } else
        {
            return 0;
        }
    }

    public void AddPirateTokenBeatenAward()
    {
        pirateTokenBeatenAwards += 1;
        DataChanged();
    }

    public void AddFameMedals(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Negative values not allowed");
        }
        fameMedalPieces += amount;
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

    public void IncreaseFoodBonus(int n)
    {
        int newValue = foodProduceBonus + n;
        if (newValue < 2 && newValue >= 0)
        {
            foodProduceBonus += n;
        }
    }

    public void IncreaseGoodsBonus(int n)
    {
        int newValue = goodsProduceBonus + n;
        if (newValue < 2 && newValue >= 0)
        {
            goodsProduceBonus += n;
        }
    }

    public void IncreaseOreBonus(int n)
    {
        int newValue = oreProduceBonus + n;
        if (newValue < 2 && newValue >= 0)
        {
            oreProduceBonus += n;
        }
    }

    public void IncreaseCarbonBonus(int n)
    {
        int newValue = carbonProduceBonus + n;
        if (newValue < 2 && newValue >= 0)
        {
            carbonProduceBonus += n;
        }
    }

    public void IncreaseFuelBonus(int n)
    {
        int newValue = fuelProduceBonus + n;
        if (newValue < 2 && newValue >= 0)
        {
            fuelProduceBonus += n;
        }
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



