using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Bingyan.Test
{
    public class ArchiveTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void IOTest()
        {
            Archive.Set("flt", 1.2f);
            Archive.Set("vec2", Vector2.one);
            Archive.Set("vec3", Vector3.one);

            var quat = new Quaternion(1, 2, 3, 4);
            Archive.Set("quat", quat);
            Archive.Save(0);

            Archive.LoadToGame(0);
            Assert.AreEqual(1.2f, Archive.Get("flt", 0f));
            Assert.AreEqual(Vector2.one, Archive.Get("vec2", Vector2.zero));
            Assert.AreEqual(Vector3.one, Archive.Get("vec3", Vector3.zero));
            Assert.AreEqual(quat, Archive.Get("quat", Quaternion.identity));
        }
    }
}