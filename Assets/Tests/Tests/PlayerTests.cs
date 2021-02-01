using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void BuildSpacePortNegative()
        {
            var player = new Player(Color.black);
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
            var player = new Player(Color.black);
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
            var player = new Player(Color.black);
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
            var player = new Player(Color.black);
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
            var player = new Player(Color.black);
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
            var player = new Player(Color.black);
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




    }
}
