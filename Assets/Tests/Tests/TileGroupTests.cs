using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TileGroupTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TileGroupSuccessfulCreationWith3Tiles()
        {
            // Use the Assert class to test conditions
            Tile_[] tiles = { new FoodResourceTile(), new FoodResourceTile(), new FoodResourceTile() };
            TileGroup group = new TileGroup(tiles);

        }

        [Test]
        public void TileGroupFailCreationWithLessThan3Tiles()
        {
            try
            {
                Tile_[] tiles = { new FoodResourceTile(), new FoodResourceTile() };
                TileGroup group = new TileGroup(tiles);
                Assert.True(false); // Fail this test if we come to this point
            }
            catch
            {
                Assert.True(true);
            }
         
        }

        [Test]
        public void TileGroupFailCreationWithMoreThan3Tiles()
        {
            try
            {
                Tile_[] tiles = { new FoodResourceTile(), new FoodResourceTile(), new FoodResourceTile(), new FoodResourceTile() };
                TileGroup group = new TileGroup(tiles);
                Assert.True(false); // Fail this test if we come to this point
            }
            catch
            {
                Assert.True(true);
            }



        }


    }
}
