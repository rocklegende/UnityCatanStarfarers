using System;
using UnityEngine;

[Serializable]
public abstract class AbstractFriendshipCard
{
    public AbstractFriendshipCard()
    {
    }

    public abstract string GetTitle();
    public abstract string GetText();
    public abstract string GetTradeStationName();
    public abstract Sprite GetEffectSprite();
    public abstract Sprite GetBackgroundSprite();
    public abstract void ActivateEffect(Player targetPlayer);

}
