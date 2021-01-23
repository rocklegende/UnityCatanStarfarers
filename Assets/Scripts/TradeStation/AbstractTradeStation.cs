using System;
public abstract class AbstractTradeStation
{
    public string name;
    public AbstractFriendshipCard[] tradingCards;
    public SpaceShip[] dockedSpaceships;

    public AbstractTradeStation(AbstractFriendshipCard[] tradingCards, string name)
    {
        this.name = name;
        this.tradingCards = tradingCards;
    }

    public abstract string GetPictureName();

    
}
