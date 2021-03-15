using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FightCategory
{
    CANNON,
    FREIGHTPODS,
    BOOSTER
}

public class FightPanelScript : MonoBehaviour
{

    public GameObject OriginPlayerDiceThrowRenderer;
    public GameObject OpponentPlayerDiceThrowRenderer;
    public Text opponentText;
    public Text originPlayerText;
    public Text title;
    public Text resultText;

    

    public Action<bool> fightIsDoneCallback;

    public void SetFightIsDoneCallback(Action<bool> callback)
    {
        this.fightIsDoneCallback = callback;
    }

    public void PlayFight(Player origin, Player opponent, FightCategory fightCategory)
    {
        StartCoroutine(PlayFightCoroutine(origin, opponent, fightCategory));
    }

    /// <summary>
    /// Plays fight with animation. Calls the fightIsDoneCallback with value true if the origin player wins, false otherwise.
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="opponent"></param>
    /// <param name="fightCategory"></param>
    /// <returns></returns>

    IEnumerator PlayFightCoroutine(Player origin, Player opponent, FightCategory fightCategory)
    {
        opponentText.text = opponent.color.ToString();
        originPlayerText.text = origin.color.ToString();
        yield return new WaitForSeconds(1);
        var originDiceThrow = OriginPlayerDiceThrowRenderer.GetComponent<ShipDiceThrowRenderer>().Cast();
        var opponentDiceThrow = OpponentPlayerDiceThrowRenderer.GetComponent<ShipDiceThrowRenderer>().Cast();
        yield return new WaitForSeconds(2);

        var originPlayerWon = false;
        switch (fightCategory)
        {
            case FightCategory.CANNON:
                title.text = "Cannon fight";
                originPlayerWon = originDiceThrow.GetValue() + origin.ship.Cannons >= opponentDiceThrow.GetValue() + opponent.ship.Cannons;
                break;
            case FightCategory.FREIGHTPODS:
                title.text = "Freightpods fight";
                originPlayerWon = originDiceThrow.GetValue() + origin.ship.FreightPods >= opponentDiceThrow.GetValue() + opponent.ship.FreightPods;
                break;
            case FightCategory.BOOSTER:
                title.text = "Booster fight";
                originPlayerWon = originDiceThrow.GetValue() + origin.ship.Boosters >= opponentDiceThrow.GetValue() + opponent.ship.Boosters;
                break;
        }

        if (originPlayerWon)
        {
            resultText.color = Color.green;
            resultText.text = "You won!";
        }
        else
        {
            resultText.color = Color.red;
            resultText.text = "You lost!";
        }

        yield return new WaitForSeconds(2);

        fightIsDoneCallback(originPlayerWon);

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
