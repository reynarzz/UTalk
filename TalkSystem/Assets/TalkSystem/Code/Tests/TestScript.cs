using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
//using TalkSystem.Editor;
using uTalk;

namespace Tests
{
    public class TestScript
    {
        [Test]
        public void GetStartingCharIndex_TEST()
        {
            //var charIndex = Highlight.GetStartingCharIndex("The text being tested right now text", 6);
            //var charIndex2 = Highlight.GetStartingCharIndex("The text being\ntested right now text", 3);

            //Assert.IsTrue(charIndex == 32);
            //Assert.IsTrue(charIndex2 == 15);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
