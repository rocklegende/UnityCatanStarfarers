using System;
public class GameText
{
    public GameText()
    {
    }

    public static string EncounterYouWantToFlee()
    {
        return "A pirate attacks you, you want to flee?";
    }

    public static string EncounterSomeoneHasProblemsWithShip()
    {
        return "Another starfarer has a problem with his ship. You want to help him?";
    }

    public static string EncounterOtherStarfarerIsAttackedByPilot()
    {
        return "Another starfarer gets attacked by a pirate. You want to help him?";
    }

    public static string Encounter1For2Trade()
    {
        return "Starfarer offers a 1 for 2 trade for you, do you want to make that deal?";
    }

    public static string EncounterRobPlayersFor1Card()
    {
        return "Do you wanna give up a card to rob them?";
    }

    public static string EncounterWantToTryARaumsprung()
    {
        return "Do you want to try a raumsprung?";
    }

    public static string EncounterDiscardIfMoreThanLimitUpgrades(int limit)
    {
        return string.Format("All Players who have more than {0} upgrades, need to discard one", limit);
    }

}
