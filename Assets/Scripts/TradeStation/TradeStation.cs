﻿ using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
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

[Serializable]
public abstract class TradeStation : TileGroup
{
    public string name;
    public List<AbstractFriendshipCard> tradingCards;
    public List<Token> dockedTokens = new List<Token>();
    public TradeStationMedal medal;
    public bool isNotifying = true;
    int capacity = 5;

    public TradeStation(List<AbstractFriendshipCard> tradingCards, string name, Tile_[] tiles)
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
        Debug.Log("Token entered trade station area, do nothing at this point");
    }

    public override void SetCenter(SpacePoint center)
    {
        this.center = center;
        settlePoints = new List<SpacePoint>() { center };
    }

    public bool IsFull()
    {
        return dockedTokens.Count >= capacity;
    }

    public void RemoveCard(AbstractFriendshipCard card)
    {
        tradingCards.Remove(card);
    }

    public override void OnTokenSettled(Token token)
    {
        dockedTokens.Add(token);
        AssignOwnerOfMedal();

        if (isNotifying)
        {
            var notifier = new SFElement();
            notifier.app.Notify(SFNotification.open_friendship_card_selection, notifier, new object[] { this });
        }
    }

    void SetNewMedalOwner(Player player)
    {
        medal.owner = player;
        medal.owner.AddFriendShipChip();
    }

    Dictionary<Player, int> GetSpaceShipCountByPlayer()
    {
        var spaceShipCount = new Dictionary<Player, int>();
        foreach (var spaceShip in dockedTokens)
        {
            if (spaceShipCount.ContainsKey(spaceShip.owner))
            {
                spaceShipCount[spaceShip.owner] += 1;
            }
            else
            {
                spaceShipCount[spaceShip.owner] = 1;
            }
        }
        return spaceShipCount;
    }

    private void AssignOwnerOfMedal()
    {
        var spaceShipCount = GetSpaceShipCountByPlayer();
        var ordered = spaceShipCount.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        var highestPlayer = ordered.Keys.ElementAt(0);
        var numSpaceShipsHighestPlayer = ordered.Values.ElementAt(0);

        if (medal.owner == null)
        {
            SetNewMedalOwner(highestPlayer);
        }
        else
        {
            var numSpaceShipsOfCurrentMedalOwner = spaceShipCount[medal.owner];
            if (numSpaceShipsHighestPlayer > numSpaceShipsOfCurrentMedalOwner)
            {
                medal.owner.RemoveFriendShipChip(); //remove medal from prev owner
                SetNewMedalOwner(highestPlayer);
            }
        }
    }

    public override bool RequestSettleOfToken(Token token, SpacePoint futurePositionOfToken = null)
    {
        if (!(token is TradeBaseToken))
        {
            throw new WrongTokenTypeException();
        }

        if (futurePositionOfToken != null)
        {
            if (!SpacePointOnAtleastOneSettlePoint(futurePositionOfToken))
            {
                throw new NotOnSettleSpotException();
            }
        }
        else
        {
            if (!SpacePointOnAtleastOneSettlePoint(token.position))
            {
                throw new NotOnSettleSpotException();
            }
        }

        if (IsFull())
        {
            throw new TradeStationIsFullException();
        }

        if (token.owner.ship.FreightPods <= dockedTokens.Count)
        {
            throw new NotEnoughFreightPodsToDockException("Owner of token has " + token.owner.ship.FreightPods + " but " + dockedTokens.Count + " ships ar already docked");
        }

        return true;
    }


}
