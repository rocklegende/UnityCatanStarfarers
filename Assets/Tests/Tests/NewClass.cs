using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

namespace Tests
{
    public class SevenRollStrategy
    {
        [Test]
        public void CheckNextPlayers1()
        {
            var strat = new On7RollStrategy();
            var order = strat.NextPlayersToActClockwise(2, 4);
            var expectedOrder = new List<int>() { 2, 3, 0, 1 };

            Assert.True(order.SequenceEqual(expectedOrder));
        }

        [Test]
        public void CheckNextPlayers2()
        {
            var strat = new On7RollStrategy();
            var order = strat.NextPlayersToActClockwise(0, 4); 
            var expectedOrder = new List<int>() { 0, 1, 2, 3 };

            Assert.True(order.SequenceEqual(expectedOrder));
        }

    }
}
