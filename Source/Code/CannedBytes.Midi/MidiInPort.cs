namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics;
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
        /// Opens the Midi In Port identified by the <paramref name="deviceId"/>.
        /// </summary>
        /// <param name="deviceId">An index into the available Midi In Ports.</param>
        /// <remarks>Refer to <see cref="MidiInPortCapsCollection"/>.</remarks>
        public override void Open(int deviceId)
        {
            ThrowIfDisposed();
            Check.IfArgumentOutOfRange(deviceId, 0, NativeMethods.midiInGetNumDevs() - 1, "deviceId");

            base.Open(deviceId);

            int result = NativeMethods.midiInOpen(
                         out MidiInSafeHandle inHandle,
                         (uint)deviceId,
                         MidiProcRef,
                         ToIntPtr(),
                         NativeMethods.CALLBACK_FUNCTION | NativeMethods.MIDI_IO_STATUS);

            ThrowIfError(result);

            MidiSafeHandle = inHandle;

            Status = MidiPortStatus.Open;

            BufferManager.RegisterAllBuffers();
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
                Stop();
            }

            if (_bufferManager != null)
            {
                if (_bufferManager.UsedBufferCount > 0)
                {
                    // Reset returns the buffers from the port
                    Reset();

                    // wait until all buffers are returned
                    bool success = _bufferManager.WaitForBuffersReturned(Timeout.Infinite);

                    // should always work with infinite timeout
                    Debug.Assert(success, "Infinite timeout still failed.");
                }

                Status = MidiPortStatus.Closed | MidiPortStatus.Pending;

                _bufferManager.UnprepareAllBuffers();
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
            if (Successor == null && NextPortEventReceiver == null)
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
        /// <param name="message">The type of message to handle.</param>
        /// <param name="parameter1">Parameter 1.</param>
        /// <param name="parameter2">Parameter 2.</param>
        /// <returns>Returns true when handled.</returns>
        protected override bool OnMessage(int message, IntPtr parameter1, IntPtr parameter2)
        {
            bool handled;

            uint umsg = (uint)message;

            try
            {
                handled = HandleOpenAndClose(umsg);

                if (Successor != null && !handled)
                {
                    handled = HandleDataMessage(umsg, parameter1, parameter2);
                }

                if (NextErrorReceiver != null && !handled)
                {
                    handled = HandleErrorMessage(umsg, parameter1, parameter2);
                }

                if (NextPortEventReceiver != null && !handled)
                {
                    handled = HandlePortEvent(umsg, parameter1, parameter2);
                }

                if (!handled)
                {
                    handled = HandleUnhandledMessage(umsg, parameter1);
                }
            }
            catch
            {
                HandleUnhandledMessage(umsg, parameter1);
                throw;
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
            bool handled = true;

            switch (umsg)
            {
                case NativeMethods.MIM_DATA:
                    Successor.ShortData(param1.ToInt32(), param2.ToInt32());
                    break;
                case NativeMethods.MIM_LONGDATA:
                    var buffer = BufferManager.FindBuffer(param1);

                    if (buffer != null)
                    {
                        if (buffer.BytesRecorded > 0)
                        {
                            Successor.LongData(buffer, param2.ToInt32());

                            if (AutoReturnBuffers)
                            {
                                BufferManager.ReturnBuffer(buffer);
                            }
                        }
                        else
                        {
                            BufferManager.ReturnBuffer(buffer);
                        }
                    }
                    break;
                case NativeMethods.MIM_MOREDATA:
                    Successor.ShortData(param1.ToInt32(), param2.ToInt32());
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
            bool handled = true;

            switch (umsg)
            {
                case NativeMethods.MIM_DATA:
                    NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventType.ShortData, param1.ToInt32(), param2.ToInt32()));
                    break;
                case NativeMethods.MIM_LONGDATA:
                    var buffer = BufferManager.FindBuffer(param1);

                    if (buffer != null)
                    {
                        if (buffer.BytesRecorded > 0)
                        {
                            NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventType.LongData, buffer, param2.ToInt32()));

                            if (AutoReturnBuffers)
                            {
                                BufferManager.ReturnBuffer(buffer);
                            }
                        }
                        else
                        {
                            BufferManager.ReturnBuffer(buffer);
                        }
                    }
                    break;
                case NativeMethods.MIM_MOREDATA:
                    NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventType.MoreData, param1.ToInt32(), param2.ToInt32()));
                    break;
                case NativeMethods.MIM_ERROR:
                    NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventType.ShortError, param1.ToInt32(), param2.ToInt32()));
                    break;
                case NativeMethods.MIM_LONGERROR:
                    var errBuffer = BufferManager.FindBuffer(param1);

                    if (errBuffer != null)
                    {
                        NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventType.LongError, errBuffer, param2.ToInt32()));

                        if (AutoReturnBuffers)
                        {
                            BufferManager.ReturnBuffer(errBuffer);
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
            bool handled = true;

            switch (umsg)
            {
                case NativeMethods.MIM_ERROR:
                    NextErrorReceiver.ShortError(param1.ToInt32(), param2.ToInt32());
                    break;
                case NativeMethods.MIM_LONGERROR:
                    var buffer = BufferManager.FindBuffer(param1);

                    if (buffer != null)
                    {
                        NextErrorReceiver.LongError(buffer, param2.ToInt32());

                        if (AutoReturnBuffers)
                        {
                            BufferManager.ReturnBuffer(buffer);
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
        /// Returns buffers to the buffer manager if the message is unhandled.
        /// </summary>
        /// <param name="umsg">The type of message.</param>
        /// <param name="parameter1">The midi header pointer.</param>
        /// <returns>Returns true when buffers were returned.</returns>
        private bool HandleUnhandledMessage(uint umsg, IntPtr parameter1)
        {
            bool handled = false;

            switch (umsg)
            {
                case NativeMethods.MIM_LONGDATA:
                case NativeMethods.MIM_LONGERROR:
                    var buffer = BufferManager.FindBuffer(parameter1);

                    if (buffer != null)
                    {
                        // make sure buffers are returned when there's no handler to take care of it.
                        BufferManager.ReturnBuffer(buffer);
                        handled = true;
                    }
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
        /// Backing field for the <see cref="Successor"/> property.
        /// </summary>
        private IMidiDataReceiver _receiver;

        /// <summary>
        /// Gets or sets the next <see cref="IMidiDataReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        public IMidiDataReceiver Successor
        {
            get
            {
                return _receiver;
            }

            set
            {
                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                _receiver = value;
            }
        }

        #endregion IChainOf<IMidiReceiver> Members

        #region IChainOf<IMidiErrorReceiver> Members

        /// <summary>
        /// Backing field for the <see cref="NextErrorReceiver"/> property.
        /// </summary>
        private IMidiDataErrorReceiver _errorReceiver;

        /// <summary>
        /// Gets or sets the next <see cref="IMidiDataErrorReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        IMidiDataErrorReceiver IChainOf<IMidiDataErrorReceiver>.Successor
        {
            get
            {
                return _errorReceiver;
            }

            set
            {
                ThrowIfDisposed();
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                _errorReceiver = value;
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
                return _errorReceiver;
            }

            set
            {
                ThrowIfDisposed();
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                _errorReceiver = value;
            }
        }

        #endregion IChainOf<IMidiErrorReceiver> Members

        #region IChainOf<IMidiPortEventReceiver> Members

        /// <summary>
        /// Backing field for the <see cref="NextPortEventReceiver"/> property.
        /// </summary>
        private IMidiPortEventReceiver _portEventReceiver;

        /// <summary>
        /// Gets or sets a reference to the next port event receiver.
        /// </summary>
        IMidiPortEventReceiver IChainOf<IMidiPortEventReceiver>.Successor
        {
            get
            {
                return _portEventReceiver;
            }

            set
            {
                ThrowIfDisposed();
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                _portEventReceiver = value;
            }
        }

        /// <summary>
        /// Gets or sets the next receiver for <see cref="MidiPortEvent"/>s.
        /// </summary>
        public IMidiPortEventReceiver NextPortEventReceiver
        {
            get
            {
                return _portEventReceiver;
            }

            set
            {
                ThrowIfDisposed();
                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                _portEventReceiver = value;
            }
        }

        #endregion IChainOf<IMidiPortEventReceiver> Members

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        /// <remarks>Closes the Midi In Port. If <paramref name="disposeKind"/> is Managed
        /// the <see cref="P:BufferManager"/> and the <see cref="Successor"/> and the
        /// <see cref="NextErrorReceiver"/> are set to null.</remarks>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            base.Dispose(disposeKind);

            if (!IsDisposed &&
                disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                // we dispose the buffer manager last.
                // base.Dispose can call Close and that needs a working buffer manager.
                if (_bufferManager != null)
                {
                    _bufferManager.Dispose();
                }

                _receiver = null;
                _errorReceiver = null;
                _portEventReceiver = null;
            }
        }

        /// <summary>
        /// Backing field for the <see cref="BufferManager"/> property.
        /// </summary>
        private MidiInBufferManager _bufferManager;

        /// <summary>
        /// Gets the buffer manager for the Midi In Port.
        /// </summary>
        public MidiInBufferManager BufferManager
        {
            get
            {
                if (_bufferManager == null)
                {
                    _bufferManager = new MidiInBufferManager(this);
                }

                return _bufferManager;
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