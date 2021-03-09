using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCardHandler : MonoBehaviour
{

    public YesOrNoActionTreeNode currentDecisionNode;
    public GameObject decisionDialog; //TODO: move this reference to hud script in future
    public EncounterCardStack encounterCardStack;

    // Start is called before the first frame update
    void Start()
    {
        decisionDialog.SetActive(false);

        var youCanLeaveOption = new YesOrNoActionTreeNode("You can leave", null, null, null);
        var youHaveToFightOption = new YesOrNoActionTreeNode("You have to fight", null, null, null);
        var root = new YesOrNoActionTreeNode("Somebody needs help", null, youHaveToFightOption, youCanLeaveOption);
        var decisionTree = new YesOrNoActionTree(root);

        EncounterCard encounter = new YesOrNoEncounterCard("Example encounter card", decisionTree);

        encounterCardStack = new EncounterCardStack(new List<EncounterCard> { encounter });
        PlayEncounterCard(encounter);
    }

    public void PlayNextCard()
    {
        if (encounterCardStack.IsEmpty())
        {
            throw new EmptyCardStackException("Encounter stack is empty!");
        }

        var card = encounterCardStack.Pop();
        PlayEncounterCard(card);
    }

    void PlayEncounterCard(EncounterCard card)
    {
        if (card is YesOrNoEncounterCard)
        {
            var yesOrNoCard = (YesOrNoEncounterCard)card;
            currentDecisionNode = yesOrNoCard.tree.root;
        }
        decisionDialog.GetComponent<DecisionDialog>().SetCallback(OnUserInputReceived);
        OpenDialogAndProcessUserInput(currentDecisionNode);
    }

    void OpenDialogAndProcessUserInput(YesOrNoActionTreeNode currentNode)
    {
        decisionDialog.SetActive(true); //open the dialog
        decisionDialog.GetComponent<DecisionDialog>().SetText(currentNode.text);
    }

    void OnUserInputReceived(bool response)
    {
        decisionDialog.SetActive(false); //close it

        if (response)
        {
            currentDecisionNode = currentDecisionNode.yes;
        }

        if (!response)
        {
            currentDecisionNode = currentDecisionNode.no;
        }
        if (currentDecisionNode != null)
        {
            OpenDialogAndProcessUserInput(currentDecisionNode);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
