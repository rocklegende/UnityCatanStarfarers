using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum SelectionComponentType
{
    PLAYER,
    UPGRADE
}

public class SelectionBox : SFView
{
    public bool isSelected = false;
    public System.Action<SelectionBox> itemSelectedCallback;
    public GameObject innerComponentContainer;
    public int selectionIndex;
    private SFModel obj;

    Color unselectedColor = ColorCreator.Create(200, 200, 200, 128);
    Color selectedColor = ColorCreator.Create(256, 256, 256, 256);

    public List<GameObject> possibleInnerComponentPrefabs;

    public void Init(SFModel obj)
    {
        this.obj = obj;
        var componentType = GetSelectionComponentType(obj);

        var prefab = GetPrefabFromEnum(componentType);
        if (prefab.IsNull())
        {
            throw new System.ArgumentException();
        }
        CreateInnerPrefab(prefab, componentType);
    }

    SelectionComponentType GetSelectionComponentType(SFModel obj)
    {
        if (obj is Player)
        {
            return SelectionComponentType.PLAYER;
        }

        if (obj is Token)
        {
            return SelectionComponentType.UPGRADE;
        }
        return SelectionComponentType.PLAYER;
    }

    void CreateInnerPrefab(GameObject prefab, SelectionComponentType componentType)
    {
        var gobj = Instantiate(prefab, innerComponentContainer.transform, false);
        var innerComponentScript = GetInnerComponentScript(gobj, componentType);
        innerComponentScript.InitializeViewFromObject(this.obj);
        gobj.transform.parent = innerComponentContainer.transform;
    }

    SelectionView GetInnerComponentScript(GameObject gobj, SelectionComponentType selectionComponent)
    {
        switch (selectionComponent)
        {
            case SelectionComponentType.PLAYER:
                return gobj.GetComponentInChildren<PlayerView>();
            case SelectionComponentType.UPGRADE:
                return gobj.GetComponentInChildren<UpgradeView>();
        }
        return null;
    }

    GameObject GetPrefabFromEnum(SelectionComponentType selectionComponent)
    {
        switch (selectionComponent)
        {
            case SelectionComponentType.PLAYER:
                return possibleInnerComponentPrefabs[0];
            case SelectionComponentType.UPGRADE:
                return possibleInnerComponentPrefabs[1];
        }
        return null;
    }

    public SFModel GetObject()
    {
        return obj;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        Highlight();
    }

    public void ButtonPressed()
    {
        itemSelectedCallback(this);
    }

    void Highlight()
    {
        var image = GetComponentInChildren<Image>();
        if (isSelected)
        {
            image.color = selectedColor;
        }
        else
        {
            image.color = unselectedColor;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Highlight();
    }

    // Update is called once per frame
    void Update()
    {
        //
    }
}
