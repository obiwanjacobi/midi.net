using System;
using System.Diagnostics;
using System.Text;

namespace CannedBytes.Midi
{
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

            int result = NativeMethods.midiInOpen(out inHandle, (uint)portId,
                _midiProc, ToIntPtr(),
                NativeMethods.CALLBACK_FUNCTION | NativeMethods.MIDI_IO_STATUS);

            ThrowIfError(result);

            MidiSafeHandle = inHandle;

            Status = MidiPortStatus.Open;

            MidiBufferManager.RegisterAllBuffers();
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

            Status = MidiPortStatus.Closed | MidiPortStatus.Pending;

            if (MidiBufferManager.UsedBufferCount > 0)
            {
                // Reset returns the buffers from the port
                Reset();

                // wait until all buffers are returned
                bool success = MidiBufferManager.WaitForBuffersReturned(
                    global::System.Threading.Timeout.Infinite);

                // should always work with infinite timeout
                Debug.Assert(success);

                MidiBufferManager.UnprepareAllBuffers();
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
            #region Method checks

            ThrowIfDisposed();
            if (!IsOpen)
            {
                throw new MidiInPortException(Properties.Resources.MidiInPort_PortNotOpen);
            }
            // cannot start the in port before connecting it to a receiver
            if (Next == null)
            {
                throw new MidiInPortException(Properties.Resources.MidiInPort_NoReceiver);
            }
            // Not an error. What if we only want to receive short messages?
            //if (!MidiBufferManager.IsInitialized)
            //{
            //    throw new MidiInPortException(Properties.Resources.MidiInPort_BufferManagerNotInitialzed);
            //}

            #endregion Method checks

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
        /// <param name="msg">The midi message to handle.</param>
        /// <param name="param1">Parameter 1</param>
        /// <param name="param2">Parameter 2</param>
        protected override bool OnMessage(int msg, IntPtr param1, IntPtr param2)
        {
            bool handled = true;

            try
            {
                MidiBufferStream buffer = null;
                uint umsg = (uint)msg;

                switch (umsg)
                {
                    case NativeMethods.MIM_OPEN:
                        // don't change status here, MidiSafeHandle has not been set yet.
                        break;
                    case NativeMethods.MIM_CLOSE:
                        MidiSafeHandle = null;
                        Status = MidiPortStatus.Closed;
                        break;
                    case NativeMethods.MIM_LONGDATA:
                    case NativeMethods.MIM_LONGERROR:
                        buffer = MidiBufferManager.FindBuffer(param1);

                        if (buffer == null)
                        {
                            Debug.WriteLine("Buffer was not found in Message Proc.");
                            return false;
                        }

                        handled = false; // not handled yet.
                        break;
                    default:
                        handled = false;
                        break;
                }

                if (Next != null && handled == false)
                {
                    handled = true;

                    switch (umsg)
                    {
                        case NativeMethods.MIM_DATA:
                            Next.ShortData(param1.ToInt32(), param2.ToInt32());
                            break;
                        case NativeMethods.MIM_LONGDATA:
                            if (buffer.BytesRecorded > 0)
                            {
                                Next.LongData(buffer, param2.ToInt32());

                                if (AutoReturnBuffers)
                                {
                                    this.MidiBufferManager.Return(buffer);
                                }
                            }
                            else
                            {
                                this.MidiBufferManager.Return(buffer);
                            }
                            break;
                        case NativeMethods.MIM_MOREDATA:
                            Next.ShortData(param1.ToInt32(), param2.ToInt32());
                            break;
                        default:
                            handled = false;
                            break;
                    }
                }

                if (NextErrorReceiver != null && handled == false)
                {
                    handled = true;

                    switch (umsg)
                    {
                        case NativeMethods.MIM_ERROR:
                            NextErrorReceiver.ShortError(param1.ToInt32(), param2.ToInt32());
                            break;
                        case NativeMethods.MIM_LONGERROR:
                            NextErrorReceiver.LongError(buffer, param2.ToInt32());

                            if (AutoReturnBuffers)
                            {
                                MidiBufferManager.Return(buffer);
                            }
                            break;
                        default:
                            handled = false;
                            break;
                    }
                }

                if (NextPortEventReceiver != null && handled == false)
                {
                    handled = true;

                    switch (umsg)
                    {
                        case NativeMethods.MIM_DATA:
                            NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.ShortData, param1.ToInt32(), param2.ToInt32()));
                            break;
                        case NativeMethods.MIM_LONGDATA:
                            if (buffer.BytesRecorded > 0)
                            {
                                NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.LongData, buffer, param2.ToInt32()));

                                if (AutoReturnBuffers)
                                {
                                    this.MidiBufferManager.Return(buffer);
                                }
                            }
                            else
                            {
                                this.MidiBufferManager.Return(buffer);
                            }
                            break;
                        case NativeMethods.MIM_MOREDATA:
                            NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.MoreData, param1.ToInt32(), param2.ToInt32()));
                            break;
                        case NativeMethods.MIM_ERROR:
                            NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.ShortError, param1.ToInt32(), param2.ToInt32()));
                            break;
                        case NativeMethods.MIM_LONGERROR:
                            NextPortEventReceiver.PortEvent(new MidiPortEvent(MidiPortEventTypes.LongError, buffer, param2.ToInt32()));

                            if (AutoReturnBuffers)
                            {
                                MidiBufferManager.Return(buffer);
                            }
                            break;
                        default:
                            handled = false;
                            break;
                    }
                }

                if (handled == false)
                {
                    switch (umsg)
                    {
                        case NativeMethods.MIM_LONGDATA:
                        case NativeMethods.MIM_LONGERROR:
                            // make sure buffers are returned when there's no handler to take care of it.
                            MidiBufferManager.Return(buffer);
                            handled = true;
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

        #region IChainOf<IMidiReceiver> Members

        private IMidiDataReceiver receiver;

        /// <summary>
        /// Gets or sets the next <see cref="IMidiDataReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        public IMidiDataReceiver Next
        {
            get { return this.receiver; }
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

        private IMidiDataErrorReceiver errorReceiver;

        /// <summary>
        /// Gets or sets the next <see cref="IMidiDataErrorReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        IMidiDataErrorReceiver IChainOf<IMidiDataErrorReceiver>.Next
        {
            get { return this.errorReceiver; }
            set
            {
                #region Method checks

                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                #endregion Method checks

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
            get { return this.errorReceiver; }
            set
            {
                #region Method checks

                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                #endregion Method checks

                this.errorReceiver = value;
            }
        }

        #endregion IChainOf<IMidiErrorReceiver> Members

        #region IChainOf<IMidiPortEventReceiver> Members

        private IMidiPortEventReceiver portEventReceiver;

        IMidiPortEventReceiver IChainOf<IMidiPortEventReceiver>.Next
        {
            get { return this.portEventReceiver; }
            set
            {
                #region Method checks

                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                #endregion Method checks

                this.portEventReceiver = value;
            }
        }

        /// <summary>
        /// Gets or sets the next receiver for <see cref="MidiPortEvent"/>s.
        /// </summary>
        public IMidiPortEventReceiver NextPortEventReceiver
        {
            get { return this.portEventReceiver; }
            set
            {
                #region Method checks

                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                #endregion Method checks

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
                if (bufferManager != null)
                {
                    bufferManager.Dispose();
                    bufferManager = null;
                }

                if (disposing)
                {
                    receiver = null;
                    errorReceiver = null;
                }
            }
        }

        private MidiInBufferManager bufferManager;

        /// <summary>
        /// Gets the buffer manager for the Midi In Port.
        /// </summary>
        public MidiInBufferManager MidiBufferManager
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