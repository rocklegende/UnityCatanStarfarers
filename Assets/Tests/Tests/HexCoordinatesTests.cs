using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class HexCoordinatesTests
    {
        [Test]
        public void HexCoordinatesNeighborTests()
        {
            HexCoordinates sampleHexCoords = new HexCoordinates(2, 3);
            Assert.AreEqual(sampleHexCoords.getNeighborCoordinates(Direction.E), new HexCoordinates(3, 3));
            Assert.AreEqual(sampleHexCoords.E(), new HexCoordinates(3, 3));

            Assert.AreEqual(sampleHexCoords.getNeighborCoordinates(Direction.W), new HexCoordinates(1, 3));
            Assert.AreEqual(sampleHexCoords.W(), new HexCoordinates(1, 3));

            Assert.AreEqual(sampleHexCoords.getNeighborCoordinates(Direction.NE), new HexCoordinates(3, 2));
            Assert.AreEqual(sampleHexCoords.NE(), new HexCoordinates(3, 2));

            Assert.AreEqual(sampleHexCoords.getNeighborCoordinates(Direction.SW), new HexCoordinates(1, 4));
            Assert.AreEqual(sampleHexCoords.SW(), new HexCoordinates(1, 4));

            Assert.AreEqual(sampleHexCoords.getNeighborCoordinates(Direction.NW), new HexCoordinates(2, 2));
            Assert.AreEqual(sampleHexCoords.NW(), new HexCoordinates(2, 2));

            Assert.AreEqual(sampleHexCoords.getNeighborCoordinates(Direction.SE), new HexCoordinates(2, 4));
            Assert.AreEqual(sampleHexCoords.SE(), new HexCoordinates(2, 4));
        }

        [Test]
        public void HexCoordinatesGetCubeCoordinates()
        {
            HexCoordinates hex = new HexCoordinates(-2, -2);
            Assert.AreEqual(hex.getCubeRepresentation(), new Vector3(-2, 4, -2));

            hex = new HexCoordinates(-2, 2);
            Assert.AreEqual(hex.getCubeRepresentation(), new Vector3(-2, 0, 2));
        }

    }
}
