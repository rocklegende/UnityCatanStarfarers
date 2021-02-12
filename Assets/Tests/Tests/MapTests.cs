using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class MapTests
    {
        private Helper helper = new Helper();

        private Map CreateSampleMap3x3()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap4x3()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMapWithObstacles()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new EmptyTile(), new FoodResourceTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMapWithObstacles2()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile()  },
                { new BorderTile(), new EmptyTile(), new FoodResourceTile(), new EmptyTile(),  new BorderTile() },
                { new BorderTile(), new EmptyTile(), new FoodResourceTile(), new EmptyTile(), new BorderTile() },
                { new FoodResourceTile(), new EmptyTile(), new EmptyTile(), new EmptyTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMapWithObstacles3()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile()  },
                { new BorderTile(), new BorderTile(), new FoodResourceTile(), new EmptyTile(),  new BorderTile() },
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap4x4()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap4x5()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap5x6()
        {
            Tile_[,] mapRepresentation = {
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile(), new BorderTile(), },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile()},
            };

            return new Map(mapRepresentation);
        }


        private bool spacePointArraysAreEqual(SpacePoint[] spacePoints1, SpacePoint[] spacePoints2)
        {
            if (spacePoints1.Length != spacePoints2.Length)
            {
                return false;
            }
            Array.Sort(spacePoints1, new SpacePointComparer());
            Array.Sort(spacePoints2, new SpacePointComparer());

            for (int i = 0; i < spacePoints1.Length; i++)
            {
                if (!spacePoints1[i].Equals(spacePoints2[i]))
                {
                    return false;
                } 
            }

            return true;
        }

        private void printSpacePoints(SpacePoint[] spacePoints)
        {
            foreach(SpacePoint spacePoint in spacePoints)
            {
                spacePoint.print();
            }
        }

        [Test]
        public void MapGetNeighborsOfSpacePoint1()
        {
            Map map = CreateSampleMapWithObstacles();

            var origin = new SpacePoint(new HexCoordinates(0, 1), 0);
            SpacePoint[] neighbors = map.GetNeighborsOfSpacePoint(origin);

            var se_point = new SpacePoint(new HexCoordinates(1, 1), 1);
            var n_point = new SpacePoint(new HexCoordinates(1, 0), 1);
            var sw_point = new SpacePoint(new HexCoordinates(0, 1), 1);

            Assert.False(new Helper().SpacePointArrayContainsPoint(neighbors, se_point)); // should be blocked
            Assert.True(new Helper().SpacePointArrayContainsPoint(neighbors, n_point)); // should NOT be blocked
            Assert.True(new Helper().SpacePointArrayContainsPoint(neighbors, sw_point)); // should NOT be blocked
        }

        [Test]
        public void MapGetNeighborsOfSpacePoint2()
        {
            Map map = CreateSampleMapWithObstacles();

            var origin = new SpacePoint(new HexCoordinates(1, 1), 1);
            SpacePoint[] neighbors = map.GetNeighborsOfSpacePoint(origin);

            var s_point = new SpacePoint(new HexCoordinates(0, 2), 0);
            var ne_point = new SpacePoint(new HexCoordinates(1, 1), 0);
            var nw_point = new SpacePoint(new HexCoordinates(0, 1), 0);

            Assert.False(new Helper().SpacePointArrayContainsPoint(neighbors, s_point)); // should be blocked
            Assert.False(new Helper().SpacePointArrayContainsPoint(neighbors, ne_point)); // should be blocked
            Assert.False(new Helper().SpacePointArrayContainsPoint(neighbors, nw_point)); // should be blocked
        }

        [Test]
        public void MapGetNeighborsOfSpacePoint3()
        {
            Map map = CreateSampleMapWithObstacles();

            var origin = new SpacePoint(new HexCoordinates(0, 1), 1);
            SpacePoint[] neighbors = map.GetNeighborsOfSpacePoint(origin);

            var s_point = new SpacePoint(new HexCoordinates(-1, 2), 0);
            var ne_point = new SpacePoint(new HexCoordinates(0, 1), 0);
            var nw_point = new SpacePoint(new HexCoordinates(-1, 1), 0);

            Assert.True(new Helper().SpacePointArrayContainsPoint(neighbors, s_point)); // should be blocked
            Assert.True(new Helper().SpacePointArrayContainsPoint(neighbors, ne_point)); // should be blocked
            Assert.True(new Helper().SpacePointArrayContainsPoint(neighbors, nw_point)); // should be blocked
        }

        [Test]
        public void MapGetDistanceBetweenPoints1()
        {
            Map map = CreateSampleMapWithObstacles2();

            var origin = new SpacePoint(new HexCoordinates(1, 2), 1);
            var destination = new SpacePoint(new HexCoordinates(1, 2), 0);
            Assert.AreEqual(map.distanceBetweenPoints(origin, destination), 5);
            Assert.AreEqual(map.distanceBetweenPoints(destination, origin), 5);
        }

        [Test]
        public void MapGetDistanceBetweenPoints2()
        {
            Map map = CreateSampleMapWithObstacles2();

            var origin = new SpacePoint(new HexCoordinates(1, 1), 1);
            var destination = new SpacePoint(new HexCoordinates(1, 2), 0);
            Assert.AreEqual(map.distanceBetweenPoints(origin, destination), 3);
            Assert.AreEqual(map.distanceBetweenPoints(destination, origin), 3);
        }

        [Test]
        public void MapGetAllAvailableSpacePoints()
        {

            Map map = CreateSampleMap3x3();

            SpacePoint[] actualPoints = map.getAllAvailableSpacePoints();

            SpacePoint[] expectedPoints = new SpacePoint[]
            {
                new SpacePoint(new HexCoordinates(0, 1), 0),
                new SpacePoint(new HexCoordinates(0, 1), 1),
                new SpacePoint(new HexCoordinates(1, 1), 1),
                new SpacePoint(new HexCoordinates(0, 2), 0),
                new SpacePoint(new HexCoordinates(0, 2), 1),
                new SpacePoint(new HexCoordinates(-1, 2), 0)
            };

            Assert.True(spacePointArraysAreEqual(actualPoints, expectedPoints));
        }

        [Test]
        public void MapGetAllSpacePointsInDistance2()
        {

            Map map = CreateSampleMapWithObstacles3();

            SpacePoint origin = new SpacePoint(new HexCoordinates(1, 1), 0);
            SpacePoint[] spacePointsTwoStepsAway = map.GetSpacePointsInDistance(origin, 2);

            SpacePoint[] expectedPoints = new SpacePoint[]
            {
                new SpacePoint(new HexCoordinates(0, 2), 0),
                new SpacePoint(new HexCoordinates(1, 2), 0),
                new SpacePoint(new HexCoordinates(2, 1), 0),
            }; 

            Assert.True(spacePointArraysAreEqual(spacePointsTwoStepsAway, expectedPoints));
        }

        [Test]
        public void MapGetAllSpacePointsInDistance3()
        {

            Map map = CreateSampleMapWithObstacles3();
            SpacePoint origin = new SpacePoint(new HexCoordinates(2, 1), 0);

            SpacePoint[] spacePointsThreeStepsAway = map.GetSpacePointsInDistance(origin, 3);
            SpacePoint[] expectedPoints = new SpacePoint[]
            {
                new SpacePoint(new HexCoordinates(1, 1), 1),
                new SpacePoint(new HexCoordinates(2, 2), 1),
            };

            Assert.True(spacePointArraysAreEqual(spacePointsThreeStepsAway, expectedPoints));
        }

        [Test]
        public void MapGetSpacePointsInDistanceZeroCase()
        {

            Map map = CreateSampleMapWithObstacles3();
            SpacePoint origin = new SpacePoint(new HexCoordinates(1, 1), 0);

            SpacePoint[] spacePointsThreeStepsAway = map.GetSpacePointsInDistance(origin, 0);
            SpacePoint[] expectedPoints = new SpacePoint[]
            {
                new SpacePoint(new HexCoordinates(1, 1), 0),
            };

            Assert.True(spacePointArraysAreEqual(spacePointsThreeStepsAway, expectedPoints));
        }

        [Test]
        public void MapGetSpacePointsInsideRangeCase()
        {

            Map map = CreateSampleMapWithObstacles3();
            SpacePoint origin = new SpacePoint(new HexCoordinates(1, 2), 0);

            SpacePoint[] spacePointsThreeStepsAway = map.GetSpacePointsInsideRange(origin, 3, 2);
            SpacePoint[] expectedPoints = new SpacePoint[]
            {
                new SpacePoint(new HexCoordinates(1, 1), 0),
                new SpacePoint(new HexCoordinates(1, 1), 1),
                new SpacePoint(new HexCoordinates(2, 1), 0),
                new SpacePoint(new HexCoordinates(2, 2), 0),
                new SpacePoint(new HexCoordinates(1, 3), 0),
                new SpacePoint(new HexCoordinates(1, 3), 1),
                new SpacePoint(new HexCoordinates(3, 1), 1),
            };

            Assert.True(spacePointArraysAreEqual(spacePointsThreeStepsAway, expectedPoints));
        }



        [Test]
        public void MapGetOffset()
        {
            Map map = CreateSampleMap4x3();
            Assert.AreEqual(map.getOffset(), 1);
            map = CreateSampleMap4x4();
            Assert.AreEqual(map.getOffset(), 1);
            map = CreateSampleMap4x5();
            Assert.AreEqual(map.getOffset(), 2);
        }

        [Test]
        public void MapGetWidth()
        {
            Map map = CreateSampleMap4x3();
            Assert.AreEqual(map.width(), 4);
            map = CreateSampleMap5x6();
            Assert.AreEqual(map.width(), 5);
        }

        [Test]
        public void MapGetHeight()
        {
            Map map = CreateSampleMap4x3();
            Assert.AreEqual(map.height(), 3);
            map = CreateSampleMap5x6();
            Assert.AreEqual(map.height(), 6);
        }

        [Test]
        public void MapCoordAreInBounds()
        {
            Map map = CreateSampleMap4x3();
            Assert.False(map.coordsAreInBounds(new HexCoordinates(3, 0)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(2, 0)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(-1, 0)));
            Assert.False(map.coordsAreInBounds(new HexCoordinates(-2, 0)));


            Assert.False(map.coordsAreInBounds(new HexCoordinates(0, -1)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(0, 0)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(0, 1)));
            Assert.True(map.coordsAreInBounds(new HexCoordinates(0, 2)));
            Assert.False(map.coordsAreInBounds(new HexCoordinates(0, 3)));
        }

        [Test]
        public void MapHexesAtPointTest1()
        {
            Map map = CreateSampleMap4x4();

            SpacePoint point = new SpacePoint(new HexCoordinates(1, 0), 1);
            HexCoordinates[] validCoords = map.getValidHexCoordinatesAtPoint(point);
            HexCoordinates[] expected = new HexCoordinates[]
            {
                new HexCoordinates(0, 0),
                new HexCoordinates(1, 0)
            };

            Assert.True(helper.HexCoordinateGroupsAreEqual(validCoords, expected));
        }

        [Test]
        public void MapHexesAtPointTest2()
        {
            Map map = CreateSampleMap4x4();

            SpacePoint point = new SpacePoint(new HexCoordinates(0, 0), 1);
            HexCoordinates[] validCoords = map.getValidHexCoordinatesAtPoint(point);
            HexCoordinates[] expected = new HexCoordinates[]
            {
                new HexCoordinates(0, 0),
            };

            Assert.True(helper.HexCoordinateGroupsAreEqual(validCoords, expected));
        }

        [Test]
        public void MapHexesAtPointTest3()
        {
            Map map = CreateSampleMap4x4();

            SpacePoint point = new SpacePoint(new HexCoordinates(1, 0), 0);
            HexCoordinates[] validCoords = map.getValidHexCoordinatesAtPoint(point);
            HexCoordinates[] expected = new HexCoordinates[]
            {
                new HexCoordinates(1, 0),
            };

            Assert.True(helper.HexCoordinateGroupsAreEqual(validCoords, expected));
        }

        [Test]
        public void TestCoordsToArrayIndexes()
        {

            Map map = CreateSampleMap3x3(); //offset is one

            HexCoordinates coords = new HexCoordinates(-1, 3);
            (int, int) expectedIndexes = (3, 0);
            Assert.True(expectedIndexes == map.coordsToArrayIndexes(coords));


        }

        [Test]
        public void TestArrayIndexesToCoords1()
        {

            Map map = CreateSampleMap3x3(); //offset is one

            (int, int) indexes = (2, 2);
            HexCoordinates expectedCoords = new HexCoordinates(1, 2);

            Assert.True(expectedCoords.Equals(map.arrayIndexesToCoords(indexes)));


        }

        [Test]
        public void TestArrayIndexesToCoords2()
        {

            Map map = CreateSampleMap3x3(); //offset is one

            (int, int) indexes = (3, 0);
            HexCoordinates expectedCoords = new HexCoordinates(-1, 3);
            Assert.True(expectedCoords.Equals(map.arrayIndexesToCoords(indexes)));


        }


        [Test]
        public void TestSetTileGroupAtSpacePointPositiveTest1()
        {
            Map map = CreateSampleMap4x4();
            TileGroup tg = new ResourceTileGroup(new ResourceTile[] { new OreResourceTile(), new FoodResourceTile(), new GoodsResourceTile() });
            SpacePoint point = new SpacePoint(new HexCoordinates(1, 1), 1);
            map.SetTileGroupAtSpacePoint(tg, point);
            Assert.True(map.getTileAt(new HexCoordinates(1, 1)) is OreResourceTile);
            Assert.True(map.getTileAt(new HexCoordinates(0, 1)) is FoodResourceTile);
            Assert.True(map.getTileAt(new HexCoordinates(1, 0)) is GoodsResourceTile);
        }

        [Test]
        public void TestSetTileGroupAtSpacePointPositiveTest2()
        {
            Map map = CreateSampleMap4x4();
            TileGroup tg = new ResourceTileGroup(new ResourceTile[] { new OreResourceTile(), new FoodResourceTile(), new GoodsResourceTile() });
            SpacePoint point = new SpacePoint(new HexCoordinates(0, 1), 0);
            map.SetTileGroupAtSpacePoint(tg, point);
            Assert.True(map.getTileAt(new HexCoordinates(0, 1)) is OreResourceTile);
            Assert.True(map.getTileAt(new HexCoordinates(0, 0)) is FoodResourceTile);
            Assert.True(map.getTileAt(new HexCoordinates(1, 0)) is GoodsResourceTile);
        }

        [Test]
        public void TestSetTileGroupAtSpacePointNegativeTest()
        {
            Map map = CreateSampleMap4x4();
            TileGroup tg = new ResourceTileGroup(new ResourceTile[] { new OreResourceTile(), new FoodResourceTile(), new GoodsResourceTile() });
            SpacePoint point = new SpacePoint(new HexCoordinates(1, 1), 0);
            try
            {
                // should fail since (2,0) is BorderTile and cant be set
                map.SetTileGroupAtSpacePoint(tg, point);
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                Assert.True(true);
            }
            
            
        }

    }
}
