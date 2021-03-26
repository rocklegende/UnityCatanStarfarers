using System;
using System.Collections;
using System.Collections.Generic;

public class ShipDiceThrow
{
    public readonly int value1;
    public readonly int value2;
    private int baseFlyValue = 3;

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

    /// <summary>
    /// Returns raw combined value of the balls. So black(0) and yellow(2) deliver a raw value of 2.
    /// </summary>
    /// <returns></returns>
    public int GetRawValue()
    {
        return value1 + value2;
    }

    /// <summary>
    /// Returns how much steps can be flown based on the values of the balls.
    /// If a black ball occurs the value will be the baseValue of this class.
    /// </summary>
    /// <returns></returns>
    public int GetFlyValue()
    {
        if (TriggersEncounterCard())
        {
            return baseFlyValue;
        } else
        {
            return GetRawValue();
        }
    }
}

public class ShipDiceThrower
{
    // 0 is black, standard according the rules is: 2x yellow (2 value), 1x red (3 value), 1x blue (1 value), 1x black (0 value)
    readonly List<int> values = new List<int> { 0, 1, 2, 2, 3 }; 
    public ShipDiceThrower()
    {
    }

    public ShipDiceThrow CastDice()
    {
        values.Shuffle();
        return new ShipDiceThrow(values[0], values[1]);
    }
}
