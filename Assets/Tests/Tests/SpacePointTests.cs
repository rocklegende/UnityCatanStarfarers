using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SpacePointTests
    {
        [Test]
        public void SpacePointTestsDistance()
        {
            Assert.AreEqual(new SpacePoint(new HexCoordinates(0, 0), 0).DistanceTo(new SpacePoint(new HexCoordinates(0, 0), 0)), 0);

            Assert.AreEqual(new SpacePoint(new HexCoordinates(0, 0), 0).DistanceTo(new SpacePoint(new HexCoordinates(0, 0), 1)), 1);

            Assert.AreEqual(new SpacePoint(new HexCoordinates(0, 0), 0).DistanceTo(new SpacePoint(new HexCoordinates(0, 1), 0)), 2);

            Assert.AreEqual(new SpacePoint(new HexCoordinates(1, 1), 0).DistanceTo(new SpacePoint(new HexCoordinates(0, 1), 1)), 3);

            Assert.AreEqual(new SpacePoint(new HexCoordinates(0, 1), 0).DistanceTo(new SpacePoint(new HexCoordinates(1, 1), 1)), 1);

            Assert.AreEqual(new SpacePoint(new HexCoordinates(4, 0), 0).DistanceTo(new SpacePoint(new HexCoordinates(0, 4), 0)), 8);

            Assert.AreEqual(new SpacePoint(new HexCoordinates(0, 1), 0).DistanceTo(new SpacePoint(new HexCoordinates(0, 2), 1)), 3);

            Assert.AreEqual(new SpacePoint(new HexCoordinates(1, 0), 0).DistanceTo(new SpacePoint(new HexCoordinates(-1, 2), 1)), 5);

            Assert.AreEqual(new SpacePoint(new HexCoordinates(1, 0), 1).DistanceTo(new SpacePoint(new HexCoordinates(-1, 2), 0)), 3);
        }

        [Test]
        public void TestIfEqualsWorks()
        {

        }

    }
}
