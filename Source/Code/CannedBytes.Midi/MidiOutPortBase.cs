using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutPortBase class represent the common implementation for
    /// both the <see cref="MidiOutPort"/> and the <see cref="MidiOutStreamPort"/>.
    /// </summary>
    public abstract class MidiOutPortBase : MidiPort, IMidiDataSender
    {
        /// <summary>
        /// Provides a base implementation for opening an Out Port.
        /// </summary>
        /// <param name="portId">Must lie between 0 and the number of out devices.</param>
        public override void Open(int portId)
        {
            ThrowIfDisposed();
            Throw.IfArgumentOutOfRange(portId, 0, NativeMethods.midiOutGetNumDevs() - 1, "portId");

            base.Open(portId);

            if (IsOpen && MidiBufferManager != null)
            {
                MidiBufferManager.PrepareAllBuffers();
            }
        }

        /// <summary>
        /// Turns off all notes and returns pending <see cref="T:MidiBufferStream"/>s to the
        /// <see cref="P:BufferManager"/> marked as done.
        /// </summary>
        public override void Reset()
        {
            if (!IsOpen)
            {
                throw new MidiInPortException(Properties.Resources.MidiOutPort_PortNotOpen);
            }

            int result = NativeMethods.midiOutReset(MidiSafeHandle);

            ThrowIfError(result);

            base.Reset();
        }

        private MidiOutBufferManager bufferManager;

        /// <summary>
        /// Gets the buffer manager for the Midi In Port.
        /// </summary>
        public virtual MidiOutBufferManager MidiBufferManager
        {
            get
            {
                if (this.bufferManager == null)
                {
                    this.bufferManager = new MidiOutBufferManager(this);
                }

                return this.bufferManager;
            }
            protected set
            {
                Contract.Requires(value != null);
                Throw.IfArgumentNull(value, "MidiBufferManager");

                this.bufferManager = value;
            }
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">When true also the managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.bufferManager != null)
                {
                    this.bufferManager.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Retrieves the Midi Stream Out Port capabilities.
        /// </summary>
        public MidiOutPortCaps Capabilities
        {
            get { return MidiOutPort.GetPortCapabilities(PortId); }
        }

        /// <summary>
        /// Returns the capabilities for the specified <paramref name="portId"/>.
        /// </summary>
        /// <param name="portId">An index into the list of available out ports.</param>
        /// <returns>Never returns null.</returns>
        public static MidiOutPortCaps GetPortCapabilities(int portId)
        {
            MidiOutCaps caps = new MidiOutCaps();

            int result = NativeMethods.midiOutGetDevCaps(
                new IntPtr(portId), ref caps, (uint)MemoryUtil.SizeOfMidiOutCaps);

            ThrowIfError(result);

            return new MidiOutPortCaps(ref caps);
        }

        /// <summary>
        /// Throws an exception if the <paramref name="result"/> represents an error code.
        /// </summary>
        /// <param name="result">The result of a call to one of the <see cref="NativeMethods"/> methods.</param>
        /// <exception cref="MidiOutPortException">Thrown when the <paramref name="result"/> is non-zero.</exception>
        public static void ThrowIfError(int result)
        {
            if (result != NativeMethods.MMSYSERR_NOERROR)
            {
                string msg = GetErrorText(result);

                throw new MidiOutPortException(msg);
            }
        }

        /// <summary>
        /// Lookup an error description text.
        /// </summary>
        /// <param name="result">Result of a <see cref="NativeMethods"/> call.</param>
        /// <returns>Returns the error text or an empty string. Never returns null.</returns>
        protected static string GetErrorText(int result)
        {
            if (result != NativeMethods.MMSYSERR_NOERROR)
            {
                StringBuilder builder = new StringBuilder(NativeMethods.MAXERRORLENGTH);

                int err = NativeMethods.midiOutGetErrorText(result, builder, builder.Capacity);

                if (err != NativeMethods.MMSYSERR_NOERROR)
                {
                    // TODO: log error
                }

                return builder.ToString();
            }

            return String.Empty;
        }

        #region IMidiSender Members

        /// <summary>
        /// Sends the short midi message to the Midi Out Port.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        public virtual void ShortData(int data)
        {
            int result = NativeMethods.midiOutShortMsg(MidiSafeHandle, (uint)data);

            ThrowIfError(result);
        }

        /// <summary>
        /// Sends the long midi message to the Midi Out Port.
        /// </summary>
        /// <param name="buffer">The long midi message. Must not be null.</param>
        public virtual void LongData(MidiBufferStream buffer)
        {
            Throw.IfArgumentNull(buffer, "buffer");

            //if ((buffer.HeaderFlags & NativeMethods.MHDR_PREPARED) == 0)
            //{
            //    throw new InvalidOperationException("LongData cannot be called with a MidiBufferStream that has not been prepared.");
            //}

            int result = NativeMethods.midiOutLongMsg(MidiSafeHandle, buffer.ToIntPtr(),
                (uint)MemoryUtil.SizeOfMidiHeader);

            ThrowIfError(result);
        }

        #endregion IMidiSender Members

        /// <summary>
        /// Midi out device callback handler.
        /// </summary>
        /// <param name="msg">The type of callback event.</param>
        /// <param name="param1">First parameter dependent on <paramref name="msg"/>.</param>
        /// <param name="param2">Second parameter dependent on <paramref name="msg"/>.</param>
        protected override bool OnMessage(int msg, IntPtr param1, IntPtr param2)
        {
            bool handled = true;

            switch ((uint)msg)
            {
                case NativeMethods.MOM_OPEN:
                    // don't change status here, MidiSafeHandle has not been set yet.
                    break;
                case NativeMethods.MOM_CLOSE:
                    Status = MidiPortStatus.Closed;
                    MidiSafeHandle = null;
                    break;
                case NativeMethods.MOM_DONE:
                    MidiBufferStream buffer = MidiBufferManager.FindBuffer(param1);
                    if (buffer != null)
                    {
                        MidiBufferManager.Return(buffer);
                    }
                    break;
                default:
                    handled = false;
                    break;
            }

            return handled;
        }
    }
}