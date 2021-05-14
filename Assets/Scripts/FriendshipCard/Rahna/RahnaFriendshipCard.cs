﻿using System;
using UnityEngine;

[Serializable]
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

[Serializable]
public class RahnaDiscardLimitIncrease : RahnaFriendshipCard
{
    int newDiscardLimit = 12;
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.DiscardLimit = newDiscardLimit;
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Increaes the Discard limit to " + newDiscardLimit;
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class RahnaBuyFamemedals : RahnaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.AllowFameMedalBuy();
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Once per round you can trade one goods for one fame medal for one";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class RahnaNoIncomeBonus : RahnaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.receivesBonusOnNoPayout = true;
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "You can get one resource if you got no resources that round";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}


[Serializable]
public class RahnaRichHelpPoorBonus : RahnaFriendshipCard
{
    public override void ActivateEffect(Player targetPlayer)
    {
        targetPlayer.ActivateRichHelpPoorBonus();
    }

    public override Sprite GetEffectSprite()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "RICH HELP POOR";
    }

    public override string GetTitle()
    {
        throw new NotImplementedException();
    }
}