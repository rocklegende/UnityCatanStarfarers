using System;
using UnityEngine;


public class ResourcePicker : MonoBehaviour
{
    Action<Hand> callback;
    public GameObject resourceCardStack;

    public ResourcePicker()
    {
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
