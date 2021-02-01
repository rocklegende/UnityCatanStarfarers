using System;
using UnityEngine;

public class SpaceShip
{
    public int Cannons = 0;
    public int FreightPods = 0;
    public int Boosters = 0;
    public int currentRolledBalls;

    public SpaceShip()
    {
    }

    public void Add(Token token)
    {
        if (token is BoosterUpgradeToken)
        {
            Boosters += 1;
        } else if (token is CannonUpgradeToken)
        {
            Cannons += 1;
        } else if (token is FreightPodUpgradeToken)
        {
            FreightPods += 1;
        } else
        {
            
        }
    }

    public void Remove(Token token)
    {

    }
}
