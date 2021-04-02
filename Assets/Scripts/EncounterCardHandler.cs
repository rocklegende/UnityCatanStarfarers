using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCardHandler : MonoBehaviour
{

    public DecisionTreeNode currentDecisionNode;
    public GameObject decisionDialog; //TODO: move this reference to hud script in future
    public EncounterCardStack encounterCardStack;
    public GameObject HUD;

    // Start is called before the first frame update
    void Start()
    {
        decisionDialog.SetActive(false);
        var hudScript = HUD.GetComponent<HUDScript>();

        var noResourcesGiven = new DecisionTreeNode(null, 0, new LoseOneFameMedalAction(hudScript))
        {
            text = "You gave me no resources, thank you, you win one fame medal"
        };
        var oneResourceGiven = new DecisionTreeNode(null, 1, new LoseOneFameMedalAction(hudScript))
        {
            text = "You lost one fame medal you cheap fuck"
        };
        var twoResourcesGiven = new DecisionTreeNode(null, 2, new WinOneFameMedalAction(hudScript))
        {
            text = "You gave me two resources, thank you, you win one fame medal"
        };
        var threeResourcesGiven = new DecisionTreeNode(null, 3, new WinOneFameMedalAction(hudScript))
        {
            text = "You gave me three resources, thank you, you win one fame medal"
        };


        var dontGiveResourcesOption = new DecisionTreeNode(null, false, null)
        {
            text = "OK, you can fly away"
        };

        var giveResourcesAction = new GiveupResourcesEncounterAction(hudScript, 3);

        var yesGiveResourcesOption = new DecisionTreeNode(new DecisionTreeNode[] {
            noResourcesGiven,
            oneResourceGiven,
            twoResourcesGiven,
            threeResourcesGiven
        }, true, giveResourcesAction)
        {
            text = "OK, give me up to three resources"
        };

        //var action = new YesOrNoEncounterAction(hudScript);
        var action = new FightEncounterAction(hudScript, FightCategory.CANNON);

        var giveResourceCard = new DecisionTreeNode(new DecisionTreeNode[] { dontGiveResourcesOption, yesGiveResourcesOption }, null, action)
        {
            text = "You see a starfarer that needs help with resources, do you help?"
        };

        var decisionTree = new DecisionTree(giveResourceCard);

        var encounter = new EncounterCard(decisionTree);

        encounterCardStack = new EncounterCardStack(new List<EncounterCard> { encounter });
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

    //TODO: should not be public, just for testing purposes
    public void PlayEncounterCard(EncounterCard card)
    {
        //var decisionDialogScript = decisionDialog.GetComponent<DecisionDialog>();
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
            currentDecisionNode.action.Execute();
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
                var bla = value.value.Equals(node.decisionValue);
                if (node.GetsTriggeredByValue(value.value))
                {
                    currentDecisionNode = node;
                    OpenDialogForDecisionNode(currentDecisionNode);
                    break;
                }
            }
        }
    }

    //void OnUserChoseOption(DialogOption option)
    //{
    //    decisionDialog.SetActive(false); //close it

    //    if (currentDecisionNode.HasNext())
    //    {
    //        foreach (var node in currentDecisionNode.GetNext())
    //        {
    //            if (node.GetsTriggeredByValue(option.value))
    //            {
    //                currentDecisionNode = node;
    //                OpenDialogForDecisionNode(currentDecisionNode);
    //                break;
    //            }
    //        }
    //    } 
    //}

    void EncounterCardDone()
    {
        decisionDialog.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
