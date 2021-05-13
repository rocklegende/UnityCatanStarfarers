﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using com.onebuckgames.UnityStarFarers;

namespace Tests
{
    public class PlayerTests
    {

        Player GetGenericPlayer()
        {
            return new Player(new SFColor(Color.black));
        }

        int VpFromTokens(Token[] tokens)
        {
            var player = GetGenericPlayer();
            foreach(var tok in tokens)
            {
                player.tokens.Add(tok);
            }
            return player.GetVictoryPointsFromTokens();
        }

        [Test]
        public void AddCardTypeToPlayer()
        {
            var player = GetGenericPlayer();
            player.AddCard(CardType.CARBON);
            Assert.AreEqual(1, player.hand.NumberCardsOfType<CarbonCard>());

            player.AddCard(CardType.GOODS);
            Assert.AreEqual(1, player.hand.NumberCardsOfType<GoodsCard>());

            player.AddCard(CardType.FUEL);
            Assert.AreEqual(1, player.hand.NumberCardsOfType<FuelCard>());

            player.AddCard(CardType.ORE);
            Assert.AreEqual(1, player.hand.NumberCardsOfType<OreCard>());

            player.AddCard(CardType.FOOD);
            Assert.AreEqual(1, player.hand.NumberCardsOfType<FoodCard>());
        }

        [Test]
        public void GetVictoryPointsFromBeatenPirateTokens()
        {
            var player = GetGenericPlayer();
            player.AddPirateTokenBeatenAward();
            player.AddPirateTokenBeatenAward();
            Assert.AreEqual(2, player.GetVictoryPoints());
        }

        [Test]
        public void GetVictoryPointsTestSettledColony()
        {
            int vp = VpFromTokens(new Token[] { new ColonyBaseToken() });
            Assert.AreEqual(1, vp);
        }

        [Test]
        public void GetVictoryPointsTestUnsettledColony()
        {
            var token = new ColonyBaseToken();
            token.attachToken(new ShipToken());
            int vp = VpFromTokens(new Token[] { token });
            Assert.AreEqual(0, vp);
        }

        [Test]
        public void GetVictoryPointsTestSettleTradeShip()
        {
            int vp = VpFromTokens(new Token[] { new TradeBaseToken() });
            Assert.AreEqual(1, vp);
        }

        [Test]
        public void GetVictoryPointsTestUnsettledTradeShip()
        {
            var token = new TradeBaseToken();
            token.attachToken(new ShipToken());
            int vp = VpFromTokens(new Token[] { token });
            Assert.AreEqual(0, vp);
        }

        [Test]
        public void GetVictoryPointsTestColonyWithSpacePort()
        {
            var token = new ColonyBaseToken();
            token.attachToken(new SpacePortToken());
            int vp = VpFromTokens(new Token[] { token });
            Assert.AreEqual(2, vp);
        }

        [Test]
        public void TestFlyableTokens()
        {
            var player = GetGenericPlayer();

            var spacePortToken = new SpacePortToken();

            var settledColonyToken = new ColonyBaseToken();

            var tradeWithShip = new TradeBaseToken(); // only this one can fly
            tradeWithShip.attachToken(new ShipToken());

            var tradeWithShipDisabled = new TradeBaseToken();
            tradeWithShipDisabled.attachToken(new ShipToken());
            tradeWithShipDisabled.Disable();

            player.AddToken(spacePortToken);
            player.AddToken(settledColonyToken);
            player.AddToken(tradeWithShip);

            var expected = new List<Token>() { tradeWithShip };
            Assert.True(expected.SequenceEqual(player.GetTokensThatCanFly()));

        }

        [Test]
        public void BuildSpacePortNegativeNotEnoughResources()
        {
            var map = new DefaultMapGenerator().GenerateRandomMap();
            var player = GetGenericPlayer();
            player.AddCard(new FuelCard());

            var token = new SpacePortToken();

            try
            {
                player.BuildToken(map, token.GetType(), SpacePoint.Zero);
                Assert.True(false);
            } catch (NotEnoughResourcesException e)
            {
                Assert.True(true);
            }

        }

        [Test]
        public void BuildSpacePortPositiveEnoughResources()
        {
            var map = new DefaultMapGenerator().GenerateRandomMap();
            var player = GetGenericPlayer();
            player.AddCard(new FuelCard());
            player.AddCard(new CarbonCard());
            player.AddCard(new CarbonCard());
            player.AddCard(new CarbonCard());
            player.AddCard(new FoodCard());
            player.AddCard(new FoodCard());

            var token = new SpacePortToken();

            try
            {
                player.BuildToken(map, token.GetType(), SpacePoint.Zero);
                Assert.True(player.hand.NumberCardsOfType<FuelCard>() == 1);
                Assert.True(player.hand.NumberCardsOfType<CarbonCard>() == 0);
                Assert.True(true);
            }
            catch (NotEnoughResourcesException e)
            {
                Assert.True(false);
            }
        }


