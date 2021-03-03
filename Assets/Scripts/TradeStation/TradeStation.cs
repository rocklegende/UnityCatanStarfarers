 using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TradeStationMedal
{
    public TradeStation ts;
    public Player owner;
    public TradeStationMedal(TradeStation ts)
    {
        this.ts = ts;

    }

    public void SetOwner(Player player)
    {
        this.owner = player;
    }
}

public abstract class TradeStation : TileGroup
{
    public string name;
    public AbstractFriendshipCard[] tradingCards;
    public List<Token> dockedSpaceships = new List<Token>();
    public TradeStationMedal medal;
    int capacity = 5;

    public TradeStation(AbstractFriendshipCard[] tradingCards, string name, Tile_[] tiles)
        : base(tiles)
    {
        this.name = name;
        this.tradingCards = tradingCards;
        this.medal = new TradeStationMedal(this);
    }

    public abstract string GetPictureName();

    public int GetCapacity()
    {
        return capacity;
    }

    public int GetCapacityPerTile()
    {
        return GetCapacity() / GetTiles().Length;
    }   

    public override void OnTokenEnteredArea(Token token)
    {
        // do nothing
    }

    public void RemoveCard(AbstractFriendshipCard card)
    {
        tradingCards = tradingCards.Where(i => i != card).ToArray();
    }

    public override void OnTokenSettled(Token token)
    {
        dockedSpaceships.Add(token);
        AssignOwnerOfMedal();

        var notifier = new SFElement();
        notifier.app.Notify(SFNotification.open_friendship_card_selection, notifier, new object[] { this, tradingCards });

        // remove card from trading cards and give to owner of token
        //pass card to player

        //TODO: inform that tradestation data changed
    }

    private void AssignOwnerOfMedal()
    {
        // get the player with the highest spaceships docked and the number of spaceships he has
        var spaceShipCount = new Dictionary<Player, int>();
        foreach (var spaceShip in dockedSpaceships)
        {
            if (spaceShipCount.ContainsKey(spaceShip.owner))
            {
                spaceShipCount[spaceShip.owner] += 1;
            } else
            {
                spaceShipCount[spaceShip.owner] = 1;
            }
        }

        var ordered = spaceShipCount.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        var highestPlayer = ordered.Keys.ElementAt(0);
        var numSpaceShipsHighestPlayer = ordered.Values.ElementAt(0);

        if (medal.owner == null)
        {
            medal.owner = highestPlayer;
        } else
        {
            var numSpaceShipsOfCurrentOwner = spaceShipCount[medal.owner];
            if (numSpaceShipsHighestPlayer > numSpaceShipsOfCurrentOwner)
            {
                medal.owner = highestPlayer;
            }
        }
        Debug.Log("Jo");

        
    }


}
