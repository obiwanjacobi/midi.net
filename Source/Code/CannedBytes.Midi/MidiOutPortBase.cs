namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Threading;
    using System.Diagnostics;

    /// <summary>
    /// The MidiOutPortBase class represent the common implementation for
    /// both the <see cref="MidiOutPort"/> and the <see cref="MidiOutStreamPort"/>.
    /// </summary>
    public abstract class MidiOutPortBase : MidiPort, IMidiDataSender, IChainOf<IMidiDataCallback>
    {
        /// <summary>
        /// Provides a base implementation for opening an Out Port.
        /// </summary>
        /// <param name="deviceId">Must lie between 0 and the number of out devices.</param>
        public override void Open(int deviceId)
        {
            ThrowIfDisposed();
            Check.IfArgumentOutOfRange(deviceId, 0, NativeMethods.midiOutGetNumDevs() - 1, "deviceId");

            base.Open(deviceId);

            if (IsOpen && this.bufferManager != null)
            {
                this.bufferManager.PrepareAllBuffers();
            }
        }

        /// <summary>
        /// Closes the Midi Out Port.
        /// </summary>
        /// <remarks>
        /// If any buffers are still in use the <see cref="M:Reset"/> method is called to
        /// return all the buffers to the <see cref="P:BufferManager"/>. The method will block until all
        /// buffers are returned.
        /// </remarks>
        public override void Close()
        {
            ThrowIfDisposed();

            if (this.bufferManager != null)
            {
                Status = MidiPortStatus.Closed | MidiPortStatus.Pending;

                if (this.bufferManager.UsedBufferCount > 0)
                {
                    // Reset returns the buffers from the port
                    Reset();

                    // wait until all buffers are returned
                    bool success = this.bufferManager.WaitForBuffersReturned(Timeout.Infinite);

                    // should always work with infinite timeout
                    Debug.Assert(success, "Infinite timeout still fails.");
                }

                this.bufferManager.UnprepareAllBuffers();
            }

            base.Close();
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

        /// <summary>
        /// Backing field for the <see cref="BufferManager"/> property.
        /// </summary>
        private MidiOutBufferManager bufferManager;

        /// <summary>
        /// Gets the buffer manager for the Midi In Port.
        /// </summary>
        public virtual MidiOutBufferManager BufferManager
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
                Check.IfArgumentNull(value, "MidiBufferManager");

                this.bufferManager = value;
            }
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            try
            {
                if (!IsDisposed)
                {
                    if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
                    {
                        if (this.bufferManager != null)
                        {
                            this.bufferManager.Dispose();
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposeKind);
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
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public virtual void LongData(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            ////if ((buffer.HeaderFlags & NativeMethods.MHDR_PREPARED) == 0)
            ////{
            ////    throw new InvalidOperationException("LongData cannot be called with a MidiBufferStream that has not been prepared.");
            ////}

            buffer.BytesRecorded = buffer.Position;

            int result = NativeMethods.midiOutLongMsg(
                         MidiSafeHandle,
                         buffer.ToIntPtr(),
                         (uint)MemoryUtil.SizeOfMidiHeader);

            ThrowIfError(result);
        }

        #endregion IMidiSender Members

        #region IChainOf<IMidiDataCallback> members

        /// <summary>
        /// Backing field of the <see cref="NextCallback"/> properties.
        /// </summary>
        private IMidiDataCallback callback;

        /// <summary>
        /// Gets or sets the reference to the next component that receives the callback.
        /// </summary>
        IMidiDataCallback IChainOf<IMidiDataCallback>.Successor
        {
            get
            {
                return this.callback;
            }

            set
            {
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiOutPort_CannotChangeCallback);
                }

                this.callback = value;
            }
        }

        /// <summary>
        /// Gets or sets the callback reference that receives notifications.
        /// </summary>
        public IMidiDataCallback NextCallback
        {
            get
            {
                return this.callback;
            }

            set
            {
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiOutPort_CannotChangeCallback);
                }

                this.callback = value;
            }
        }

        #endregion IChainOf<IMidiDataCallback> members

        /// <summary>
        /// Midi out device callback handler.
        /// </summary>
        /// <param name="msg">The type of callback event.</param>
        /// <param name="parameter1">First parameter dependent on <paramref name="msg"/>.</param>
        /// <param name="parameter2">Second parameter dependent on <paramref name="msg"/>.</param>
        /// <returns>Returns true when handled.</returns>
        protected override bool OnMessage(int msg, IntPtr parameter1, IntPtr parameter2)
        {
            Contract.Assume(this.BufferManager != null);

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
                    MidiBufferStream buffer = this.BufferManager.FindBuffer(parameter1);
                    if (buffer != null)
                    {
                        if (this.NextCallback != null)
                        {
                            this.NextCallback.LongData(buffer, MidiDataCallbackType.Done);
                        }

                        this.BufferManager.ReturnBuffer(buffer);
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