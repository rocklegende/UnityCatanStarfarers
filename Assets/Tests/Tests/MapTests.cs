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
                { new NullTile(), new BorderTile(), new BorderTile() },
                { new BorderTile(), new FoodResourceTile(), new BorderTile() },
                { new BorderTile(), new BorderTile(), new BorderTile() },
            };

            return new Map(mapRepresentation);
        }


        private Map CreateSampleMap4x3()
        {
            Tile_[,] mapRepresentation = {
                { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new NullTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap4x4()
        {
            Tile_[,] mapRepresentation = {
                { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new NullTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new NullTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap4x5()
        {
            Tile_[,] mapRepresentation = {
                { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new NullTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new NullTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new NullTile() },
            };

            return new Map(mapRepresentation);
        }

        private Map CreateSampleMap5x6()
        {
            Tile_[,] mapRepresentation = {
                { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
                { new NullTile(), new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile() },
                { new FoodResourceTile(), new FoodResourceTile(), new BorderTile(), new BorderTile(), new NullTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new NullTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new NullTile() },
                { new BorderTile(), new BorderTile(), new BorderTile(), new BorderTile(), new NullTile() },
            };

            return new Map(mapRepresentation);
        }


        private bool spacePointArraysAreEqual(SpacePoint[] spacePoints1, SpacePoint[] spacePoints2)
        {
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

            Map map = CreateSampleMap3x3();

            SpacePoint origin = new SpacePoint(new HexCoordinates(0, 1), 0);
            SpacePoint[] spacePointsTwoStepsAway = map.getAllSpacePointsInDistance(origin, 2);

            SpacePoint[] expectedPoints = new SpacePoint[]
            {
                new SpacePoint(new HexCoordinates(0, 2), 0),
                new SpacePoint(new HexCoordinates(-1, 2), 0)
            }; 

            Assert.True(spacePointArraysAreEqual(spacePointsTwoStepsAway, expectedPoints));
        }

        [Test]
        public void MapGetAllSpacePointsInDistance3()
        {

            Map map = CreateSampleMap3x3();
            SpacePoint origin = new SpacePoint(new HexCoordinates(0, 1), 0);

            SpacePoint[] spacePointsThreeStepsAway = map.getAllSpacePointsInDistance(origin, 3);
            SpacePoint[] expectedPoints = new SpacePoint[]
            {
                new SpacePoint(new HexCoordinates(0, 2), 1),
            };

            printSpacePoints(map.getAllAvailableSpacePoints());

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

    }
}
