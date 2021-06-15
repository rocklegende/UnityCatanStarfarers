using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallPlayerInfoView : SFController
{
    public Text playerName;
    public Text vp;
    public Text numResourceCards;
    public Text booster;
    public Text cannons;
    public Text freightPods;
    public Text coloniesLeft;
    public Text tradeShipsLeft;
    public Text spacePortsLeft;
    public Player player;

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        //switch (p_event_path)
        //{
        //    case SFNotification.player_data_changed:
        //        Draw();
        //        break;
        //}
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
        Draw();
    }

    public void OnPlayerDataChanged()
    {
        Draw();
    }

    public void Draw()
    {
        if (player != null)
        {
            playerName.text = player.name;
            vp.text = "VP: " + player.GetVictoryPoints().ToString();
            numResourceCards.text = "Cards: " + player.hand.Count();
        }
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
