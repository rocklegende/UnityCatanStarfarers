using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDScript : SFController, FriendShipCardSelectorDelegate
{
    public Player player { get { return globalGamecontroller.mainPlayer; } }
    public List<Player> players { get { return globalGamecontroller.players; } }
    public Text oreCardStackText;
    public Text carbonCardStackText;
    public Text foodCardStackText;
    public Text fuelCardStackText;
    public Text goodsCardStackText;

    public Text boosterText;
    public Text cannonsText;
    public Text freightPodsText;
    public Text boosterBonusText;
    public Text cannonsBonusText;
    public Text freightPodsBonusText;
    public Text playerNameText;

    public Text vpText;
    public Text fameMedalPiecesText;

    public Text ShipsLeft;
    public Text ColoniesLeft;
    public Text TradeLeft;
    public Text PortLeft;

    public GameObject buildShipsDropDownRef;
    public GameObject upgradesDropDownRef;

    public GameObject oreCardStack;
    public GameObject carbonCardStack;
    public GameObject fuelCardStack;
    public GameObject goodsCardStack;
    public GameObject foodCardStack;
    public List<BuildDropDownOption> buildDropDownOptions;
    public bool isReceivingNotifications = false;
    public bool isInteractionActivated = true;

    public Text stateText;
    public GameObject settleButton;

    public GameObject friendShipCardSelection;
    public GameObject smallPlayerViewPrefab;
    public GameObject otherPlayerContainer;

    public GameObject shipDiceThrowRenderer;
    public GameObject normalDiceThrowRenderer;
    public GameObject resourcePicker;
    public GameObject decisionDialog;
    public GameObject fightPanel;
    public GameObject tradePanel;
    public GameObject tradeOfferView;

    public GameObject DoFameMedalBuyButton;
    public GameObject RichHelpPoorBonusButton;

    public GameObject MapObject;

    public GameObject playerSelectionView;
    public GameObject multiSelectionView;
    List<GameObject> smallPlayerViews = new List<GameObject>();
    public Button BuildShipsButton;

    private List<BuildDropDownOption> buildShipsDropdownOptions;
    private List<BuildDropDownOption> buildUpgradesDropdownOptions;



    // Start is called before the first frame update
    void Start()
    {
        friendShipCardSelection.SetActive(false);
        friendShipCardSelection.GetComponent<FriendShipCardSelector>().delegate_ = this;
        shipDiceThrowRenderer.SetActive(false);
        tradePanel.SetActive(false);
        resourcePicker.SetActive(false);
        tradeOfferView.SetActive(false);
        CloseSelection();
        CreateBuildDropDowns();

    }


    // Update is called once per frame
    void Update()
    {

    }

    public List<GameObject> GetSmallPlayerViews()
    {
        return smallPlayerViews;
    }

    public void SomeButtonPressed()
    {
        tradeOfferView.GetComponent<TradeOfferView>().Draw();
    }

    public void DisplayTradeOffer(TradeOffer tradeOffer, System.Action<bool> didMakeDecision)
    {
        tradeOfferView.SetActive(true);
        tradeOfferView.GetComponent<TradeOfferView>().Init(tradeOffer, didMakeDecision);
        tradeOfferView.GetComponent<TradeOfferView>().Draw();
    }

    public void OnRichHelpPoorButtonPressed()
    {
        var action = new TakeResourceFromOpponent(this, player, RichHelpPoorPlayed, 1, 2);
        action.StartAction();
    }

    void RichHelpPoorPlayed()
    {
        player.RichHelpPoorMoveMade();
    }

    public void OnFameMedalTradeButtonClicked()
    {
        player.BuyFameMedal();
    }

    //void OpenSelection(List<SFModel> selectableObjects, System.Action<List<SFModel>> selectedTokenCallback)
    //{
    //    multiSelectionView.SetActive(true);
    //    var multiSelectScript = multiSelectionView.GetComponent<MultiSelection>();
    //    multiSelectScript.SetSelectableObjects(selectableObjects.ToArray());
    //    multiSelectScript.selectCallback = selectedTokenCallback;
    //}

    public void CloseSelection()
    {
        multiSelectionView.SetActive(false);
    }

    public void OpenUpgradeSelection(List<Upgrade> selectableUpgrades, System.Action<List<int>> selectedIndexesCallback, int maxSelectable = -1)
    //TODO: remove Duplication in OpenPlayerSelection and OpenUpgradeSelection methods
    {
        if (selectableUpgrades.Count == 0)
        {
            selectedIndexesCallback(new List<int>() { });
            return;
        }

        multiSelectionView.SetActive(true);
        var multiSelectScript = multiSelectionView.GetComponent<MultiSelection>();
        multiSelectScript.SetSelectableObjects(selectableUpgrades.ToArray());
        multiSelectScript.selectCallback = selectedIndexesCallback;
        if (maxSelectable != -1 && maxSelectable > 0)
        {
            multiSelectScript.maxSelectable = maxSelectable;
        }

    }

    public void OpenPlayerSelection(List<Player> selectablePlayers, System.Action<List<int>> selectedIndexesCallback, int maxSelectable = -1)
    {
        if (selectablePlayers.Count == 0)
        {
            selectedIndexesCallback(new List<int>() { });
            return;
        }
        multiSelectionView.SetActive(true);
        var multiSelectScript = multiSelectionView.GetComponent<MultiSelection>();
        multiSelectScript.SetSelectableObjects(selectablePlayers.ToArray());
        multiSelectScript.selectCallback = selectedIndexesCallback;
        if (maxSelectable != -1 && maxSelectable > 0)
        {
            multiSelectScript.maxSelectable = maxSelectable;
        }

    }

    public void OpenNormalDiceThrowRenderer(System.Action<DiceThrow> callback)
    {
        normalDiceThrowRenderer.GetComponent<DiceThrowRenderer>().callback = callback;
        normalDiceThrowRenderer.SetActive(true);
    }

    public void CloseNormalDiceThrowRenderer()
    {
        normalDiceThrowRenderer.SetActive(false);
    }

    public void OpenResourcePicker(System.Action<Hand> callback, int cardLimit = -1, int onlySelectableAtValue = -1, bool resetValues = true)
    {
        //TODO: possibly better to instantiate a new gameobject here to reset all values, so we dont run into problems if we open this multiple times

        resourcePicker.SetActive(true);

        if (resetValues)
        {
            resourcePicker.GetComponent<ResourcePicker>().Reset();
        }

        resourcePicker.GetComponent<ResourcePicker>().SetTotalMaximum(cardLimit);
        resourcePicker.GetComponent<ResourcePicker>().SetOnlySelectableAtValue(onlySelectableAtValue);
        resourcePicker.GetComponent<ResourcePicker>().SetCallback(callback);
    }

    /// <summary>
    /// Opens the Resource Card Picker for discarding, sets the hand limit of the picker automatically to that of the main player.
    /// </summary>
    /// <param name="callback"></param>
    /// <param name="cardLimit"></param>
    /// <param name="onlySelectableAtValue"></param>
    public void OpenDiscardResourcePicker(System.Action<Hand> callback, int cardLimit = -1, int onlySelectableAtValue = -1)
    {
        //TODO: possibly better to instantiate a new gameobject here to reset all values, so we dont run into problems if we open this multiple times

        OpenResourcePicker(callback, cardLimit, onlySelectableAtValue);
        resourcePicker.GetComponent<ResourcePicker>().SetHandLimit(player.hand);
    }

    public void CloseResourcePicker()
    {
        resourcePicker.SetActive(false);
    }

    void CreateBuildDropDowns()
    {
        CreateBuildShipsDropdown();
        CreateUpgradeDropdown();
    }

    private void CreateUpgradeDropdown()
    {
        var upgradeOptions = new List<BuildDropDownOption> {
            new BuildDropDownOption(new BoosterUpgradeToken(), "booster", new BoosterUpgradeToken().cost),
            new BuildDropDownOption(new CannonUpgradeToken(), "cannon", new CannonUpgradeToken().cost),
            new BuildDropDownOption(new FreightPodUpgradeToken(), "freightpod", new FreightPodUpgradeToken().cost)
        };
        buildUpgradesDropdownOptions = upgradeOptions;
        upgradesDropDownRef.GetComponent<BuildDropDown>().SetOptions(upgradeOptions);
        upgradesDropDownRef.GetComponent<BuildDropDown>().optionSelectedCallback = UpgradeDropdownOptionSelected;
    }

    private void CreateBuildShipsDropdown()
    {
        var buildShipsOptions = new List<BuildDropDownOption> {
            new BuildDropDownOption(new SpacePortToken(), "build_space_port_btn", new SpacePortToken().cost),
            new BuildDropDownOption(new TradeBaseToken(), "build_trade_ship_btn", new TradeBaseToken().cost),
            new BuildDropDownOption(new ColonyBaseToken(),"build_colony_ship_btn", new ColonyBaseToken().cost)
        };
        buildShipsDropdownOptions = buildShipsOptions;
        buildShipsDropDownRef.GetComponent<BuildDropDown>().SetOptions(buildShipsOptions);
        buildShipsDropDownRef.GetComponent<BuildDropDown>().optionSelectedCallback = TokenDropdownOptionSelected;
    }

    public void TokenDropdownOptionSelected(int index)
    {
        CloseAllDropDowns();
        app.Notify(SFNotification.HUD_build_token_btn_clicked, this, new object[] { (Token)buildShipsDropdownOptions[index].buildableToken });
    }

    public void UpgradeDropdownOptionSelected(int index)
    {
        CloseAllDropDowns();
        app.Notify(SFNotification.HUD_build_upgrade_btn_clicked, this, new object[] { (Upgrade)buildUpgradesDropdownOptions[index].buildableToken });
    }

    public void SetStateText(string text)
    {
        stateText.text = text;
    }

    public void SetMainPlayer(Player player)
    {
        //this.player = player;
        //tradePanel.GetComponent<TradePanelScript>().Init(player);
        //OnPlayerDataChanged();
    }

    public void UpdatePlayers(List<Player> playersList, Player mainPlayer)
    {
        SetPlayers(playersList, mainPlayer);
        Draw();
    }

    public void SetPlayers(List<Player> playersList, Player mainPlayer)
    {
        //this.players = playersList;

        //var playersOtherThanMain = playersList.Where(p => p != mainPlayer);
        //SetMainPlayer(mainPlayer);

        //foreach (var notMainPlayer in playersOtherThanMain)
        //{
        //    if (smallPlayerViews.Count > 0)
        //    {
        //        UpdateExistingSmallPlayerView(notMainPlayer);
        //    }
        //    else
        //    {
        //        AddSmallPlayerView(notMainPlayer);
        //    }
        //}

    }

    void UpdateExistingSmallPlayerView(Player playerData)
    {
        var playerView = smallPlayerViews.Find(view => view.GetComponent<SmallPlayerInfoView>().player.name == playerData.name);
        if (playerView != null)
        {
            Debug.Log(string.Format("Setting new player: {0}; num cards: {1}", playerData.name, playerData.hand.Count()));
            playerView.GetComponent<SmallPlayerInfoView>().SetPlayer(playerData);
        }
        else
        {
            Logger.LogError("Couldnt find smallPlayerView for name: " + playerData.name);
        }
    }

    void AddSmallPlayerView(Player playerData)
    {
        GameObject smallPlayerView = Instantiate(smallPlayerViewPrefab, transform, false);
        smallPlayerView.GetComponent<SmallPlayerInfoView>().SetPlayer(playerData);
        smallPlayerView.transform.parent = otherPlayerContainer.transform;
        smallPlayerViews.Add(smallPlayerView);
    }

    public void ShowShipDiceThrowPanel(System.Action<ShipDiceThrow> callback)
    {
        shipDiceThrowRenderer.SetActive(true);
        shipDiceThrowRenderer.GetComponent<ShipDiceThrowRenderer>().callback = callback;
    }

    public void CloseShipDiceThrowPanel()
    {
        shipDiceThrowRenderer.SetActive(false);
    }

    public void ShowSettleButton(bool show)
    {
        settleButton.SetActive(show);
    }

    public void OnPlayerDataChanged()
    {
        Draw();
    }

    public void Draw()
    {
        Debug.Log("Drawing hud");
        DrawResourceStacks();
        DrawUpgrades();
        DrawVP();
        DrawFameMedalPieces();
        DrawTokenLeft();
        DrawDropDowns();
        DrawExtraActionButtons();
        DrawName();
        DrawSmallPlayerViews();
    }

    void DrawSmallPlayerViews()
    {
        foreach (var smallPlayerView in smallPlayerViews)
        {
            smallPlayerView.GetComponent<SmallPlayerInfoView>().Draw();
        }
    }

    public void ActivateAllInteraction(bool isInteractive)
    {
        this.isInteractionActivated = isInteractive;
        foreach (var button in GetComponentsInChildren<Button>())
        {
            button.interactable = isInteractive;
        }
    }

    void DrawName()
    {
        playerNameText.text = player.name;
    }

    void DrawExtraActionButtons()
    {
        DoFameMedalBuyButton.SetActive(player.CanBuyFameMedal());

        var richHelpPoorBonusIsPossible = player.hasRichHelpPoorBonus && !player.richHelpPoorBonusMadeThisRound;
        RichHelpPoorBonusButton.SetActive(richHelpPoorBonusIsPossible);
    }

    void DrawFameMedalPieces()
    {
        fameMedalPiecesText.text = player.FameMedalPieces.ToString();
    }

    void DrawResourceStacks()
    {
        Hand hand = player.hand;
        carbonCardStackText.text = hand.NumberCardsOfType<CarbonCard>().ToString();
        goodsCardStackText.text = hand.NumberCardsOfType<GoodsCard>().ToString();
        fuelCardStackText.text = hand.NumberCardsOfType<FuelCard>().ToString();
        foodCardStackText.text = hand.NumberCardsOfType<FoodCard>().ToString();
        oreCardStackText.text = hand.NumberCardsOfType<OreCard>().ToString();
    }

    void DrawUpgrades()
    {
        boosterText.text = player.ship.Boosters.ToString();
        if (player.ship.BoostersBonus > 0)
        {
            boosterBonusText.text = "+ " + player.ship.BoostersBonus.ToString();
        }

        cannonsText.text = player.ship.Cannons.ToString();
        if (player.ship.CannonsBonus > 0)
        {
            cannonsBonusText.text = "+ " + player.ship.CannonsBonus.ToString();
        }

        freightPodsText.text = player.ship.FreightPods.ToString();
        if (player.ship.FreightPodsBonus > 0)
        {
            freightPodsBonusText.text = "+ " + player.ship.FreightPodsBonus.ToString();
        }
    }

    void DrawVP()
    {
        vpText.text = player.GetVictoryPoints().ToString();
    }

    void DrawTokenLeft()
    {
        ShipsLeft.text = "Ships: " + player.tokenStorage.GetTokensOfType(new ShipToken().GetType()).Length.ToString();
        PortLeft.text = "Ports: " + player.tokenStorage.GetTokensOfType(new SpacePortToken().GetType()).Length.ToString();
        ColoniesLeft.text = "Cols: " + player.tokenStorage.GetTokensOfType(new ColonyBaseToken().GetType()).Length.ToString();
        TradeLeft.text = "Trade: " + player.tokenStorage.GetTokensOfType(new TradeBaseToken().GetType()).Length.ToString();
    }

    void DrawDropDowns()
    {
        DrawInteractibilityOfDropDownOptions();
    }

    void DrawInteractibilityOfDropDownOptions()
    {
        if (players.IsNotNull())
        {
            var allDropDowns = new List<BuildDropDown> {
                upgradesDropDownRef.GetComponent<BuildDropDown>(),
                buildShipsDropDownRef.GetComponent<BuildDropDown>()
            };

            foreach (var dropdown in allDropDowns)
            {
                foreach (var option in dropdown.GetOptions())
                {
                    if (option.buildableToken != null)
                    {

                        var map = MapObject.GetComponent<MapScript>().map;
                        var canBeBuild = option.buildableToken.CanBeBuildByPlayer(player, map);
                        dropdown.SetOptionInteractable(option, option.buildableToken.CanBeBuildByPlayer(player, map));
                    }
                }
            }

        }
    }

    public void OpenTradePanel(int numCardsToGive, int numCardsToReceive, System.Action<Hand, Hand> callback)
    {
        tradePanel.SetActive(true);
        var tradePanelScript = tradePanel.GetComponent<TradePanelScript>();
        tradePanelScript.SetExactInput(numCardsToGive);
        tradePanelScript.SetExactOutput(numCardsToReceive);
        tradePanelScript.callback = callback;
    }

    public void CloseTradePanel()
    {
        tradePanel.SetActive(false);
    }

    public void NextButtonClicked()
    {
        app.Notify(SFNotification.next_button_clicked, this);
    }

    public void SettleButtonPressed()
    {
        app.Notify(SFNotification.settle_button_clicked, this);
    }

    public void ShowFriendshipCardSelection(TradeStation tradeStation)
    {
        friendShipCardSelection.GetComponent<FriendShipCardSelector>().DisplaySelectionForTradeStation(tradeStation);
        friendShipCardSelection.SetActive(true);
    }

    public void CloseFriendshipCardSelection()
    {
        friendShipCardSelection.SetActive(false);
    }


    public void BuildShipsToggleBtnPressed()
    {
        upgradesDropDownRef.GetComponent<BuildDropDown>().hide(); //close other dropdown if open
        buildShipsDropDownRef.GetComponent<BuildDropDown>().toggle();

    }

    public void CloseAllDropDowns()
    {

        buildShipsDropDownRef.GetComponent<BuildDropDown>().hide();
        upgradesDropDownRef.GetComponent<BuildDropDown>().hide();
    }

    public void BuildUpgradeToggleBtnPressed()
    {
        buildShipsDropDownRef.GetComponent<BuildDropDown>().hide(); //close other dropdown if open
        upgradesDropDownRef.GetComponent<BuildDropDown>().toggle();
    }

    public void MakeTradeBtnPressed()
    {
        if (tradePanel.activeInHierarchy)
        {
            tradePanel.SetActive(false);
        }
        else
        {
            tradePanel.SetActive(true);
        }
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        if (isReceivingNotifications)
        {
            switch (p_event_path)
            {
                case SFNotification.open_friendship_card_selection:
                    var tradeStation = (TradeStation)p_data[0];
                    ShowFriendshipCardSelection(tradeStation);
                    break;
            }
        }
    }

    public void didSelectFriendshipCard(TradeStation station, AbstractFriendshipCard card)
    {
        CloseFriendshipCardSelection();
        station.RemoveCard(card);
        player.AddFriendShipCard(card);
    }
}
