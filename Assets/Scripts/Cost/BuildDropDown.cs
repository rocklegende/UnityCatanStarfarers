using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BuildDropDown : SFController
{

    List<BuildDropDownOption> options;
    List<Button> buttons;
    public GameObject dropDownBtnPrefab;
    public bool isOpen = false;
    public GameObject dropDown;
    public Action<int> optionSelectedCallback;

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

    public void SelectOptionAtIndex(int index)
    {
        if (index < 0 || index >= options.Count)
        {
            throw new ArgumentException("index not valid");
        }
        buttons[index].onClick.Invoke();
    }

    public List<BuildDropDownOption> GetOptions()
    {
        return options;
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

        List<Button> list = new List<Button>();

        foreach (BuildDropDownOption option in options)
        {
            GameObject btn = DrawOption(option, position);
            position += new Vector3(0, verticalOffset, 0);
            list.Add(btn.GetComponent<Button>());
        }

        buttons = list;
    }

    public void SetOptionInteractable(BuildDropDownOption option, bool isInteractable)
    {
        //TODO: finding based on image name looks weird and could easily not work if changes are made
        int index = options.FindIndex(opt => opt.imageName == option.imageName); 
        this.buttons[index].interactable = isInteractable;
    }    

    GameObject DrawOption(BuildDropDownOption option, Vector3 position)
    {
        Sprite sprite = new Helper().CreateSpriteFromImageName(option.imageName);
        GameObject btn = (GameObject)Instantiate(dropDownBtnPrefab, position, Quaternion.identity);
        btn.GetComponentInChildren<BuildDropDownBtnImageScript>().SetSprite(sprite);
        btn.GetComponentInChildren<CostRenderer>().SetCost(option.cost);
        btn.transform.parent = dropDown.transform;

        Button btnComponent = btn.GetComponent<Button>();
        btnComponent.onClick.AddListener(() => DidSelectDropdownOption(option.imageName)); 

        return btn;

    }

    void DidSelectDropdownOption(string imageName)
    {
        optionSelectedCallback(options.FindIndex(opt => opt.imageName == imageName));
    }


    public override void OnNotification(string p_event_path, UnityEngine.Object p_target, params object[] p_data)
    {
        
    }
}
