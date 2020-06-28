using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CannedBytes.Midi.IO.UnitTests.Media
{
    [TestClass]
    [DeploymentItem(@"IO\Media\town.mid")]
    public class MediaTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Check_Town_Mid_Exists()
        {
            File.Exists(Path.Combine(TestContext.DeploymentDirectory, TestMedia.MidFileName)).Should().BeTrue();
        }
    }
}