using System;
using System.Collections.Generic;

public abstract class TradeStation : TileGroup
{
    public string name;
    public AbstractFriendshipCard[] tradingCards;
    public List<Token> dockedSpaceships = new List<Token>();
    int capacity = 5;

    // TODO: add renderer script just for rendering trade stations (these scripts handle how to render 3 hex tiles)

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

    public override void OnTokenSettled(Token token)
    {
        dockedSpaceships.Add(token);
        //TODO: give token a new position inside the trade station

        // remove card from trading cards and give to owner of token
        //pass card to player

        //TODO: inform that tradestation data changed
    }


}
