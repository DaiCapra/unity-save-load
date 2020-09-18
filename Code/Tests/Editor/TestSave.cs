using System.Collections;
using Code.Runtime;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Code.Tests.Editor
{
    public class TestSave
    {
        private SaveService _service;
        private string _root;

        [SetUp]
        public void Setup()
        {
            _root = $"{Application.streamingAssetsPath}";

            _service = new SaveService();
            _service.Init(_root);
        }

        [Test]
        public void WriteAndRead()
        {
            var path = "test";
            var data1 = new DataContainer {value = 42};

            Assert.True(_service.WriteToFile(path, data1));
            Assert.True(_service.ReadFromFile(path, out DataContainer data2));
            Assert.AreEqual(data1.value, data2.value);
        }

        [Test]
        public void SaveAndLoad()
        {
            var campaign = "campaign";
            var name = "save1";
            var data1 = new DataContainer {value = 42};

            Assert.True(_service.Save(campaign, name, data1));
            Assert.True(_service.Load(campaign, name, out DataContainer data2));

            Assert.AreEqual(data1.value, data2.value);
        }

        [Test]
        public void SaveAndDelete()
        {
            var campaign = "campaign";
            var name = "save2";
            var data1 = new DataContainer {value = 42};
            Assert.True(_service.Save(campaign, name, data1));
            Assert.True(_service.Delete(campaign, name));
        }

        [Test]
        public void AutoSave()

        {
            var campaign = "autosaves";
            var data1 = new DataContainer {value = 42};

            Assert.True(_service.Delete(campaign));

            for (int i = 0; i < 10; i++)
            {
                Assert.True(_service.AutoSave(campaign, data1));
            }

            var saves = _service.GetSaves(campaign);
            Assert.AreEqual(_service.Settings.AutoSaveCount, saves.Count);
        }
    }
}