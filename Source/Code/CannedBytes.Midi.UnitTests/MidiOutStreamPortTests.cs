using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiOutStreamPortTests
    {
        static MidiOutPortCapsCollection portCaps = new MidiOutPortCapsCollection();

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        internal static MidiOutStreamPort CreateMidiOutStreamPort()
        {
            if (portCaps.Count == 0)
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
                Assert.IsNotNull(port.BufferManager);
            }
        }

        [TestMethod]
        public void Ctor_NoParams_ClosedStatus()
        {
            using (var port = CreateMidiOutStreamPort())
            {
                Assert.IsTrue(port.HasStatus(MidiPortStatus.Closed));
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

            Assert.IsTrue(port.Status == MidiPortStatus.None);
        }

        [TestMethod]
        public void Open_ThenClose_StatusReflectsOpenState()
        {
            using (var port = CreateMidiOutStreamPort())
            {
                port.Open(0);

                Assert.IsTrue(port.HasStatus(MidiPortStatus.Open));

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

                Assert.IsTrue(port.HasStatus(MidiPortStatus.Closed));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Dispose_AccessDisposedPort_ThrowsException()
        {
            MidiOutStreamPort port = null;

            using (port = CreateMidiOutStreamPort())
            {
            } // Dispose

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