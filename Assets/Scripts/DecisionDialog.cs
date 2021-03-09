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

public class DialogOption
{
    public object value;
    public string text;
    public readonly string id;
    public DialogOption(string id, string text, object value)
    {
        this.id = id;
        this.value = value;
        this.text = text;
    }
}

public class DecisionDialog : MonoBehaviour
{

    public Text textObject;
    string text;
    Action<DialogOption> callback;
    public Button btnPrefab;
    public GameObject buttonHorizontalStack;

    public void SetText(string text)
    {
        this.textObject.text = text;
    }

    public void SetCallback(Action<DialogOption> callback)
    {
        this.callback = callback;
    }

    public void SetOptions(DialogOption[] options)
    {
        for (int i = 0; i < options.Length ; i++)
        {
            var dialogButton = options[i];
            var button = Instantiate(btnPrefab, buttonHorizontalStack.transform, false);
            var text = button.GetComponentInChildren<Text>();
            text.text = dialogButton.text;
            button.onClick.AddListener(() => OnOptionChosen(dialogButton));
            button.transform.parent = buttonHorizontalStack.transform;
        }
    }

    public void OnOptionChosen(DialogOption option)
    {
        callback(option);
    }

    // Start is called before the first frame update
    void Start()
    {       

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
