using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="MidiInPort"/> class.
    /// </summary>
    [TestClass]
    public class MidiInPortTests
    {
        static MidiInPortCapsCollection portCaps = new MidiInPortCapsCollection();

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        internal static MidiInPort CreateMidiInPort()
        {
            if (portCaps.Count == 0)
            {
                Assert.Inconclusive("No Midi In Ports found.");
            }

            return new MidiInPort();
        }

        [TestMethod]
        public void Ctor_NoParams_BufferManagerNotNull()
        {
            using (var port = CreateMidiInPort())
            {
                // must have a buffer manager
                Assert.IsNotNull(port.MidiBufferManager);
            }
        }

        [TestMethod]
        public void Ctor_NoParams_ClosedStatus()
        {
            using (var port = CreateMidiInPort())
            {
                Assert.IsTrue(port.HasStatus(MidiPortStatus.Closed));
            }
        }

        [TestMethod]
        public void Dispose_FreshInstance_StatusReflectsState()
        {
            MidiInPort port = null;

            using (port = CreateMidiInPort())
            {
                // dispose
            }

            Assert.IsTrue(port.Status == MidiPortStatus.None);
        }

        [TestMethod]
        public void Open_ThenClose_StatusReflectsOpenState()
        {
            using (var port = CreateMidiInPort())
            {
                port.Open(0);

                Assert.IsTrue(port.HasStatus(MidiPortStatus.Open));

                port.Close();
            }
        }

        [TestMethod]
        public void Open_ThenClose_StatusReflectsClosedState()
        {
            using (var port = CreateMidiInPort())
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
            MidiInPort port = null;
            using (port = CreateMidiInPort())
            {
            } // Dispose

            port.Open(0);

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(MidiInPortException))]
        public void Reset_PortNotOpen_ThrowsException()
        {
            using (var port = CreateMidiInPort())
            {
                port.Reset();
            }

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(MidiInPortException))]
        public void Start_PortNotOpen_ThrowsException()
        {
            using (var port = CreateMidiInPort())
            {
                port.Start();
            }

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(MidiInPortException))]
        public void Stop_PortNotOpen_ThrowsException()
        {
            using (var port = CreateMidiInPort())
            {
                port.Stop();
            }

            Assert.Fail();
        }
    }
}