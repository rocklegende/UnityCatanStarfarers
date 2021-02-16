using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TradeStation : TileGroup
{
    public string name;
    public AbstractFriendshipCard[] tradingCards;
    public List<Token> dockedSpaceships = new List<Token>();
    int capacity = 5;

    public TradeStation(AbstractFriendshipCard[] tradingCards, string name, Tile_[] tiles)
        : base(tiles)
    {
        this.name = name;
        this.tradingCards = tradingCards;
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
        Debug.Log("");
        tradingCards = tradingCards.Where(i => i != card).ToArray();
        Debug.Log("");

    }

    public override void OnTokenSettled(Token token)
    {
        dockedSpaceships.Add(token);

        var notifier = new SFElement();
        notifier.app.Notify(SFNotification.open_friendship_card_selection, notifier, new object[] { this, tradingCards });

        // remove card from trading cards and give to owner of token
        //pass card to player

        //TODO: inform that tradestation data changed
    }


}
