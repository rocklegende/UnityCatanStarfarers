using System;
using System.Collections.Generic;

[Serializable]
public class NormalDiceChip : DiceChip
{
    public NormalDiceChip(List<int> values) : base(values)
    {
    }

    public NormalDiceChip(int value) : base(new List<int>() { value })
    {
    }

    public override string GetTextureName()
    {
        throw new NotImplementedException();
    }
}
