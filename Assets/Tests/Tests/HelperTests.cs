using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class HelperTests
    {
        public Helper helper = new Helper();

        [Test]
        public void HelperTestsDistanceBetweenHexCoords()
        {
            Assert.AreEqual(new HexCoordinates(0, 0).DistanceTo(new HexCoordinates(-1, 2)), 2);
            Assert.AreEqual(new HexCoordinates(0, 0).DistanceTo(new HexCoordinates(1, 2)), 3);
        }

        

        [Test]
        public void HelperTestsGetHexesAtCityPositionWithVertex1()
        {
            SpacePoint point = new SpacePoint(new HexCoordinates(4, 2), 1);
            HexCoordinates[] hexCoordsAtPoint = helper.getCoordinatesOfHexesAtPoint(point);
            HexCoordinates[] expectedHexCoords = new HexCoordinates[] { new HexCoordinates(3, 2), new HexCoordinates(4, 1), new HexCoordinates(4, 2) };

            Assert.True(helper.HexCoordinateGroupsAreEqual(hexCoordsAtPoint, expectedHexCoords));
        }

        [Test]
        public void HelperTestsGetHexesAtCityPositionWithVertex0()
        {
            SpacePoint point = new SpacePoint(new HexCoordinates(2, 2), 0);
            HexCoordinates[] hexCoordsAtPoint = helper.getCoordinatesOfHexesAtPoint(point);
            HexCoordinates[] expectedHexCoords = new HexCoordinates[] { new HexCoordinates(2, 1), new HexCoordinates(3, 1), new HexCoordinates(2, 2) };

            Assert.True(helper.HexCoordinateGroupsAreEqual(hexCoordsAtPoint, expectedHexCoords));
        }

        [Test]
        public void HelperTestsGetAllHexesDistanceAwayFromGivenHex()
        {
            HexCoordinates origin = new HexCoordinates(1, 0);
            HexCoordinates[] twoAway = helper.getAllHexesDistanceAwayFromHex(origin, 2);

            HexCoordinates[] expectedCoordinates = new HexCoordinates[]
            {
                new HexCoordinates(-1, 0),
                new HexCoordinates(-1, 1),
                new HexCoordinates(-1, 2),

                new HexCoordinates(0, 2),
                new HexCoordinates(1, 2),
                new HexCoordinates(2, 1),

                new HexCoordinates(3, 0),
                new HexCoordinates(3, -1),
                new HexCoordinates(3, -2),

                new HexCoordinates(2, -2),
                new HexCoordinates(1, -2),
                new HexCoordinates(0, -1),
            };

            Assert.True(helper.HexCoordinateGroupsAreEqual(twoAway, expectedCoordinates));
            Assert.True(helper.HexCoordinateGroupsAreEqual(helper.getAllHexesDistanceAwayFromHex(origin, 0), new HexCoordinates[] { origin }));
        }
    }
}
