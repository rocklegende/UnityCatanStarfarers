using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeOfferResponseView : MonoBehaviour
{
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

    public void AcceptButtonPressed()
    {
        callback(player, true);
    }

    public void DeclineButtonPressed()
    {
        callback(player, false);
    }

    void Update()
    {
        
    }
}
