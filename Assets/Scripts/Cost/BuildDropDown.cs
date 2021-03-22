using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuildDropDown : SFController
{

    List<BuildDropDownOption> options;
    GameObject[] buttons;
    public GameObject dropDownBtnPrefab;
    public bool isOpen = false;
    public GameObject dropDown;

    // Use this for initialization
    void Start()
    {
        dropDown.SetActive(false);
    }

    public void SetOptions(List<BuildDropDownOption> opts)
    {
        options = opts;
        DrawOptions();
    }

    public List<BuildDropDownOption> GetOptions()
    {
        return options;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void show()
    {
        isOpen = true;
        dropDown.SetActive(isOpen);
    }

    public void hide()
    {

        isOpen = false;
        dropDown.SetActive(isOpen);
    }

    public void toggle()
    {
        if (isOpen)
        {
            hide();
        } else
        {
            show();
        }
    }

    void DrawOptions()
    {

        float verticalOffset = 60.0f;
        Vector3 position = new Vector3(200, 200, 0);

        List<GameObject> list = new List<GameObject>();

        foreach(BuildDropDownOption option in options)
        {
            GameObject btn = DrawOption(option, position);
            position += new Vector3(0, verticalOffset, 0);
            list.Add(btn);
        }

        buttons = list.ToArray();
    }

    public void SetOptionInteractable(BuildDropDownOption option, bool isInteractable)
    {
        int index = options.FindIndex(opt => opt.imageName == option.imageName); //TODO: finding based on image name looks weird and could easily not work if changes are made
        this.buttons[index].GetComponent<Button>().interactable = isInteractable;
    }

    

    GameObject DrawOption(BuildDropDownOption option, Vector3 position)
    {
        Sprite sprite = new Helper().CreateSpriteFromImageName(option.imageName);
        GameObject btn = (GameObject)Instantiate(dropDownBtnPrefab, position, Quaternion.identity);
        btn.GetComponentInChildren<BuildDropDownBtnImageScript>().SetSprite(sprite);
        Button btnComponent = btn.GetComponent<Button>();
        btnComponent.onClick.AddListener(delegate { option.method(option.token); }); 

        Cost cost = new Cost(new Resource[] { new FoodResource(), new FoodResource(), new GoodsResource() }); 
        btn.GetComponentInChildren<CostRenderer>().SetCost(option.token.cost);

        btn.transform.parent = dropDown.transform;

        return btn;

    }

    public override void OnNotification(string p_event_path, UnityEngine.Object p_target, params object[] p_data)
    {
        
    }
}
