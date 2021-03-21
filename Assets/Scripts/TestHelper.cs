using System;
using UnityEngine;
using UnityEngine.UI;


public class TestHelper
{
    public TestHelper()
    {

    }

    public Player CreateGenericPlayer()
    {
        return new Player(Color.black, new SFElement());
    }
}
