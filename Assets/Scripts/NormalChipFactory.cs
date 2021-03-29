using System;
public class NormalChipFactory
{
    public NormalChipFactory()
    {
    }

    public static NormalDiceChip Create2Chip()
    {
        return new NormalDiceChip(2);
    }

    public static NormalDiceChip Create3Chip()
    {
        return new NormalDiceChip(3);
    }

    public static NormalDiceChip Create4Chip()
    {
        return new NormalDiceChip(4);
    }

    public static NormalDiceChip Create5Chip()
    {
        return new NormalDiceChip(5);
    }

    public static NormalDiceChip Create6Chip()
    {
        return new NormalDiceChip(6);
    }

    public static NormalDiceChip Create8Chip()
    {
        return new NormalDiceChip(8);
    }

    public static NormalDiceChip Create9Chip()
    {
        return new NormalDiceChip(9);
    }

    public static NormalDiceChip Create10Chip()
    {
        return new NormalDiceChip(10);
    }

    public static NormalDiceChip Create11Chip()
    {
        return new NormalDiceChip(11);
    }

    public static NormalDiceChip Create12Chip()
    {
        return new NormalDiceChip(12);
    }

    public static NormalDiceChip Create3_12Chip()
    {
        return new NormalDiceChip(new System.Collections.Generic.List<int>() { 3, 12});
    }

    public static NormalDiceChip Create2_11Chip()
    {
        return new NormalDiceChip(new System.Collections.Generic.List<int>() { 2, 11 });
    }
}
