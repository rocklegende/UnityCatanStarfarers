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
        GiveResourcesEncounterCard giveResources = new GiveResourcesEncounterCard("Jo give me all your resources bitch", null);

        encounterCardStack = new EncounterCardStack(new List<EncounterCard> { encounter, giveResources });
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

    void PlayEncounterCard(EncounterCard card)
    {
        var decisionDialogScript = decisionDialog.GetComponent<DecisionDialog>();
        decisionDialogScript.SetCallback(OnUserChoseOption);
        decisionDialog.SetActive(true);
        if (card is YesOrNoEncounterCard)
        {
            var yesOrNoCard = (YesOrNoEncounterCard)card;
            currentDecisionNode = yesOrNoCard.tree.root;
            decisionDialogScript.SetOptions(yesOrNoCard.GetDialogOptions());
            OpenDialogForDecisionNode(currentDecisionNode);
        }

        if (card is GiveResourcesEncounterCard)
        {
            var giveResourcesCard = (GiveResourcesEncounterCard)card;
            decisionDialogScript.SetOptions(giveResourcesCard.GetDialogOptions());
        }         
    }

    void OpenDialogForDecisionNode(YesOrNoActionTreeNode currentNode)
    {
        decisionDialog.SetActive(true); //open the dialog
        decisionDialog.GetComponent<DecisionDialog>().SetText(currentNode.text);
    }

    void OnUserChoseOption(DialogOption option)
    {
        decisionDialog.SetActive(false); //close it
        if (option.value is bool)
        {
            var response = (bool)option.value;
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
                OpenDialogForDecisionNode(currentDecisionNode);
            }
        }

        if (option.value is int)
        {
            Debug.Log("Chose integer value: " + (int)option.value);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
