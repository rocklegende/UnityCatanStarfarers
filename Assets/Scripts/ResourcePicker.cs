using System;
using UnityEngine;
using UnityEngine.UI;


public class ResourcePicker : MonoBehaviour
{
    Action<Hand> callback;
    public GameObject resourceCardStack;
    public Button selectButton;
    int onlySelectableAtValue = -1; //decided if the selection is only possible at a specific value, -1 means all is selectable

    public ResourcePicker()
    {
    }

    void Start()
    {
        resourceCardStack.GetComponent<ResourceCardStackRenderer>().SetChangedCallback(ResourcePickerHandChanged);
    }

    public void Reset()
    {
        resourceCardStack.GetComponent<ResourceCardStackRenderer>().ResetStacks();
    }

    public void SetOnlySelectableAtValue(int value)
    {
        onlySelectableAtValue = value;
        if (onlySelectableAtValue != -1)
        {
            var currentPickedHand = resourceCardStack.GetComponent<ResourceCardStackRenderer>().GetOutput();
            SetSelectButtonInteractive(onlySelectableAtValue == currentPickedHand.Count());
        }
    }

    void SetSelectButtonInteractive(bool active)
    {
        selectButton.interactable = active;
    }

    void ResourcePickerHandChanged(Hand newHand)
    {
        if (onlySelectableAtValue != -1 && onlySelectableAtValue >= 0)
        {
            SetSelectButtonInteractive(onlySelectableAtValue == newHand.Count());
        }
    }

    public void SetTotalMaximum(int amount)
    {
        resourceCardStack.GetComponent<ResourceCardStackRenderer>().SetTotalMaximum(amount);
    }

    public void SetCallback(Action<Hand> callback)
    {
        this.callback = callback;
    }

    public void OnSelectButtonClick()
    {
        var outputHand = resourceCardStack.GetComponent<ResourceCardStackRenderer>().GetOutput();
        Debug.Log("picked num resources: " + outputHand.Count());
        callback(outputHand);
    }
}
