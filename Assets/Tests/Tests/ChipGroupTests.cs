using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ChipGroupTests
    {
        [Test]
        public void RetrieveChipPositive()
        {
            CircleChipGroup circleChipGroup = new CircleChipGroup(new List<DiceChip>() { new DiceChip3(), new DiceChip3() });

            var chip = circleChipGroup.RetrieveChip();
            Assert.True(circleChipGroup.GetChips().Count == 1);
        }

        [Test]
        public void RetrieveChipNegative()
        {
            CircleChipGroup circleChipGroup = new CircleChipGroup(new List<DiceChip>() { });
            try
            {
                var chip = circleChipGroup.RetrieveChip();
                Assert.True(false);
            }
            catch (ArgumentException e)
            {
                //should fail since there are no chips left
                Assert.True(true);
            }
           
        }

    }
}
