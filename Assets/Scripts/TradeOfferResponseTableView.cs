using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeOfferResponseTableView : MonoBehaviour
{
    public GameObject tableRowPrefab;
    public System.Action<Player> callback;
    private List<TradeOfferResponseView> _rows = new List<TradeOfferResponseView>();
    public List<TradeOfferResponseView> Rows { get { return _rows; } }

    public void AddRow(Player player)
    {
        var row = Instantiate(tableRowPrefab, this.transform, false);
        row.GetComponent<TradeOfferResponseView>().Init(player, TradeOfferAcceptedFromPlayer);
        row.transform.parent = transform.parent;
        _rows.Add(row.GetComponent<TradeOfferResponseView>());
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
