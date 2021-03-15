using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : SFController
{
    public GameObject HUD;
    public GameObject Map;
    public Map mapModel;

    public GameState state;
    public Player[] players;
    public Player mainPlayer;
    public int currentPlayerAtTurn = 0;
    public PayoutHandler payoutHandler;

    public EncounterCardHandler encounterCardHandler;

    // Start is called before the first frame update
    void Start()
    {
        DebugStartState debugState = new TestNormalDiceDebugState(this);
        debugState.Setup();
    }

    public void SetState(GameState state)
    {
        this.state = state;
        //TODO: this.state.Setup();
    }

    void AddPayoutToPlayer(Hand payout, Player player)
    {
        foreach (var card in payout.cards)
        {
            player.AddCard(card);
        }
    }

    public void PayoutPlayers(DiceThrow dt)
    {
        foreach (var player in players)
        {
            var payout = payoutHandler.GetPayoutForPlayer(player, dt);
            AddPayoutToPlayer(payout, player);
        }
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        switch (p_event_path)
        {
            case SFNotification.token_was_selected:
                state.OnTokenClicked((Token)p_data[0], (GameObject)p_data[1]);
                break;
            case SFNotification.spacepoint_selected:
                var point = (SpacePoint)p_data[0];
                var spacePointObject = (GameObject)p_data[1];
                point.print();
                state.OnSpacePointClicked(point, spacePointObject);
                break;

            case SFNotification.HUD_build_token_btn_clicked:
                state.OnBuildShipOptionClicked((Token)p_data[0]);
                break;

            case SFNotification.HUD_build_upgrade_btn_clicked:
                state.OnBuildUpgradeOptionClicked((Token)p_data[0]);
                break;

            case SFNotification.next_button_clicked:
                state.OnNextButtonClicked();
                break;

            case SFNotification.settle_button_clicked:
                state.OnSettleButtonPressed();
                break;

            case SFNotification.token_data_changed:
                if (mapModel != null)
                {
                    mapModel.OnTokenDataChanged((Token)p_data[0]);
                }
                break;

            case SFNotification.token_can_settle:
                state.OnTokenCanSettle((bool)p_data[0], (Token)p_data[1]);
                break;

        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
