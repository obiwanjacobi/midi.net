using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class MidiOutPortTests
    {
        static MidiOutPortCapsCollection portCaps = new MidiOutPortCapsCollection();

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        internal static MidiOutPort CreateMidiOutPort()
        {
            if (portCaps.Count == 0)
            {
                Assert.Inconclusive("No Midi Out Ports found.");
            }

            return new MidiOutPort();
        }

        [TestMethod]
        public void Ctor_NoParams_BufferManagerNotNull()
        {
            using (var port = CreateMidiOutPort())
            {
                // must have a buffer manager
                Assert.IsNotNull(port.BufferManager);
            }
        }

        [TestMethod]
        public void Ctor_NoParams_ClosedStatus()
        {
            using (var port = CreateMidiOutPort())
            {
                Assert.IsTrue(port.HasStatus(MidiPortStatus.Closed));
            }
        }

        [TestMethod]
        public void Dispose_FreshInstance_StatusReflectsState()
        {
            MidiOutPort port = null;

            using (port = CreateMidiOutPort())
            {
                // dispose
            }

            Assert.IsTrue(port.Status == MidiPortStatus.None);
        }

        [TestMethod]
        public void Open_ThenClose_StatusReflectsOpenState()
        {
            using (var port = CreateMidiOutPort())
            {
                port.Open(0);

                Assert.IsTrue(port.HasStatus(MidiPortStatus.Open));

                port.Close();
            }
        }

        [TestMethod]
        public void Open_ThenClose_StatusReflectsClosedState()
        {
            using (var port = CreateMidiOutPort())
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
            MidiOutPort port = null;
            using (port = CreateMidiOutPort())
            {
            } // Dispose

            port.Open(0);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(MidiInPortException))]
        public void Reset_PortNotOpen_ThrowsException()
        {
            using (var port = CreateMidiOutPort())
            {
                port.Reset();
            }

            Assert.Fail();
        }
    }
}