using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using com.onebuckgames.UnityStarFarers;

public enum CardType
{
    CARBON,
    FUEL,
    GOODS,
    FOOD,
    ORE
}

[Serializable]
public class TokenStorage {

    List<Token> availableTokens;
    int numSpaceShips = 3;
    int numSpacePorts = 3;
    int numTrade = 7;
    int numColony = 9;

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

public interface PlayerComparer
{
    bool comparePlayerStates(Player state1, Player state2);
}

public class LostOneFameMedalComparer : PlayerComparer
{
    public bool comparePlayerStates(Player state1, Player state2)
    {
        return state1.FameMedalPieces - 1 == state2.FameMedalPieces;
    }
}

public class WonOneFameMedalComparer : PlayerComparer
{
    public bool comparePlayerStates(Player state1, Player state2)
    {
        return state1.FameMedalPieces + 1 == state2.FameMedalPieces;
    }
}

[Serializable]
public class Player : SFModel, Observer, IComparable
{
    public readonly string guid;
    public SFColor color;
    public List<Token> tokens; // tokens on gameboard
    public List<Token> giftedTokens = new List<Token>();
    public TokenStorage tokenStorage;
    public string name = "DefaultPlayerName";
    public bool receivesBonusOnNoPayout = false;
    public bool fameMedalBuyPossible = false;
    public SpaceShip ship;
    public Hand hand;
    public bool hasRichHelpPoorBonus = false;
    public bool richHelpPoorBonusMadeThisRound = false;
    /// <summary>
    /// Position in the order of turns. 0 means this is the first player in the order.
    /// </summary>
    public int TurnOrderPosition;

    bool fameMedalBuyMadeThisRound = false;

    int fameMedalPieces;
    public int FameMedalPieces { get { return fameMedalPieces; } }

    TradingRules rules;
    public TradingRules Rules { get { return rules; } }

    List<AbstractFriendshipCard> friendShipCards;
    public List<AbstractFriendshipCard> FriendShipCards { get { return friendShipCards; } }

    int FriendShipChips;
    int foodProduceBonus;
    int goodsProduceBonus;
    int oreProduceBonus;
    int carbonProduceBonus;
    int fuelProduceBonus;
    int pirateTokenBeatenAwards = 0;
    int twoCardsBonusThreshold = 7;
    int oneCardBonusThreshold = 9;

    int discardLimit = 7;
    /// <summary>
    /// Get the maximum amount of cards that player can have before he needs to discard if a 7 is rolled.
    /// </summary>
    /// <returns></returns>
    public int DiscardLimit {
        get { return discardLimit; }
        set { DataChanged(); discardLimit = value; }
    }


    public Player(SFColor color)
    {
        this.color = color;
        fameMedalPieces = 0;
        ship = new SpaceShip(DataChanged);
        hand = new Hand();
        rules = new TradingRules();
        tokens = new List<Token> {};
        tokenStorage = new TokenStorage();
        friendShipCards = new List<AbstractFriendshipCard>();
        FriendShipChips = 0;
        guid = Guid.NewGuid().ToString();
    }

    public void UpdateData(Player newPlayerData)
    {
        color = newPlayerData.color;
        tokens = newPlayerData.tokens; // tokens on gameboard
        giftedTokens = newPlayerData.giftedTokens;
        tokenStorage = newPlayerData.tokenStorage;
        name = newPlayerData.name;
        receivesBonusOnNoPayout = newPlayerData.receivesBonusOnNoPayout;
        fameMedalBuyPossible = newPlayerData.fameMedalBuyPossible;
        ship = newPlayerData.ship;
        hand = newPlayerData.hand;
        hasRichHelpPoorBonus = newPlayerData.hasRichHelpPoorBonus;
        richHelpPoorBonusMadeThisRound = newPlayerData.richHelpPoorBonusMadeThisRound;
        TurnOrderPosition = newPlayerData.TurnOrderPosition;
        //DataChanged();

    //    bool fameMedalBuyMadeThisRound = false { get; }
    //    public int fameMedalPieces { get; }
    //TradingRules rules;
    //List<AbstractFriendshipCard> friendShipCards;
    //int FriendShipChips;
    //int foodProduceBonus;
    //int goodsProduceBonus;
    //int oreProduceBonus;
    //int carbonProduceBonus;
    //int fuelProduceBonus;
    //int pirateTokenBeatenAwards = 0;
    //int twoCardsBonusThreshold = 7;
    //int oneCardBonusThreshold = 9;
    //int discardLimit = 7;
}

public Player SimpleClone()
    {
        //TODO: make real copy of everything here
        var player = new Player(this.color);
        player.foodProduceBonus = foodProduceBonus;
        player.goodsProduceBonus = goodsProduceBonus;
        player.oreProduceBonus = oreProduceBonus;
        player.fameMedalPieces = fameMedalPieces;
        return player;
    }

    /// <summary>
    /// Returns true if the player has more cards than the allowed discard limit (used on 7 Roll)
    /// </summary>
    /// <returns></returns>
    public bool ExceedsDiscardLimit()
    {
        return hand.Count() > DiscardLimit;
    }

    public void ActivateRichHelpPoorBonus()
    {
        hasRichHelpPoorBonus = true;
        DataChanged();
    }

    public void RichHelpPoorMoveMade()
    {
        richHelpPoorBonusMadeThisRound = true;
        DataChanged();
    }

    public void PayToOtherPlayer(Player other, Hand hand)
    {
        SubtractHand(hand);
        other.AddHand(hand);
    }

    void DataChanged()
    {
        Notify(this);
    }

    public void OnTurnReceived()
    {
        ResetOneTimeActions();
        ResetTokens();
        DataChanged();
    }

