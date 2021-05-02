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
    public string buttonText;
    public readonly string id;
    public DialogOption(string id, string buttonText, object value)
    {
        this.id = id;
        this.value = value;
        this.buttonText = buttonText;
    }
}

public class DecisionDialog : MonoBehaviour
{

    public Text textObject;
    string text;
    Action<DialogOption> callback;
    Action doneCallback;
    public Button btnPrefab;
    public GameObject buttonHorizontalStack;
    public List<Button> currentButtons;

    public void SetText(string text)
    {
        this.textObject.text = text;
    }

    public void SetCallback(Action<DialogOption> callback)
    {
        this.callback = callback;
    }

    void SetDoneCallback(Action doneCallback)
    {
        this.doneCallback = doneCallback;
    }

    public void SetOptions(DialogOption[] options)
    {
        RemoveButtons();
        for (int i = 0; i < options.Length ; i++)
        {
            var dialogButton = options[i];
            var button = Instantiate(btnPrefab, buttonHorizontalStack.transform, false);
            var buttonTextComponent = button.GetComponentInChildren<Text>();
            buttonTextComponent.text = dialogButton.buttonText;
            button.onClick.AddListener(() => OnOptionChosen(dialogButton));
            button.transform.parent = buttonHorizontalStack.transform;
            currentButtons.Add(button);
        }
    }

    public Button GetButtonWithText(string text)
    {
        return currentButtons.Find(button => button.GetComponentInChildren<Text>().text == text);
    }

    public void RemoveButtons()
    {
        for (int i = 0; i < buttonHorizontalStack.transform.childCount; i++)
        {
            GameObject child = buttonHorizontalStack.transform.GetChild(i).gameObject;
            Destroy(child);
        }
    }

    public void PresentDoneButton(Action callback)
    {
        RemoveButtons();
        SetDoneCallback(callback);
        var button = Instantiate(btnPrefab, buttonHorizontalStack.transform, false);
        var buttonTextComponent = button.GetComponentInChildren<Text>();
        buttonTextComponent.text = "Done";
        button.onClick.AddListener(() => DoneButtonClicked());
        button.transform.parent = buttonHorizontalStack.transform;
    }

    void DoneButtonClicked()
    {
        doneCallback();
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
