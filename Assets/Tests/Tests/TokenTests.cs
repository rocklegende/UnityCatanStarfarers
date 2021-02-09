using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TokenTests
    {

        [Test]
        public void SettlingMechanisms1()
        {
            var token = new ColonyBaseToken();
            token.attachToken(new ShipToken());
            Assert.True(token.attachedToken != null);
            Assert.True(!token.IsSettled());
            token.settle();
            Assert.True(token.attachedToken == null);
            Assert.True(token.IsSettled());
        }

    }
}
