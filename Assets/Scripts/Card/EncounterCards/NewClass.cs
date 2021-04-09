using System;
public class FameMedalGainStrategy
{
    public static void HandleFameMedalGain(int gain, Player player)
    {
        player.AddFameMedals(gain);
    }
}
