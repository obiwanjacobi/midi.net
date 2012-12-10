using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.IO.UnitTests.Media
{
    [TestClass]
    [DeploymentItem(@"Media\town.mid")]
    public class MediaTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Check_Town_Mid_Exists()
        {
            Assert.IsTrue(File.Exists(Path.Combine(TestContext.DeploymentDirectory, TestMedia.MidFileName)));
        }
    }
}