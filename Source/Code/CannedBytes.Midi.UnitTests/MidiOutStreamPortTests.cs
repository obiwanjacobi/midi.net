using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiOutStreamPortTests
    {
        static readonly MidiOutPortCapsCollection _portCaps = new MidiOutPortCapsCollection();

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        internal static MidiOutStreamPort CreateMidiOutStreamPort()
        {
            if (_portCaps.Count == 0)
            {
                Assert.Inconclusive("No Midi Out Ports found.");
            }

            return new MidiOutStreamPort();
        }

        [TestMethod]
        public void Ctor_NoParams_BufferManagerNotNull()
        {
            using (var port = CreateMidiOutStreamPort())
            {
                // must have a buffer manager
                port.BufferManager.Should().NotBeNull();
            }
        }

        [TestMethod]
        public void Ctor_NoParams_ClosedStatus()
        {
            using (var port = CreateMidiOutStreamPort())
            {
                port.HasStatus(MidiPortStatus.Closed).Should().BeTrue();
            }
        }

        [TestMethod]
        public void Dispose_FreshInstance_StatusReflectsState()
        {
            MidiOutStreamPort port = null;

            using (port = CreateMidiOutStreamPort())
            {
                // dispose
            }

            port.Status.Should().Be(MidiPortStatus.None);
        }

        [TestMethod]
        public void Open_ThenClose_StatusReflectsOpenState()
        {
            using (var port = CreateMidiOutStreamPort())
            {
                port.Open(0);

                port.HasStatus(MidiPortStatus.Open).Should().BeTrue();

                port.Close();
            }
        }

        [TestMethod]
        public void Open_ThenClose_StatusReflectsClosedState()
        {
            using (var port = CreateMidiOutStreamPort())
            {
                port.Open(0);

                port.Close();

                port.HasStatus(MidiPortStatus.Closed).Should().BeTrue();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose_AccessDisposedPort_ThrowsException()
        {
            var port = CreateMidiOutStreamPort();
            port.Dispose();

            port.Open(0);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(MidiInPortException))]
        public void Reset_PortNotOpen_ThrowsException()
        {
            using (var port = CreateMidiOutStreamPort())
            {
                port.Reset();
            }

            Assert.Fail();
        }
    }
}