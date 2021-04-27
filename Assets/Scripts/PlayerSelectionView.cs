using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionView : MonoBehaviour
{

    public Button selectButton;
    public GameObject playerHorizontalGroup;
    public GameObject playerBoxPrefab;
    public System.Action<List<Player>> selectCallback;
    public List<PlayerSelectionBox> selectionBoxes = new List<PlayerSelectionBox>();

    public bool multiselect = true;
    public int multiSelectMaxSelectable = 2;

    public List<GameObject> playerBoxObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ClearPreviousBoxes()
    {
        selectionBoxes = new List<PlayerSelectionBox>();
        foreach (var bla in playerBoxObjects)
        {
            Destroy(bla);
        }
    }

    public void SetSelectablePlayers(List<Player> players)
    {

        ClearPreviousBoxes();
        foreach (var player in players)
        {
            var playerBox = Instantiate(playerBoxPrefab, playerHorizontalGroup.transform, false);
            playerBox.GetComponent<PlayerSelectionBox>().SetPlayer(player);
            playerBox.GetComponent<PlayerSelectionBox>().itemSelectedCallback = PlayerBoxWasSelected;
            selectionBoxes.Add(playerBox.GetComponent<PlayerSelectionBox>());
            playerBox.transform.parent = playerHorizontalGroup.transform;
            playerBoxObjects.Add(playerBox);
        }
    }

    List<PlayerSelectionBox> GetSelectedBoxes()
    {
        return selectionBoxes.Where(box => box.isSelected).ToList();
    }

    void PlayerBoxWasSelected(PlayerSelectionBox box)
    {
        if (multiselect)
        {
            if (!box.isSelected)
            {
                if (GetSelectedBoxes().Count < multiSelectMaxSelectable)
                {
                    box.SetSelected(true);
                }
            } else
            {
                box.SetSelected(false);
            }
            
        } else
        {
            foreach (var b in GetSelectedBoxes())
            {
                b.SetSelected(false);
            }
            box.SetSelected(true);
        }
    }

    public void SelectButtonPressed()
    {
        var players = GetSelectedPlayers();
        selectCallback(players);
    }

    List<Player> GetSelectedPlayers()
    {
        return selectionBoxes.Where(box => box.isSelected).Select(selectedBox => selectedBox.GetPlayer()).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
