using System;
using UnityEngine;

public abstract class AbstractFriendshipCard
{
    public AbstractFriendshipCard()
    {
    }

    public abstract string GetTitle();
    public abstract string GetText();
    public abstract string GetTradeStationName();
    public abstract Sprite GetEffectSprite();
    public abstract void ActivateEffect(Player targetPlayer);

}
