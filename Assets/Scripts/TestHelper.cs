using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class TestHelper
{
    public TestHelper()
    {

    }

    public Player CreateGenericPlayer()
    {
        return new Player(Color.black, new SFElement());
    }

    public static Map CreateMockMap()
    {
        return new Map(new Tile_[,] { });
    }

    //public static bool ListsAreEqual(IList<T> list1, IList<T> list2)
    //{

    //}
}
