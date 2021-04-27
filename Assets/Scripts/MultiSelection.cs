using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSelection : SFView
{

    public Button selectButton;
    public GameObject optionsHorizontalStack;
    public GameObject selectionBoxPrefab;
    public System.Action<List<int>> selectCallback;
    public List<SelectionBox> selectionBoxes = new List<SelectionBox>();
    public int maxSelectable = 2;

    public List<GameObject> selectionBoxObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //TODO: this line is only to debug, remove in production
        //SetSelectableObjects(new Token[] { new BoosterUpgradeToken(), new FreightPodUpgradeToken() });
    }

    bool IsSingleSelect()
    {
        return maxSelectable == 1;
    }

    void ClearPreviousBoxes()
    {
        selectionBoxes = new List<SelectionBox>();
        foreach (var bla in selectionBoxObjects)
        {
            Destroy(bla);
        }
    }

    public void SetSelectableObjects(SFModel[] objects)
    {
        
        ClearPreviousBoxes();
        for (int i = 0; i < objects.Length; i++)
        {
            var obj = objects[i];
            var playerBox = Instantiate(selectionBoxPrefab, optionsHorizontalStack.transform, false);
            playerBox.GetComponent<SelectionBox>().Init(obj);
            playerBox.GetComponent<SelectionBox>().selectionIndex = i;
            playerBox.GetComponent<SelectionBox>().itemSelectedCallback = BoxWasSelected;
            selectionBoxes.Add(playerBox.GetComponent<SelectionBox>());
            playerBox.transform.parent = optionsHorizontalStack.transform;
            selectionBoxObjects.Add(playerBox);
        }
    }

    List<SelectionBox> GetSelectedBoxes()
    {
        return selectionBoxes.Where(box => box.isSelected).ToList();
    }

    void BoxWasSelected(SelectionBox box)
    {
        if (IsSingleSelect())
        {
            BoxWasSelectedInSingleSelectMode(box);
        }
        else
        {
            BoxWasSelectedInMultiSelectMode(box);
        }
    }

    void BoxWasSelectedInSingleSelectMode(SelectionBox box)
    {
        foreach (var b in GetSelectedBoxes())
        {
            b.SetSelected(false);
        }
        box.SetSelected(true);
    }

    void BoxWasSelectedInMultiSelectMode(SelectionBox box)
    {
        if (!box.isSelected)
        {
            if (GetSelectedBoxes().Count < maxSelectable)
            {
                box.SetSelected(true);
            }
        }
        else
        {
            box.SetSelected(false);
        }
    }

    public void SelectButtonPressed()
    {
        var indexes = GetSelectedIndexes();
        gameObject.SetActive(false);
        selectCallback(indexes);
    }

    List<int> GetSelectedIndexes()
    {
        return GetSelectedBoxes().Select(selectedBox => selectedBox.selectionIndex).ToList();
    }
}