        // A Test behaves as an ordinary method
        [Test]
        public void BuildTradeShipNegative()
        {
            var map = new DefaultMapGenerator().GenerateRandomMap();
            var player = GetGenericPlayer();
            player.AddCard(new FuelCard());

            var token = new TradeBaseToken();

            try
            {
                player.BuildToken(map, token.GetType(), SpacePoint.Zero);
                Assert.True(false);
            }
            catch (NotEnoughResourcesException e)
            {
                Assert.True(true);
            }

        }

        [Test]
        public void BuildTradeShipPositive()
        {
            var map = new DefaultMapGenerator().GenerateRandomMap();
            var player = GetGenericPlayer();
            player.AddCard(new OreCard());
            player.AddCard(new FuelCard());
            player.AddCard(new GoodsCard());
            player.AddCard(new GoodsCard());

            var token = new TradeBaseToken();

            try
            {
                player.BuildToken(map, token.GetType(), SpacePoint.Zero);
                Assert.True(player.hand.NumberCardsOfType<FuelCard>() == 0);
                Assert.True(player.hand.NumberCardsOfType<GoodsCard>() == 0);
                Assert.True(player.hand.NumberCardsOfType<OreCard>() == 0);
                Assert.True(true);
            }
            catch (NotEnoughResourcesException e)
            {
                Assert.True(false);
            }
        }

        // A Test behaves as an ordinary method
        [Test]
        public void BuildColonyShipNegative()
        {
            var map = new DefaultMapGenerator().GenerateRandomMap();
            var player = GetGenericPlayer();
            player.AddCard(new FuelCard());

            var token = new ColonyBaseToken();

            try
            {
                player.BuildToken(map, token.GetType(), SpacePoint.Zero);
                Assert.True(false);
            }
            catch (NotEnoughResourcesException e)
            {
                Assert.True(true);
            }

        }

        [Test]
        public void BuildColonyShipPositive()
        {
            var map = new DefaultMapGenerator().GenerateRandomMap();
            var player = GetGenericPlayer();
            player.AddCard(new OreCard());
            player.AddCard(new FuelCard());
            player.AddCard(new CarbonCard());
            player.AddCard(new FoodCard());

            var token = new ColonyBaseToken();

            try
            {
                player.BuildToken(map, token.GetType(), SpacePoint.Zero);
                Assert.True(player.hand.NumberCardsOfType<FuelCard>() == 0);
                Assert.True(player.hand.NumberCardsOfType<GoodsCard>() == 0);
                Assert.True(player.hand.NumberCardsOfType<OreCard>() == 0);
                Assert.True(player.hand.NumberCardsOfType<FoodCard>() == 0);
                Assert.True(true);
            }
            catch (NotEnoughResourcesException e)
            {
                Assert.True(false);
            }
        }

        [Test]
        public void BuildColonyAtPosition()
        {
            var player = GetGenericPlayer();
            player.AddCard(new OreCard());
            player.AddCard(new FuelCard());
            player.AddCard(new CarbonCard());
            player.AddCard(new FoodCard());

            var mockMap = new Map(new Tile_[,] { });

            var position = new SpacePoint(new HexCoordinates(0, 0), 1);

            try
            {
                var token = player.BuildToken(mockMap, new ColonyBaseToken().GetType(), position);
                Assert.True(token.associatedMap == mockMap);
                Assert.True(position.IsEqualTo(token.position));
            }
            catch (NotEnoughResourcesException e)
            {
                Assert.True(false);
            }
        }

        [Test]
        public void TestGetVPFromFameMedals2()
        {
            var player = GetGenericPlayer();
            player.AddFameMedal();
            player.RemoveFameMedal();

            Assert.True(player.GetVictoryPointsFromFameMedals() == 0);
        }

        [Test]
        public void TestGetVPFromFameMedals3()
        {
            var player = GetGenericPlayer();
            player.AddFameMedal();
            player.RemoveFameMedal();
            player.RemoveFameMedal();

            Assert.True(player.GetVictoryPointsFromFameMedals() == 0);
        }

        [Test]
        public void TestFameMedalBuyCanOnlyBeDoneOnce()
        {
            var player = GetGenericPlayer();
            player.hand.AddCard(new GoodsCard());
            player.hand.AddCard(new GoodsCard());
            player.AllowFameMedalBuy();
            player.BuyFameMedal();
            try
            {
                player.BuyFameMedal();
                Assert.True(false);
            } catch (System.ArgumentException e)
            {
                Assert.True(true);
            }

            
        }

