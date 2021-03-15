﻿using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TradingCalculatorTests
    {

        Player GetGenericPlayer()
        {
            return new Player(Color.black, new SFElement());
        }

        Hand GetHandWithResources(int food = 0, int goods = 0, int fuel = 0, int ore = 0, int carbon = 0)
        {
            var hand = new Hand();
            for (int i = 0; i < food; i++)
            {
                hand.AddCard(new FoodCard());
            }

            for (int i = 0; i < goods; i++)
            {
                hand.AddCard(new GoodsCard());
            }

            for (int i = 0; i < ore; i++)
            {
                hand.AddCard(new OreCard());
            }

            for (int i = 0; i < fuel; i++)
            {
                hand.AddCard(new FuelCard());
            }

            for (int i = 0; i < carbon; i++)
            {
                hand.AddCard(new CarbonCard());
            }
            return hand;

        }

        // A Test behaves as an ordinary method
        [Test]
        public void GetOutputFromFood1()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(2);
            var output = tc.GetTradeOutputFromFood(tradingInput);
            Assert.AreEqual(0, output);
        }

        [Test]
        public void GetOutputFromFood2()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(3);
            var output = tc.GetTradeOutputFromFood(tradingInput);
            Assert.AreEqual(1, output);
        }

        [Test]
        public void GetOutputFromFood3()
        {
            Player player = GetGenericPlayer();
            player.ChangeFoodTradingRatio(2);
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(4);
            var output = tc.GetTradeOutputFromFood(tradingInput);
            Assert.AreEqual(2, output);
        }

        [Test]
        public void GetOutputFromGoods0()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 1);
            var output = tc.GetTradeOutputFromGoods(tradingInput);
            Assert.AreEqual(0, output);
        }

        [Test]
        public void GetOutputFromGoods1()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 2);
            var output = tc.GetTradeOutputFromGoods(tradingInput);
            Assert.AreEqual(1, output);
        }

        [Test]
        public void GetOutputFromGoods2()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 3);
            var output = tc.GetTradeOutputFromGoods(tradingInput);
            Assert.AreEqual(1, output);
        }

        [Test]
        public void GetOutputFromGoods3()
        {
            Player player = GetGenericPlayer();
            player.ChangeGoodsTradingRatio(1);
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 4);
            var output = tc.GetTradeOutputFromGoods(tradingInput);
            Assert.AreEqual(4, output);
        }

        [Test]
        public void GetOutputFromFuel1()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 2);
            var output = tc.GetTradeOutputFromFuel(tradingInput);
            Assert.AreEqual(0, output);
        }

        [Test]
        public void GetOutputFromFuel2()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 3);
            var output = tc.GetTradeOutputFromFuel(tradingInput);
            Assert.AreEqual(1, output);
        }

        [Test]
        public void GetOutputFromFuel3()
        {
            Player player = GetGenericPlayer();
            player.ChangeFuelTradingRatio(2);
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 4);
            var output = tc.GetTradeOutputFromFuel(tradingInput);
            Assert.AreEqual(2, output);
        }

        [Test]
        public void GetOutputFromCarbon1()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 0, 0, 2);
            var output = tc.GetTradeOutputFromCarbon(tradingInput);
            Assert.AreEqual(0, output);
        }

        [Test]
        public void GetOutputFromCarbon2()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 0, 0, 3);
            var output = tc.GetTradeOutputFromCarbon(tradingInput);
            Assert.AreEqual(1, output);
        }

        [Test]
        public void GetOutputFromCarbon3()
        {
            Player player = GetGenericPlayer();
            player.ChangeCarbonTradingRatio(2);
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 0, 0, 4);
            var output = tc.GetTradeOutputFromCarbon(tradingInput);
            Assert.AreEqual(2, output);
        }

        [Test]
        public void GetOutputFromOre1()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 0, 2);
            var output = tc.GetTradeOutputFromOre(tradingInput);
            Assert.AreEqual(0, output);
        }

        [Test]
        public void GetOutputFromOre2()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 0, 3);
            var output = tc.GetTradeOutputFromOre(tradingInput);
            Assert.AreEqual(1, output);
        }

        [Test]
        public void GetOutputFromOre3()
        {
            Player player = GetGenericPlayer();
            player.ChangeOreTradingRatio(2);
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(0, 0, 0, 4);
            var output = tc.GetTradeOutputFromOre(tradingInput);
            Assert.AreEqual(2, output);
        }

        [Test]
        public void TestIsOutPossibleZeroCase()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources();
            var tradingOutput = GetHandWithResources(3);
            Assert.False(tc.IsOutputPossible(tradingInput, tradingOutput));
        }

        [Test]
        public void TestIsOutputPossibleCasePositive1()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(2, 3, 4, 4, 1);
            var tradingOutput = GetHandWithResources(3);
            Assert.True(tc.IsOutputPossible(tradingInput, tradingOutput));
        }

        [Test]
        public void TestIsOutputPossibleCaseNegative1()
        {
            Player player = GetGenericPlayer();
            TradingCalculator tc = new TradingCalculator(player);
            var tradingInput = GetHandWithResources(2, 3, 4, 4, 1);
            var tradingOutput = GetHandWithResources(5);
            Assert.False(tc.IsOutputPossible(tradingInput, tradingOutput));
        }


    }
}