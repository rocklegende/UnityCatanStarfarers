using System;
using UnityEngine;

public abstract class OrzelFriendshipCard : AbstractFriendshipCard
{
    public OrzelFriendshipCard()
    {
    }

    public override string GetTradeStationName()
    {
        return "Orzel";
    }
}

public class OrzelFriendshipCardFood : OrzelFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ChangeFoodTradingRatio(2);
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "2:1, food";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}