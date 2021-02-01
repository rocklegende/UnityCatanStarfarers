using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuildDropDown : MonoBehaviour
{

    BuildDropDownOption[] options;
    GameObject[] buttons;
    public GameObject dropDownBtnPrefab;
    public bool isOpen = false;

    // Use this for initialization
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void SetOptions(BuildDropDownOption[] opts)
    {
        options = opts;
        DrawOptions();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void show()
    {
        isOpen = true;
        gameObject.SetActive(isOpen);
    }

    public void hide()
    {

        isOpen = false;
        gameObject.SetActive(isOpen);
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
        int index = Array.FindIndex<BuildDropDownOption>(options, opt => opt.imageName == option.imageName);
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

        btn.transform.parent = this.transform;

        return btn;

    }

}
