using System;
using UnityEngine;


public class OrzelTradeStation : AbstractTradeStation
{
    public OrzelTradeStation() : base(new AbstractFriendshipCard[] { }, "Orzel")
    {
    }

    public override string GetPictureName()
    {
        return "OrzelStation";
    }
}
