using System;
using UnityEngine;

public abstract class RahnaFriendshipCard : AbstractFriendshipCard
{
    public RahnaFriendshipCard()
    {
    }

    public override string GetTradeStationName()
    {
        return "Rahna";
    }

    public override Sprite GetBackgroundSprite()
    {
        return new Helper().CreateSpriteFromImageName("rahna_trading_bg");
    }

}

public class RahnaBlaBla : RahnaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        throw new NotImplementedException();
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "You get to do some stupid shit";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}