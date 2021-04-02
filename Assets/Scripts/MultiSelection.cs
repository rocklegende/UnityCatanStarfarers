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
    public System.Action<List<SFModel>> selectCallback;
    public List<SelectionBox> selectionBoxes = new List<SelectionBox>();

    public bool multiselect = true;
    public int multiSelectMaxSelectable = 2;

    public List<GameObject> selectionBoxObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        //TODO: this line is only to debug, remove in production
        //SetSelectableObjects(new Token[] { new BoosterUpgradeToken(), new FreightPodUpgradeToken() });
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
        foreach (var obj in objects)
        {
            var playerBox = Instantiate(selectionBoxPrefab, optionsHorizontalStack.transform, false);
            playerBox.GetComponent<SelectionBox>().Init(obj);
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
        if (multiselect)
        {
            if (!box.isSelected)
            {
                if (GetSelectedBoxes().Count < multiSelectMaxSelectable)
                {
                    box.SetSelected(true);
                }
            }
            else
            {
                box.SetSelected(false);
            }

        }
        else
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
        var selectedObjects = GetSelectedObjects();
        selectCallback(selectedObjects);
    }

    List<SFModel> GetSelectedObjects()
    {
        return selectionBoxes.Where(box => box.isSelected).Select(selectedBox => selectedBox.GetObject()).ToList();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
