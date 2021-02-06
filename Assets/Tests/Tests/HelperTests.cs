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
        public void TestSpacePointArrayContainsPointPositive()
        {
            SpacePoint[] spArray = new SpacePoint[] { new SpacePoint(new HexCoordinates(0, 1), 1) };
            var searchedPoint = new SpacePoint(new HexCoordinates(0, 1), 1);
            Assert.True(helper.SpacePointArrayContainsPoint(spArray, searchedPoint));
        }

        [Test]
        public void TestSpacePointArrayContainsPointNegative()
        {
            SpacePoint[] spArray = new SpacePoint[] { new SpacePoint(new HexCoordinates(0, 1), 1) };
            var searchedPoint = new SpacePoint(new HexCoordinates(0, 1), 0);
            Assert.False(helper.SpacePointArrayContainsPoint(spArray, searchedPoint));
        }

        [Test]
        public void HelperTestsDistanceBetweenHexCoords()
        {
            Assert.AreEqual(new HexCoordinates(0, 0).DistanceTo(new HexCoordinates(-1, 2)), 2);
            Assert.AreEqual(new HexCoordinates(0, 0).DistanceTo(new HexCoordinates(1, 2)), 3);
        }

        [Test]
        public void CreateSpacePointArrayFromShortRepresentationNegativeTest()
        {
            string[] shortRepresentation = new string[] { "1,1,0", "1,2,2" };
            try
            {
                // this should fail, since we can't have vertexNumber 2
                SpacePoint[] points = helper.SpacePointArrayFromShortRep(shortRepresentation);
                // if it comes to this point than the creation succeeded, which should NOT happen
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                Assert.True(true);
            }
            
        }

        

        [Test]
        public void CreateSpacePointArrayFromShortRepresentationPositiveTest()
        {
            string[] shortRepresentation = new string[] { "1,1,0", "1,2,1" };
            SpacePoint[] points = helper.SpacePointArrayFromShortRep(shortRepresentation);
            Assert.True(points[0].Equals(new SpacePoint(new HexCoordinates(1, 1), 0)));
            Assert.True(points[1].Equals(new SpacePoint(new HexCoordinates(1, 2), 1)));
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
