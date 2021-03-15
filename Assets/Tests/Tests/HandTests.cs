using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class HandTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void CanPayCostTest1()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            Assert.True(hand.CanPayCost(new Cost(new Resource[] { new FuelResource(), new FuelResource() })));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CanPayCostTest2()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            Assert.False(hand.CanPayCost(new Cost(new Resource[] { new CarbonResource(), new FuelResource() })));
        }

        [Test]
        public void CanPayCostTest3()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());
            Assert.True(hand.CanPayCost(new Cost(new Resource[] { })));
        }

        [Test]
        public void RemoveCardTest1()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            hand.RemoveCardOfType(new FuelCard().GetType());
            Assert.True(hand.Count() == 1);
        }

        [Test]
        public void RemoveCardTest2()
        {
            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            try
            {
                hand.RemoveCardOfType(new FoodCard().GetType());
                Assert.True(false);
            } catch (ArgumentException e)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void SubtractHandTest1()
        {
            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            Hand subtract = new Hand();
            subtract.AddCard(new FoodCard());
            subtract.AddCard(new FoodCard());

            try
            {
                hand.SubtractHand(subtract);
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void SubtractHandTest2()
        {
            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            Hand subtract = new Hand();
            subtract.AddCard(new FuelCard());
            subtract.AddCard(new FuelCard());

            try
            {
                hand.SubtractHand(subtract);
                Assert.True(true);
                Assert.AreEqual(0, hand.Count());
            }
            catch (ArgumentException e)
            {
                Assert.True(false);
            }
        }

        [Test]
        public void PayCostTest1()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            hand.PayCost(new Cost(new Resource[] { }));
            var groupedResources = hand.GetGroupedResources();
            Assert.True(groupedResources[new FuelCard().resource.Name] == 2);
        }

        [Test]
        public void PayCostTest2()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            hand.PayCost(new Cost(new Resource[] { new FuelResource() }));
            var groupedResources = hand.GetGroupedResources();
            Assert.True(groupedResources[new FuelCard().resource.Name] == 1);
        }

        [Test]
        public void PayCostTest3()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            hand.PayCost(new Cost(new Resource[] { new FuelResource(), new FuelResource() }));
            var groupedResources = hand.GetGroupedResources();
            Assert.False(groupedResources.ContainsKey(new FuelCard().resource.Name));
        }

        [Test]
        public void PayCostTest4CannotPay()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new FuelCard());
            hand.AddCard(new FuelCard());

            try
            {
                hand.PayCost(new Cost(new Resource[] { new FuelResource(), new FuelResource(), new FuelResource() }));
                Assert.True(false);
            } catch (NotEnoughResourcesException e)
            {
                Assert.True(true);
            }
        }

        [Test]
        public void PayCostTest5CannotPay()
        {

            // Use the Assert class to test conditions
            Hand hand = new Hand();
            hand.AddCard(new CarbonCard());
            hand.AddCard(new CarbonCard());

            try
            {
                hand.PayCost(new Cost(new Resource[] { new FuelResource(), new FuelResource(), new FuelResource() }));
                Assert.True(false);
            }
            catch (NotEnoughResourcesException e)
            {
                Assert.True(true);
            }
        }
    }
}
