﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeOfferResponseTableView : MonoBehaviour
{
    public GameObject tableRowPrefab;
    public System.Action<Player> callback;

    public void AddRow(Player player)
    {
        var row = Instantiate(tableRowPrefab, this.transform, false);
        row.GetComponent<TradeOfferResponseView>().Init(player, TradeOfferAcceptedFromPlayer);
        row.transform.parent = transform.parent;
    }

    void TradeOfferAcceptedFromPlayer(Player player, bool isAccepted)
    {
        if (isAccepted)
        {
            gameObject.SetActive(false);
            callback(player);
        }
    }
}
