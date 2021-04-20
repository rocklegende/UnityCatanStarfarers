using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCardHandler : MonoBehaviour
{
    public DecisionTreeNode currentDecisionNode;
    public EncounterCardStack encounterCardStack;
    public GameObject HUD;
    private GameObject decisionDialog;

    // Start is called before the first frame update
    void Start()
    {
        decisionDialog = HUD.GetComponent<HUDScript>().decisionDialog;
        decisionDialog.SetActive(false);
    }

    /// <summary>
    /// Play next encounter card from the stack.
    /// Throws EmptyCardStackException if stack is empty.
    /// 
    /// </summary>
    public void PlayNextCard()
    {
        if (encounterCardStack.IsEmpty())
        {
            throw new EmptyCardStackException("Encounter stack is empty!");
        }

        var card = encounterCardStack.Pop();
        PlayEncounterCard(card);
    }

    public void PlayEncounterCard(EncounterCard card)
    {
        decisionDialog.SetActive(true);
        currentDecisionNode = card.decisionTree.root;
        OpenDialogForDecisionNode(currentDecisionNode);      
    }

    void OpenDialogForDecisionNode(DecisionTreeNode currentNode)
    {
        var decisionDialogScript = decisionDialog.GetComponent<DecisionDialog>();
        decisionDialog.SetActive(true); //open the dialog
        decisionDialogScript.RemoveButtons(); //present no options initially
        decisionDialogScript.SetText(currentNode.text);

        if (currentDecisionNode.action != null)
        {
            currentDecisionNode.action.SetCallback(ValueChosen);
            currentDecisionNode.action.ExecuteTemplateMethod();
        }
        else
        {
            decisionDialogScript.PresentDoneButton(EncounterCardDone);
        }
    }

    void ValueChosen(EncounterActionValue value)
    {
        decisionDialog.SetActive(false); //close it

        if (currentDecisionNode.HasNext())
        {
            foreach (var node in currentDecisionNode.GetNext())
            {
                if (node.GetsTriggeredByValue(value.value))
                {
                    currentDecisionNode = node;
                    OpenDialogForDecisionNode(currentDecisionNode);
                    break;
                }
            }
        }
    }

    void EncounterCardDone()
    {
        decisionDialog.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
