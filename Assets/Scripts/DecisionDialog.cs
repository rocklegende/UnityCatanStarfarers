using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogButtonValue
{
    public object value;
    public DialogButtonValue(object value)
    {
        this.value = value;
    }
}

public class DialogButton
{
    //Action<DialogButtonValue> action;
    //string text;
    //public DialogButton(string text, Action<DialogButtonValue> action)
    //{
    //    this.action = action;
    //    this.text = text;
    //}

    public DialogButtonValue value;
    public string text;
    public DialogButton(string text, DialogButtonValue value)
    {
        this.value = value;
        this.text = text;
    }

}

public class DecisionDialog : MonoBehaviour
{

    public Text textObject;
    string text;
    Action<bool> callback;
    public Button btnPrefab;
    public GameObject buttonHorizontalStack;

    //DialogButton

    public void SetText(string text)
    {
        this.textObject.text = text;
    }

    public void SetCallback(Action<bool> callback)
    {
        this.callback = callback;
    }

    public void SetButtons(DialogButton[] buttons)
    {
        for (int i = 0; i < buttons.Length ; i++)
        {
            var dialogButton = buttons[i];
            var button = Instantiate<Button>(btnPrefab, buttonHorizontalStack.transform, false);
            var text = button.GetComponentInChildren<Text>();
            text.text = dialogButton.text;
            button.onClick.AddListener(() => OnOptionChosen(dialogButton.value.value));
            button.transform.parent = buttonHorizontalStack.transform;
        }
    }

    public void OnOptionChosen(object value)
    {
        Debug.Log("You chose option " + value);
    }

    public void OnYesButtonClicked()
    {
        callback(true);
    }

    public void OnNoButtonClicked()
    {
        callback(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        //var buttons = new DialogButton[]
        //{
        //    new DialogButton("0", new DialogButtonValue(0)),
        //    new DialogButton("1", new DialogButtonValue(1)),
        //    new DialogButton("2", new DialogButtonValue(2)),
        //    new DialogButton("3", new DialogButtonValue(3))
        //};

        var buttons = new DialogButton[]
        {
            new DialogButton("Yes", new DialogButtonValue(true)),
            new DialogButton("No", new DialogButtonValue(false)),
        };

        SetButtons(buttons);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
