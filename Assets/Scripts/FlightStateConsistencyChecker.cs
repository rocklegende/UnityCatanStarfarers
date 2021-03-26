using System;
using System.Collections.Generic;

public enum FlightStateInconsistency {
    UNSETTLED_TOKEN_ON_TRADEPOINT,
    UNSETTLED_TOKEN_ON_COLONYPOINT
}

public abstract class FlightStateInconsistencyError {
    protected Player player;
    protected Token token;
    public FlightStateInconsistencyError (Player player, Token token)
    {
        this.player = player;
        this.token = token;
    }

    public abstract string GetErrorMessage();
}

public class UnsettledTokenOnTradePointError : FlightStateInconsistencyError
{
    public UnsettledTokenOnTradePointError(Player player, Token token) : base(player, token)
    {

    }

    public override string GetErrorMessage()
    {
        return "Token cannot block trade point unless it settles";
    }
}

public class UnsettledTokenOnColonyPointError : FlightStateInconsistencyError
{
    public UnsettledTokenOnColonyPointError(Player player, Token token) : base(player, token)
    {

    }

    public override string GetErrorMessage()
    {
        return "Token cannot block colony point, please move it";
    }
}



public class FlightStateConsistencyChecker
{
    public FlightStateConsistencyChecker()
    {
    }

    public List<FlightStateInconsistencyError> Check(Map map, Player[] players)
    {
        var errors = new List<FlightStateInconsistencyError>();
        foreach (var player in players)
        {
            foreach(var token in player.tokens)
            {
                if (token.IsFlyable())
                {
                    // alle tradestations durchgehen
                    // map.getAllTradeStations()
                    // jede tradestation durchgehen und gucken ob token.position == center ist
                    // wenn ja, dann liegt hier ein Fehler vor
                    var allTradeStations = map.GetTradeStations();
                    var found = allTradeStations.Find(ts => ts.GetSettlePoints().Contains(token.position));
                    if (found != null)
                    {
                        errors.Add(new UnsettledTokenOnTradePointError(player, token));
                    }
                }
            }
        }
        return errors;
    }
}
