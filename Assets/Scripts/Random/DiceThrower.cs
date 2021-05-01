using System;


public class DiceThrow {

    public readonly int firstDice;
    public readonly int secondDice;
    public DiceThrow(int firstDice, int secondDice)
    {
        this.firstDice = firstDice;
        this.secondDice = secondDice;
    }

    public int GetValue()
    {
        return firstDice + secondDice;
    }

}

public class DiceThrower
{
    private int maxDiceValue = 6;
    public DiceThrower()
    {
    }

    public DiceThrow throwDice()
    {
        int firstDice = ThreadSafeRandom.ThisThreadsRandom.Next(maxDiceValue) + 1;
        int secondDice = ThreadSafeRandom.ThisThreadsRandom.Next(maxDiceValue) + 1;
        return new DiceThrow(firstDice, secondDice);
    }
}
