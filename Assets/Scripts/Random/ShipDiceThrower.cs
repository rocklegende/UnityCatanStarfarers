using System;
using System.Collections;
using System.Collections.Generic;

public class ShipDiceThrow
{
    public readonly int value1;
    public readonly int value2;
    public ShipDiceThrow(int value1, int value2)
    {
        if (value1 < 0 || value2 < 0)
        {
            throw new ArgumentException("value1 or value2 is < 0, this is not allowed.");
        }
        this.value1 = value1;
        this.value2 = value2;
    }

    public bool TriggersEncounterCard()
    {
        return value1 == 0 || value2 == 0;
    }

    public int GetValue()
    {
        return value1 + value2;
    }
}

public class ShipDiceThrower
{
    readonly List<int> values = new List<int> { 0, 1, 1, 2, 3 }; // 0 is black
    public ShipDiceThrower()
    {
    }

    public ShipDiceThrow CastDice()
    {
        values.Shuffle();
        return new ShipDiceThrow(values[0], values[1]);
    }
}
