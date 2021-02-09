using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TokenFilter
{
    public abstract bool fullfillsFilter(Token token);

}

public class IsSettledColonyFilter : TokenFilter
{
    public override bool fullfillsFilter(Token token)
    {
        if (token is ColonyBaseToken)
        {
            ColonyBaseToken cb = (ColonyBaseToken)token;
            return cb.IsSettled();
        }
        return false;
    }
}