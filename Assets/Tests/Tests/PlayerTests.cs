using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerTests
    {

        Player GetGenericPlayer()
        {
            return new Player(Color.black, new SFElement());
        }

        void AddCashToPlayer(Player player, int amount = 5)
        {
            for (int i = 0; i < amount; i++)
            {
                player.AddCard(new GoodsCard());
                player.AddCard(new FuelCard());
                player.AddCard(new CarbonCard());
                player.AddCard(new FoodCard());
                player.AddCard(new OreCard());
            }
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
        public void BuildSpacePortNegative()
        {
            var player = GetGenericPlayer();
            player.AddCard(new FuelCard());

            var token = new SpacePortToken();

            try
            {
                player.BuildToken(token);
                Assert.True(false);
            } catch (NotEnoughResourcesException e)
            {
                Assert.True(true);
            }

        }

        [Test]
        public void BuildSpacePortPositive()
        {
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
                player.BuildToken(token);
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
            var player = GetGenericPlayer();
            player.AddCard(new FuelCard());

            var token = new TradeBaseToken();

            try
            {
                player.BuildToken(token);
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
            var player = GetGenericPlayer();
            player.AddCard(new OreCard());
            player.AddCard(new FuelCard());
            player.AddCard(new GoodsCard());
            player.AddCard(new GoodsCard());

            var token = new TradeBaseToken();

            try
            {
                player.BuildToken(token);
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
            var player = GetGenericPlayer();
            player.AddCard(new FuelCard());

            var token = new ColonyBaseToken();

            try
            {
                player.BuildToken(token);
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
            var player = GetGenericPlayer();
            player.AddCard(new OreCard());
            player.AddCard(new FuelCard());
            player.AddCard(new CarbonCard());
            player.AddCard(new FoodCard());

            var token = new ColonyBaseToken();

            try
            {
                player.BuildToken(token);
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




    }
}