        [Test]
        public void TestFameMedalBuyResetsOnNewTurn()
        {
            var player = GetGenericPlayer();
            player.hand.AddCard(new GoodsCard());
            player.hand.AddCard(new GoodsCard());
            player.AllowFameMedalBuy();
            player.BuyFameMedal();
            player.OnTurnReceived();
            try
            {
                player.BuyFameMedal();
                Assert.True(true); //should be possible again
            }
            catch (System.ArgumentException e)
            {
                Assert.True(false);
            }


        }

        [Test]
        public void AddingTokenSetsCorrectOwner()
        {
            var player = GetGenericPlayer();
            var token = new ColonyBaseToken();
            player.AddToken(token);
            Assert.True(token.owner == player);
        }

        [Test]
        public void TestTokenDisabilityIsRemovedOnNewTurn()
        {
            var token = new ColonyBaseToken();
            token.Disable();

            var player = GetGenericPlayer();
            player.AddToken(token);
            player.OnTurnReceived();
            Assert.True(!token.IsDisabled());
        }

        [Test]
        public void TokenStorageTest1()
        {
            var storage = new TokenStorage();
            var spacePortCount = storage.GetTokensOfType(new SpacePortToken().GetType()).Length;
            Assert.AreEqual(3, spacePortCount);
        }

        [Test]
        public void TokenStorageTestAddingAndRetrieving()
        {
            var storage = new TokenStorage();
            var spacePortCount = storage.GetTokensOfType(new SpacePortToken().GetType()).Length;
            Assert.AreEqual(3, spacePortCount);

            storage.AddToken(new SpacePortToken());
            spacePortCount = storage.GetTokensOfType(new SpacePortToken().GetType()).Length;
            Assert.AreEqual(4, spacePortCount);

            var spacePort = storage.RetrieveTokenOfType(new SpacePortToken().GetType());
            Assert.True(spacePort is SpacePortToken);
            spacePortCount = storage.GetTokensOfType(new SpacePortToken().GetType()).Length;
            Assert.AreEqual(3, spacePortCount);
        }

        

        [Test]
        public void TestGetRemovableUpgradeTokens()
        {
            var player = new TestHelper().CreateGenericPlayer();
            player.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
            player.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
            player.BuildUpgradeWithoutCost(new CannonUpgradeToken());

            var removableTokens = player.ship.GetRemovableUpgrades();
            Assert.True(removableTokens.Count == 2);
            Assert.True(removableTokens.Find(tok => tok is BoosterUpgradeToken) != null);
            Assert.True(removableTokens.Find(tok => tok is CannonUpgradeToken) != null);
        }



        [Test]
        public void TestGetAttachableUpgradeTokens()
        {
            var player = new TestHelper().CreateGenericPlayer();
            var tokens = player.ship.GetUpgradesThatAreNotFull();
            Assert.True(tokens.Count == 3);

            for (int i = 0; i < player.ship.BoostersMaxCapacity; i++)
            {
                player.BuildUpgradeWithoutCost(new BoosterUpgradeToken());
            }

            tokens = player.ship.GetUpgradesThatAreNotFull();
            Assert.True(tokens.Find(tok => tok is BoosterUpgradeToken) == null); //Boosters are full, they cant be in the list anymore
        }

        [Test]
        public void PayToOtherPlayer()
        {
            var players = TestHelper.CreateGenericPlayers3();
            players[0].AddHand(Hand.FromResources(3, 2, 1));
            players[1].AddHand(Hand.FromResources(3, 2, 3));

            players[0].PayToOtherPlayer(players[1], Hand.FromResources(1));

            Assert.True(players[0].hand.HasSameCardsAs(Hand.FromResources(2, 2, 1)));
            Assert.True(players[1].hand.HasSameCardsAs(Hand.FromResources(4, 2, 3)));
        }

        [Test]
        public void BuildingColonyCorrectlyAssignedColor()
        {
            var map = new DefaultMapGenerator().GenerateRandomMap();
            var player = new TestHelper().CreateGenericPlayer();
            player.AddHand(Hand.FromResources(2, 2, 2, 2, 2));
            var buildedToken = player.BuildToken(map, new ColonyBaseToken().GetType(), new SpacePoint(0, 0, 0), new ShipToken().GetType());
            Assert.True(buildedToken.GetColor().Equals(player.color));
            Assert.True(buildedToken.attachedToken.GetColor().Equals(player.color));
        }
    }
}
