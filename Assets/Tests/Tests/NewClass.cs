using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

namespace Tests
{
    public class EncounterCardTests
    {
        [Test]
        public void EncounterCard1Test()
        {
            var hudScript = new HUDScript();
            var factory = new EncounterCardFactory(hudScript);
            var encounterCard = factory.CreateEncounterCard1();

            //encounterCard.start();
            //encounterCard.traverser.start();

            //encounterCard.traverser.InputReceived(2);
            //Assert.True(encounterCard.traverser.currentNode = )

            //rootNode.action.ResultFound(2);

            //decisionTreeTraverser: Klasse die decisionTree abläuft
            //wenn decisionTreeTraverser es geschafft hat ist das Encounter vorbei
        }

    }
}
