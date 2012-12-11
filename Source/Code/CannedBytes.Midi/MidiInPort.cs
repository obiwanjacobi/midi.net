namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// The MidiInPort class represents an interface to a physical (or virtual, depending on the driver)
    /// Midi In Port.
    /// </summary>
    /// <remarks>
    /// Note that when registering a Port Event receiver it is only called when there were no Data or Error
    /// receivers to take the message.
    /// </remarks>
    public class MidiInPort : MidiPort,
        IChainOf<IMidiDataReceiver>,
        IChainOf<IMidiDataErrorReceiver>,
        IChainOf<IMidiPortEventReceiver>
    {
        /// <summary>
        /// Opens the Midi In Port identified by the <paramref name="portId"/>.
        /// </summary>
        /// <param name="portId">An index into the available Midi In Ports.</param>
        /// <remarks>Refer to <see cref="MidiInPortCapsCollection"/>.</remarks>
        public override void Open(int portId)
        {
            ThrowIfDisposed();
            Throw.IfArgumentOutOfRange(portId, 0, NativeMethods.midiInGetNumDevs() - 1, "portId");

            base.Open(portId);

            MidiInSafeHandle inHandle;

            int result = NativeMethods.midiInOpen(
                         out inHandle,
                         (uint)portId,
                         MidiProcRef,
                         ToIntPtr(),
                         NativeMethods.CALLBACK_FUNCTION | NativeMethods.MIDI_IO_STATUS);

            ThrowIfError(result);

            MidiSafeHandle = inHandle;

            Status = MidiPortStatus.Open;

            this.BufferManager.RegisterAllBuffers();
        }

        /// <summary>
        /// Closes the Midi In Port.
        /// </summary>
        /// <remarks>
        /// When the Midi Port is in a <see cref="MidiPortStatus.Started"/> status <see cref="M:Stop"/>
        /// is called. If any buffers are still in use the <see cref="M:Reset"/> method is called to
        /// return all the buffers to the <see cref="P:BufferManager"/>. The method will block until all
        /// buffers are returned.
        /// </remarks>
        public override void Close()
        {
            ThrowIfDisposed();

            if (HasStatus(MidiPortStatus.Started))
            {
                this.Stop();
            }

            if (this.bufferManager != null)
            {
                Status = MidiPortStatus.Closed | MidiPortStatus.Pending;

                if (this.bufferManager.UsedBufferCount > 0)
                {
                    // Reset returns the buffers from the port
                    this.Reset();

                    // wait until all buffers are returned
                    bool success = this.bufferManager.WaitForBuffersReturned(Timeout.Infinite);

                    // should always work with infinite timeout
                    Debug.Assert(success, "Infinite timeout still failed.");
                }

                this.bufferManager.UnprepareAllBuffers();
            }

            base.Close();
        }

        /// <summary>
        /// Resets the Midi In Port returning all buffers to the <see cref="P:BufferManager"/>.
        /// </summary>
        public override void Reset()
        {
            ThrowIfDisposed();

            if (!IsOpen)
            {
                throw new MidiInPortException(Properties.Resources.MidiInPort_PortNotOpen);
            }

            // we change the Status before making the API call to make
            // sure the returning buffers are not added again.
            base.Reset();

            int result = NativeMethods.midiInReset(MidiSafeHandle);

            ThrowIfError(result);
        }

        /// <summary>
        /// Starts recording on the Midi In Port.
        /// </summary>
        /// <remarks>
        /// The <see cref="P:NextReceiever"/> property must not be null or an exception will be thrown.
        /// </remarks>
        /// <exception cref="MidiInPortException">Thrown when the <see cref="P:NextReceiever"/> property is null
        /// or the <see cref="P:BufferManager"/> has not been initialized.</exception>
        public void Start()
        {
            ThrowIfDisposed();
            if (!IsOpen)
            {
                throw new MidiInPortException(Properties.Resources.MidiInPort_PortNotOpen);
            }

            // cannot start the in port before connecting it to a receiver
            if (this.Next == null && this.NextPortEventReceiver == null)
            {
                throw new MidiInPortException(Properties.Resources.MidiInPort_NoReceiver);
            }

            //// Not an error. What if we only want to receive short messages?
            ////if (!MidiBufferManager.IsInitialized)
            ////{
            ////    throw new MidiInPortException(Properties.Resources.MidiInPort_BufferManagerNotInitialzed);
            ////}

            int result = NativeMethods.midiInStart(MidiSafeHandle);

            ThrowIfError(result);

            ModifyStatus(MidiPortStatus.Started, MidiPortStatus.Stopped | MidiPortStatus.Paused);
        }

        /// <summary>
        /// Stop recording on the Midi In Port.
        /// </summary>
        /// <remarks>Pending buffers will be returned. Calling <see cref="Stop"/> when the Midi In Port
        /// is not in the <see cref="MidiPortStatus.Started"/> state has no effect.</remarks>
        public void Stop()
        {
            ThrowIfDisposed();

            if (!IsOpen)
            {
                throw new MidiInPortException(Properties.Resources.MidiInPort_PortNotOpen);
            }

            int result = NativeMethods.midiInStop(MidiSafeHandle);

            ThrowIfError(result);

            ModifyStatus(MidiPortStatus.Stopped, MidiPortStatus.Started | MidiPortStatus.Paused);
        }

        /// <summary>
        /// Retrieves the Midi In Port capabilities.
        /// </summary>
        public MidiInPortCaps Capabilities
        {
            get { return GetPortCapabilities(PortId); }
        }

        /// <summary>
        /// Throws an <see cref="MidiInPortException"/> when the <paramref name="result"/> is non-zero.
        /// </summary>
        /// <param name="result">A return value from on off the <see cref="NativeMethods"/> methods.</param>
        public static void ThrowIfError(int result)
        {
            if (result != NativeMethods.MMSYSERR_NOERROR)
            {
                string msg = GetErrorText(result);

                throw new MidiInPortException(msg);
            }
        }

        /// <summary>
        /// Retrieves the error text for the error <paramref name="result"/>.
        /// </summary>
        /// <param name="result">A return value from on off the <see cref="NativeMethods"/> methods.</param>
        /// <returns>Returns the error text or an empty string if <paramref name="result"/> was not an
        /// error. Never returns null.</returns>
        private static string GetErrorText(int result)
        {
            if (result != NativeMethods.MMSYSERR_NOERROR)
            {
                StringBuilder builder = new StringBuilder(NativeMethods.MAXERRORLENGTH);

                int err = NativeMethods.midiInGetErrorText(result, builder, builder.Capacity);

                if (err != NativeMethods.MMSYSERR_NOERROR)
                {
                    // TODO: log error
                }

                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary>
        /// Callback from the midi driver (on a separate thread).
        /// </summary>
        /// <param name="msg">The type of message to handle.</param>
        /// <param name="param1">Parameter 1.</param>
        /// <param name="param2">Parameter 2.</param>
        /// <returns>Returns true when handled.</returns>
        protected override bool OnMessage(int msg, IntPtr param1, IntPtr param2)
        {
            bool handled = true;

            try
            {
                uint umsg = (uint)msg;

                handled = this.HandleOpenAndClose(umsg);

                if (this.Next != null && handled == false)
                {
                    handled = this.HandleDataMessage(umsg, param1, param2);
                }

                if (this.NextErrorReceiver != null && handled == false)
                {
                    handled = this.HandleErrorMessage(umsg, param1, param2);
                }

                if (this.NextPortEventReceiver != null && handled == false)
                {
                    handled = this.HandlePortEvent(umsg, param1, param2);
                }

                if (handled == false)
                {
                    switch (umsg)
                    {
                        case NativeMethods.MIM_LONGDATA:
                        case NativeMethods.MIM_LONGERROR:
                            var buffer = this.BufferManager.FindBuffer(param1);

                            if (buffer != null)
                            {
                                // make sure buffers are returned when there's no handler to take care of it.
                                this.BufferManager.Return(buffer);
                                handled = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                // TODO:Logging
                Debug.WriteLine(e);
            }

            return handled;
        }

        /// <summary>
        /// Handles the long and short data messages.
        /// </summary>
        /// <param name="umsg">Type of message.</param>
        /// <param name="param1">First parameter.</param>
        /// <param name="param2">Second parameter.</param>
        /// <returns>Returns true if the <paramref name="umsg"/> has been handled.</returns>
        private bool HandleDataMessage(uint umsg, IntPtr param1, IntPtr param2)
        {
            Contract.Assume(this.Next != null);

            bool handled = true;

            switch (umsg)
            {
                case NativeMethods.MIM_DATA:
                    this.Next.ShortData(param1.ToInt32(), param2.ToInt32());
                    break;
                case NativeMethods.MIM_LONGDATA:
                    var buffer = this.BufferManager.FindBuffer(param1);

                    if (buffer != null)
                    {
                        if (buffer.BytesRecorded > 0)
                        {
                            this.Next.LongData(buffer, param2.ToInt32());

                            if (AutoReturnBuffers)
                            {
                                this.BufferManager.Return(buffer);
                            }
                        }
                        else
                        {
                            this.BufferManager.Return(buffer);
                        }
                    }
                    break;
                case NativeMethods.MIM_MOREDATA:
                    this.Next.ShortData(param1.ToInt32(), param2.ToInt32());
                    break;
                default:
                    handled = false;
                    break;
            }

            return handled;
        }

        /// <summary>
        /// Handles the port events for all message types.
        /// </summary>
        /// <param name="umsg">Type of message.</param>
        /// <param name="param1">First parameter.</param>
        /// <param name="param2">Second parameter.</param>
        /// <returns>Returns true if the <paramref name="umsg"/> has been handled.</returns>
        private bool HandlePortEvent(uint umsg, IntPtr param1, IntPtr param2)
        {
            Contract.Assume(this.NextPortEventReceiver != null);

            bool handled = true;

            switch (umsg)
            {
                case NativeMethods.MIM_DATA:
                    this.NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.ShortData, param1.ToInt32(), param2.ToInt32()));
                    break;
                case NativeMethods.MIM_LONGDATA:
                    var buffer = this.BufferManager.FindBuffer(param1);

                    if (buffer != null)
                    {
                        if (buffer.BytesRecorded > 0)
                        {
                            this.NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.LongData, buffer, param2.ToInt32()));

                            if (AutoReturnBuffers)
                            {
                                this.BufferManager.Return(buffer);
                            }
                        }
                        else
                        {
                            this.BufferManager.Return(buffer);
                        }
                    }
                    break;
                case NativeMethods.MIM_MOREDATA:
                    this.NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.MoreData, param1.ToInt32(), param2.ToInt32()));
                    break;
                case NativeMethods.MIM_ERROR:
                    this.NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.ShortError, param1.ToInt32(), param2.ToInt32()));
                    break;
                case NativeMethods.MIM_LONGERROR:
                    var errBuffer = this.BufferManager.FindBuffer(param1);

                    if (errBuffer != null)
                    {
                        this.NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.LongError, errBuffer, param2.ToInt32()));

                        if (AutoReturnBuffers)
                        {
                            this.BufferManager.Return(errBuffer);
                        }
                    }
                    break;
                default:
                    handled = false;
                    break;
            }

            return handled;
        }

        /// <summary>
        /// Handles the long and short error messages.
        /// </summary>
        /// <param name="umsg">Type of message.</param>
        /// <param name="param1">First parameter.</param>
        /// <param name="param2">Second parameter.</param>
        /// <returns>Returns true if the <paramref name="umsg"/> has been handled.</returns>
        private bool HandleErrorMessage(uint umsg, IntPtr param1, IntPtr param2)
        {
            Contract.Assume(this.NextErrorReceiver != null);

            bool handled = true;

            switch (umsg)
            {
                case NativeMethods.MIM_ERROR:
                    this.NextErrorReceiver.ShortError(param1.ToInt32(), param2.ToInt32());
                    break;
                case NativeMethods.MIM_LONGERROR:
                    var buffer = this.BufferManager.FindBuffer(param1);

                    if (buffer != null)
                    {
                        this.NextErrorReceiver.LongError(buffer, param2.ToInt32());

                        if (AutoReturnBuffers)
                        {
                            this.BufferManager.Return(buffer);
                        }
                    }
                    break;
                default:
                    handled = false;
                    break;
            }

            return handled;
        }

        /// <summary>
        /// Handles the open and close messages.
        /// </summary>
        /// <param name="umsg">Type of message.</param>
        /// <returns>Returns true if the <paramref name="umsg"/> has been handled.</returns>
        private bool HandleOpenAndClose(uint umsg)
        {
            bool handled = true;

            switch (umsg)
            {
                case NativeMethods.MIM_OPEN:
                    // don't change status here, MidiSafeHandle has not been set yet.
                    break;
                case NativeMethods.MIM_CLOSE:
                    MidiSafeHandle = null;
                    Status = MidiPortStatus.Closed;
                    break;
                default:
                    handled = false;
                    break;
            }

            return handled;
        }

        #region IChainOf<IMidiReceiver> Members

        /// <summary>
        /// Backing field for the <see cref="Next"/> property.
        /// </summary>
        private IMidiDataReceiver receiver;

        /// <summary>
        /// Gets or sets the next <see cref="IMidiDataReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        public IMidiDataReceiver Next
        {
            get
            {
                return this.receiver;
            }

            set
            {
                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                this.receiver = value;
            }
        }

        #endregion IChainOf<IMidiReceiver> Members

        #region IChainOf<IMidiErrorReceiver> Members

        /// <summary>
        /// Backing field for the <see cref="NextErrorReceiver"/> property.
        /// </summary>
        private IMidiDataErrorReceiver errorReceiver;

        /// <summary>
        /// Gets or sets the next <see cref="IMidiDataErrorReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        IMidiDataErrorReceiver IChainOf<IMidiDataErrorReceiver>.Next
        {
            get
            {
                return this.errorReceiver;
            }

            set
            {
                ThrowIfDisposed();
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                this.errorReceiver = value;
            }
        }

        /// <summary>
        /// Gets or sets the next <see cref="IMidiDataErrorReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        public IMidiDataErrorReceiver NextErrorReceiver
        {
            get
            {
                return this.errorReceiver;
            }

            set
            {
                ThrowIfDisposed();
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                this.errorReceiver = value;
            }
        }

        #endregion IChainOf<IMidiErrorReceiver> Members

        #region IChainOf<IMidiPortEventReceiver> Members

        /// <summary>
        /// Backing field for the <see cref="NextPortEventReceiver"/> property.
        /// </summary>
        private IMidiPortEventReceiver portEventReceiver;

        /// <summary>
        /// Gets or sets a reference to the next port event receiver.
        /// </summary>
        IMidiPortEventReceiver IChainOf<IMidiPortEventReceiver>.Next
        {
            get
            {
                return this.portEventReceiver;
            }

            set
            {
                ThrowIfDisposed();
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                this.portEventReceiver = value;
            }
        }

        /// <summary>
        /// Gets or sets the next receiver for <see cref="MidiPortEvent"/>s.
        /// </summary>
        public IMidiPortEventReceiver NextPortEventReceiver
        {
            get
            {
                return this.portEventReceiver;
            }

            set
            {
                ThrowIfDisposed();
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                this.portEventReceiver = value;
            }
        }

        #endregion IChainOf<IMidiPortEventReceiver> Members

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">True to also dispose of the managed resources.</param>
        /// <remarks>Closes the Midi In Port. If <paramref name="disposing"/> is true
        /// the <see cref="P:BufferManager"/> and the <see cref="Next"/> and the
        /// <see cref="NextErrorReceiver"/> are set to null.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                base.Dispose(disposing);

                // we dispose the buffer manager last.
                // base.Dispose can call Close and that needs a working buffer manager.
                if (this.bufferManager != null)
                {
                    this.bufferManager.Dispose();
                    this.bufferManager = null;
                }

                if (disposing)
                {
                    this.receiver = null;
                    this.errorReceiver = null;
                    this.portEventReceiver = null;
                }
            }
        }

        /// <summary>
        /// Backing field for the <see cref="BufferManager"/> property.
        /// </summary>
        private MidiInBufferManager bufferManager;

        /// <summary>
        /// Gets the buffer manager for the Midi In Port.
        /// </summary>
        public MidiInBufferManager BufferManager
        {
            get
            {
                if (this.bufferManager == null)
                {
                    this.bufferManager = new MidiInBufferManager(this);
                }

                return this.bufferManager;
            }
        }

        /// <summary>
        /// Returns the capabilities for the specified <paramref name="portId"/>.
        /// </summary>
        /// <param name="portId">An index into the list of available in ports.</param>
        /// <returns>Never returns null.</returns>
        public static MidiInPortCaps GetPortCapabilities(int portId)
        {
            MidiInCaps caps = new MidiInCaps();

            int result = NativeMethods.midiInGetDevCaps(
                new IntPtr(portId), ref caps, (uint)MemoryUtil.SizeOfMidiInCaps);

            ThrowIfError(result);

            return new MidiInPortCaps(ref caps);
        }
    }
}