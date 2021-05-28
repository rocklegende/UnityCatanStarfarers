using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeOfferResponseView : MonoBehaviour
{
    public GameObject AcceptButton;
    public GameObject Cross;
    public GameObject Spinner;
    private bool isAccepted = false;
    private bool isWaitingForResponse = true;

    public Text playernameText;
    System.Action<Player, bool> callback;
    public Player player;

    void Start()
    {
        
    }

    public void Init(Player player, System.Action<Player, bool> callback)
    {
        this.playernameText.text = player.name;
        this.player = player;
        this.callback = callback;
        
    }

    public void OfferWasDeclined()
    {
        isAccepted = false;
        isWaitingForResponse = false;
    }

    public void OfferWasAccepted()
    {
        isAccepted = true;
        isWaitingForResponse = false;

    }

    public void AcceptButtonPressed()
    {
        callback(player, true);
    }

    void Update()
    {
        if (isWaitingForResponse)
        {
            Cross.SetActive(false);
            AcceptButton.SetActive(false);
            Spinner.SetActive(true);
        } else
        {
            Spinner.SetActive(false);
            Cross.SetActive(!isAccepted);
            AcceptButton.SetActive(isAccepted);
        }
        
    }
}
