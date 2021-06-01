using System;
using UnityEngine;
using UnityEngine.UI;


public class ResourcePicker : MonoBehaviour
{
    Action<Hand> callback;
    public GameObject resourceCardStack;
    public Button selectButton;
    public Text title;
    /// <summary>
    /// Decides if the selection is only possible at a specific value, -1 means all is selectable
    /// </summary>
    int onlySelectableAtValue = -1; 

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

    public void SetText(string text)
    {
        title.text = text;
    }

    public void SetOnlySelectableAtValue(int value)
    {
        onlySelectableAtValue = value;
        if (onlySelectableAtValue != -1)
        {
            var currentPickedHand = resourceCardStack.GetComponent<ResourceCardStackRenderer>().GetDisplayedHand();
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

    public Hand GetCurrentlyPickedHand()
    {
        return resourceCardStack.GetComponent<ResourceCardStackRenderer>().GetDisplayedHand();
    }

    public void SetHandLimit(Hand limitHand)
    {
        resourceCardStack.GetComponent<ResourceCardStackRenderer>().SetHandLimit(limitHand);
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
        var outputHand = resourceCardStack.GetComponent<ResourceCardStackRenderer>().GetDisplayedHand();
        gameObject.SetActive(false);
        callback(outputHand);
    }
}
