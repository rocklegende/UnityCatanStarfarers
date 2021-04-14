using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class HUDScript : SFController, FriendShipCardSelectorDelegate, Observer
{
    public Player player;
    public Player[] players;
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

    public GameObject DoFameMedalBuyButton;
    public GameObject RichHelpPoorBonusButton;

    public GameObject MapObject;

    public GameObject playerSelectionView;
    public GameObject multiSelectionView;



    // Start is called before the first frame update
    void Start()
    {
        friendShipCardSelection.SetActive(false);
        friendShipCardSelection.GetComponent<FriendShipCardSelector>().delegate_ = this;
        shipDiceThrowRenderer.SetActive(false);
        tradePanel.SetActive(false);
        resourcePicker.SetActive(false);
        CloseSelection();
        CreateBuildDropDowns();

        //Debugging
        //OpenResourcePicker(ResourcesPicked, 2, 2);

        
    }

    void ResourcesPicked(Hand bla)
    {
        Debug.Log("you picked resources");
    }
    

    // Update is called once per frame
    void Update()
    {
    }

    public void OpenPlayerSelection(Player[] players, System.Action<List<Player>> selectedCallback)
    {
        playerSelectionView.SetActive(true);
        playerSelectionView.GetComponent<PlayerSelectionView>().SetSelectablePlayers(players);
        playerSelectionView.GetComponent<PlayerSelectionView>().selectCallback = selectedCallback;
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

    public void OpenUpgradeSelection(List<Token> selectableUpgrades, System.Action<List<int>> selectedIndexesCallback, int maxSelectable = -1)
        //TODO: remove Duplication in OpenPlayerSelection and OpenUpgradeSelection methods
    {
        multiSelectionView.SetActive(true);
        var multiSelectScript = multiSelectionView.GetComponent<MultiSelection>();
        multiSelectScript.SetSelectableObjects(selectableUpgrades.ToArray());
        if (maxSelectable != -1 && maxSelectable > 0)
        {
            multiSelectScript.maxSelectable = maxSelectable;
        }


        multiSelectScript.selectCallback = selectedIndexesCallback;
    }

    public void OpenPlayerSelection(List<Player> selectablePlayers, System.Action<List<int>> selectedIndexesCallback, int maxSelectable = -1)
    {
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
        resourcePicker.SetActive(true);

        if (resetValues)
        {
            resourcePicker.GetComponent<ResourcePicker>().Reset();
        }

        resourcePicker.GetComponent<ResourcePicker>().SetTotalMaximum(cardLimit);
        resourcePicker.GetComponent<ResourcePicker>().SetOnlySelectableAtValue(onlySelectableAtValue);
        resourcePicker.GetComponent<ResourcePicker>().SetCallback(callback);
    }

    public void CloseResourcePicker()
    {
        resourcePicker.SetActive(false);
    }

    void CreateBuildDropDowns()
    {
        //TODO: refactor the building process
        List<BuildDropDownOption> options = new List<BuildDropDownOption>();
        var spacePortOption = new BuildDropDownOption("build_space_port_btn", new SpacePortToken(), BuildTokenBtnPressed);
        var buildTradeOption = new BuildDropDownOption("build_trade_ship_btn", new TradeBaseToken(), BuildTokenBtnPressed);
        var buildColonyOption = new BuildDropDownOption("build_colony_ship_btn", new ColonyBaseToken(), BuildTokenBtnPressed);

        var buildShipsOptions = new List<BuildDropDownOption> {
            buildColonyOption,
            buildTradeOption,
            spacePortOption
        };
        buildShipsDropDownRef.GetComponent<BuildDropDown>().SetOptions(buildShipsOptions);

        var upgradeOptions = new List<BuildDropDownOption> {
            new BuildDropDownOption("booster", new BoosterUpgradeToken(), BuildUpgradeBtnPressed),
            new BuildDropDownOption("cannon", new CannonUpgradeToken(), BuildUpgradeBtnPressed),
            new BuildDropDownOption("freightpod", new FreightPodUpgradeToken(), BuildUpgradeBtnPressed)
        };
        upgradesDropDownRef.GetComponent<BuildDropDown>().SetOptions(upgradeOptions);
    }

    public void SetStateText(string text)
    {
        stateText.text = text;
    }

    public void SetMainPlayer (Player player)
    {
        this.player = player;
        tradePanel.GetComponent<TradePanelScript>().Init(player);
        OnPlayerDataChanged();
    }

    /**
     * Player at first position is the main player
     */
    public void SetPlayers (Player[] players)
    {

        

        this.players = players;
        SetMainPlayer(players[0]);

        for (int i = 1; i < players.Length; i++)
        {
            players[i - 1].RegisterObserver(this);
            GameObject smallPlayerView = Instantiate(smallPlayerViewPrefab, transform, false);
            smallPlayerView.GetComponent<SmallPlayerInfoView>().SetPlayer(players[i]);
            smallPlayerView.transform.parent = otherPlayerContainer.transform;
        }

        

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

    void OnPlayerDataChanged()
    {
        Draw();
    }

    public void Draw()
    {
        DrawResourceStacks();
        DrawUpgrades();
        DrawVP();
        DrawFameMedalPieces();
        DrawTokenLeft();
        DrawDropDowns();
        DrawExtraActionButtons();
    }

    void DrawExtraActionButtons()
    {
        if (player.CanBuyFameMedal())
        {
            DoFameMedalBuyButton.SetActive(true);
        } else
        {
            DoFameMedalBuyButton.SetActive(false);
        }

        if (player.hasRichHelpPoorBonus && !player.richHelpPoorBonusMadeThisRound)
        {
            RichHelpPoorBonusButton.SetActive(true);
        }
        else
        {
            RichHelpPoorBonusButton.SetActive(false);
        }
    }

    void DrawFameMedalPieces()
    {
        fameMedalPiecesText.text = player.GetFameMedalPieces().ToString();
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
                    if (option.token is BuildableToken)
                    {
                        var buildTok = (BuildableToken)option.token;
                        var map = MapObject.GetComponent<MapScript>().map;
                        dropdown.SetOptionInteractable(option, buildTok.CanBeBuildByPlayer(player, map, players));
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

    void BuildTokenBtnPressed(Token token)
    {
        CloseAllDropDowns();
        app.Notify(SFNotification.HUD_build_token_btn_clicked, this, new object[] { token });
    }

    public void NextButtonClicked()
    {
        app.Notify(SFNotification.next_button_clicked, this);
    }

    void BuildUpgradeBtnPressed(Token token)
    {
        CloseAllDropDowns();
        app.Notify(SFNotification.HUD_build_upgrade_btn_clicked, this, new object[] { token });
    }

    public void SettleButtonPressed()
    {
        app.Notify(SFNotification.settle_button_clicked, this);
    }

    public void ShowFriendshipCardSelection(TradeStation tradeStation, AbstractFriendshipCard[] cards)
    {
        friendShipCardSelection.GetComponent<FriendShipCardSelector>().SetCards(cards);
        friendShipCardSelection.GetComponent<FriendShipCardSelector>().SetTradeStation(tradeStation);
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
        } else
        {
            tradePanel.SetActive(true);
        }
    }

    public void AddOre()
    {
        
    }

    public void AddFood()
    {
        
    }

    public void AddGoods()
    {
        
    }

    public void AddFuel()
    {
        
    }

    public void AddCarbon()
    {
        
    }

    public override void OnNotification(string p_event_path, Object p_target, params object[] p_data)
    {
        if (isReceivingNotifications)
        {
            switch(p_event_path)
            {
                //case SFNotification.player_data_changed:
                //    OnPlayerDataChanged();
                //    break;

                case SFNotification.open_friendship_card_selection:
                    var tradeStation = (TradeStation)p_data[0];
                    var cards = (AbstractFriendshipCard[])p_data[1];
                    ShowFriendshipCardSelection(tradeStation, cards);
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

    public void SubjectDataChanged(object[] data)
    {
        OnPlayerDataChanged();
    }
}
