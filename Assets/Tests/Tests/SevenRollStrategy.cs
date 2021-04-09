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
            var order = Helper.NextPlayersClockwise(2, 4);
            var expectedOrder = new List<int>() { 2, 3, 0, 1 };

            Assert.True(order.SequenceEqual(expectedOrder));
        }

        [Test]
        public void CheckNextPlayers2()
        {
            var order = Helper.NextPlayersClockwise(0, 4); 
            var expectedOrder = new List<int>() { 0, 1, 2, 3 };

            Assert.True(order.SequenceEqual(expectedOrder));
        }

        [Test]
        public void CheckNextPlayer3()
        {
            var nextPlayer = Helper.NextPlayerClockwiseStepsAway(0, 4, 2);
            Assert.AreEqual(2, nextPlayer);
        }

        [Test]
        public void CheckNextPlayer4()
        {
            var nextPlayer = Helper.NextPlayerClockwiseStepsAway(3, 4, 2);
            Assert.AreEqual(1, nextPlayer);
        }

        [Test]
        public void CheckPreviousPlayer1()
        {
            var previous = Helper.PreviousPlayerClockwise(0, 4);
            Assert.AreEqual(3, previous);
        }

        [Test]
        public void CheckPreviousPlayer2()
        {
            var previous = Helper.PreviousPlayerClockwiseStepsAway(0, 4, 3);
            Assert.AreEqual(1, previous);
        }

        [Test]
        public void CheckPreviousPlayer3()
        {
            var previous = Helper.PreviousPlayerClockwiseStepsAway(0, 4, 7);
            Assert.AreEqual(1, previous);
        }

    }
}
