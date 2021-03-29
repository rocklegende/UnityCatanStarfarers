using System;
using UnityEngine;
using System.Collections.Generic;



public abstract class PirateTokenBeatCondition
{
    public PirateTokenBeatCondition()
    {
        
    }

    public abstract bool SpaceShipFullfillsCondition(SpaceShip ship);
    public abstract string GetTextureName();
}

public class FreightPodsBeatCondition : PirateTokenBeatCondition
{

    int minNumFreightPods;
    public FreightPodsBeatCondition(int minNumFreightPods) : base()
    {
        this.minNumFreightPods = minNumFreightPods;
    }

    public override bool SpaceShipFullfillsCondition(SpaceShip ship)
    {
        return ship.FreightPods >= minNumFreightPods;
    }

    public override string GetTextureName()
    {
        return String.Format("pirate_token_freightpod_{0}", minNumFreightPods);
    }
}

public class CannonBeatCondition : PirateTokenBeatCondition
{

    int minCannons;
    public CannonBeatCondition(int minCannons) : base()
    {
        this.minCannons = minCannons;
    }

    public override bool SpaceShipFullfillsCondition(SpaceShip ship)
    {
        return ship.Cannons >= minCannons;
    }

    public override string GetTextureName()
    {
        return String.Format("pirate_token_cannon_{0}", minCannons);
    }
}



public class PirateToken : DiceChip
{
    bool isBeaten = false;
    PirateTokenBeatCondition condition;

    public PirateToken(PirateTokenBeatCondition condition) : base(new List<int>() {})
    {
        this.condition = condition;
    }

    public void AttemptBeatingIt(SpaceShip spaceShip)
    {
        if (condition.SpaceShipFullfillsCondition(spaceShip))
        {
            isBeaten = true;
        }
    }

    public bool SpaceshipCanBeatIt(SpaceShip spaceShip)
    {
        return condition.SpaceShipFullfillsCondition(spaceShip);
    }

    public override string GetTextureName()
    {
        return condition.GetTextureName();
    }


}