    void ResetTokens()
    {
        foreach(var tok in tokens)
        {
            tok.HandleNewTurn();
        }
    }

    void ResetOneTimeActions()
    {
        fameMedalBuyMadeThisRound = false;
        richHelpPoorBonusMadeThisRound = false;
    }

    public void AllowFameMedalBuy()
    {
        fameMedalBuyPossible = true;
        DataChanged();
    }

    public bool CanBuyFameMedal()
    {
        return !fameMedalBuyMadeThisRound && fameMedalBuyPossible && hand.NumberCardsOfType<GoodsCard>() > 0;
    }

    public void BuyFameMedal()
    {
        if (!CanBuyFameMedal()) {
            throw new ArgumentException("Fame medal buy is not allowed!");
        } else
        {
            fameMedalBuyMadeThisRound = true;
            hand.PayCost(new Cost(new Resource[] { new GoodsResource() }));
            AddFameMedal();
        }
    }

    ///// <summary>
    ///// Get the maximum amount of cards that player can have before he needs to discard if a 7 is rolled.
    ///// </summary>
    ///// <returns></returns>
    //public int GetDiscardLimit()
    //{
    //    return discardLimit;
    //}

    //public void SetDiscardLimit(int limit)
    //{
    //    discardLimit = limit;
    //    DataChanged();
    //}

    public void AddRangeToFlyableTokens(int range)
    {
        if (range < 0)
        {
            throw new ArgumentException("range cant be negative");
        } else
        {
            foreach(var flyToken in tokens.Where(tok => tok.HasShipTokenOnTop()).ToList())
            {
                flyToken.addSteps(range);
            }
        }
    }

    public List<Token> GetTokensThatCanFly()
    {
        return tokens.Where(tok => tok.CanFly()).ToList();
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

    public void AddCard(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.CARBON:
                AddCard(new CarbonCard());
                break;
            case CardType.GOODS:
                AddCard(new GoodsCard());
                break;
            case CardType.FUEL:
                AddCard(new FuelCard());
                break;
            case CardType.ORE:
                AddCard(new OreCard());
                break;
            case CardType.FOOD:
                AddCard(new FoodCard());
                break;
        }
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
        token.RegisterObserver(this);
        DataChanged();
    }

    public void AddGiftedToken(Token token)
    {
        giftedTokens.Add(token);
        DataChanged();
    }

    public Token BuildToken(Map map, Type baseType, SpacePoint position, Type attachedType = null, bool isFree = false)
    {
        var baseTokenFromStorage = tokenStorage.RetrieveTokenOfType(baseType);
        var giftedToken = giftedTokens.Find(tok => tok.GetType() == baseType);
        var isGifted = giftedToken != null;

        if (isGifted)
        {
            giftedTokens.Remove(giftedToken);
        } else
        {
            if (!isFree)
            {
                hand.PayCost(baseTokenFromStorage.cost);
            }
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
        map.AddToken(baseTokenFromStorage);
        AddToken(baseTokenFromStorage);

        return baseTokenFromStorage;
    }

    public Token BuildColonyShipForFree(Map map, SpacePoint position)
    {
        return BuildColonyShip(
            map, position, true
        );
    }

    //TODO: add test for this method, just test if we get the desired colony at that position, check for type and position
    public Token BuildColonyShip(Map map, SpacePoint position, bool isFree = false)
    {
        return BuildToken(
            map,
            new ColonyBaseToken().GetType(),
            position,
            new ShipToken().GetType(),
            isFree
        );
    }

    public Token BuildTradeShipForFree(Map map, SpacePoint position)
    {
        return BuildTradeShip(
            map, position, true
        );
    }

    //TODO: add test for this method
    public Token BuildTradeShip(Map map, SpacePoint position, bool isFree = false)
    {
        return BuildToken(
            map,
            new TradeBaseToken().GetType(),
            position,
            new ShipToken().GetType(),
            isFree
        );
    }

    //public Token BuildSpacePort(SpacePoint position, bool isFree = false)
    //{

    //}

    public Token BuildTokenWithoutCost(Map map, Type baseType, SpacePoint position, Type attachedType = null)
    {
        
        return BuildToken(map, baseType, position, attachedType, true);
    }

    public void BuildUpgrade(Upgrade upgrade, bool isForFree = false)
    {
        ship.Add(upgrade);
        if (!isForFree)
        {
            hand.PayCost(upgrade.cost);
        }
        DataChanged();
    }

    public void RemoveUpgrade(Upgrade upgrade)
    {
        ship.Remove(upgrade);
        DataChanged();
    }

    public void BuildUpgradeWithoutCost(Upgrade upgrade)
    {
        BuildUpgrade(upgrade, true);
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
        if (vp <= twoCardsBonusThreshold)
        {
            return 2;
        } else if (vp > twoCardsBonusThreshold && vp <= oneCardBonusThreshold){
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
        fameMedalPieces += amount;
        if (fameMedalPieces < 0)
        {
            fameMedalPieces = 0;
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

    public void RemoveFriendShipChip()
    {
        if (FriendShipChips == 0)
        {
            throw new ArgumentException("Cant remove friendshipchip. Player has none.");
        }
        FriendShipChips -= 1;
        DataChanged();
    }

    public void AddFriendShipCard(AbstractFriendshipCard card)
    {
        friendShipCards.Add(card);
        card.ActivateEffect(this);
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

    public void SubjectDataChanged(Subject subject, object[] data)
    {
        //token data changed, delegate this change up
        DataChanged();
    }

    public int CompareTo(object obj)
    {
        var other = (Player) obj;
        return other.name.CompareTo(name);
    }
}

[Serializable]
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



