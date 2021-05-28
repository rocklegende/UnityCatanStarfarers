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
        var row = Instantiate(tableRowPrefab, this.gameObject.transform, false);
        row.GetComponent<TradeOfferResponseView>().Init(player, TradeOfferAcceptedFromPlayer);
        row.transform.parent = this.gameObject.transform;
        _rows.Add(row.GetComponent<TradeOfferResponseView>());
    }

    public void RemoveAllRows()
    {
        foreach (var row in _rows)
        {
            Destroy(row.gameObject);
        }
        _rows = new List<TradeOfferResponseView>();
    }

    public TradeOfferResponseView FindRowWithPlayer(Player player)
    {
        return _rows.Find(row => row.player.name == player.name);
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
