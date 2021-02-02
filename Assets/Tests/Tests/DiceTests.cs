using System.Collections;
using System.Collections.Generic;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class DiceTest
    {

        int[] ThrowDiceXTimes(int n) {
            List<int> thrownValues = new List<int>();
            var dt = new DiceThrower();
            for (int i = 0; i < n; i++)
            {
                var diceThrow = dt.throwDice();
                thrownValues.Add(diceThrow.GetValue());
            }
            return thrownValues.ToArray();
        }



        // A Test behaves as an ordinary method
        [Test]
        public void ThrowDice1000TimesAndCheckValueIsInRange()
        {

            var dt = new DiceThrower();
            for (int i = 0; i < 1000; i++)
            {
                var diceThrow = dt.throwDice();
                Assert.True(diceThrow.GetValue() <= 12 && diceThrow.GetValue() >= 1);
            }
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CheckEvenDistribution()
        {

            int n = 10000000;
            int[] thrownValues = ThrowDiceXTimes(n);
            int num12Thrown = Array.FindAll<int>(thrownValues, v => v == 12).Length;
            int num7Thrown = Array.FindAll<int>(thrownValues, v => v == 7).Length;
            float acceptedErrorMargin = 0.01F;

            float expectedPercentage12 = 1.0F / 36.0F;
            float expectedPercentage7 = 1.0F / 6.0F;

            float actualPercentage12 = (float) num12Thrown / (float) n;
            float actualPercentage7 = (float) num7Thrown / (float) n;

            Assert.True(Math.Abs(actualPercentage12 - expectedPercentage12) <= acceptedErrorMargin);
            Assert.True(Math.Abs(actualPercentage7 - expectedPercentage7) <= acceptedErrorMargin);
        }

        
    }
}
