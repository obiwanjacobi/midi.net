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
    /// Midi Ports are sometimes also called Midi Devices.
    /// </remarks>
    public class MidiInPort : MidiPort,
        IChainOf<IMidiReceiver>,
        IChainOf<IMidiErrorReceiver>
    {
        /// <summary>
        /// Opens the Midi In Port identified by the <paramref name="portId"/>.
        /// </summary>
        /// <param name="portId">An index into the available Midi In Ports.</param>
        /// <remarks>Refer to <see cref="MidiInPortCapsCollection"/>.</remarks>
        public override void Open(int portId)
        {
            #region Method checks

            ThrowIfDisposed();
            //Throw.IfArgumentOutOfRange(portId, 0, NativeMethods.midiInGetNumDevs() - 1, "portId");

            #endregion Method checks

            base.Open(portId);

            MidiInSafeHandle inHandle;

            int result = NativeMethods.midiInOpen(out inHandle, (uint)portId,
                _midiProc, ToIntPtr(),
                NativeMethods.CALLBACK_FUNCTION | NativeMethods.MIDI_IO_STATUS);

            ThrowIfError(result);

            MidiSafeHandle = inHandle;
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
            #region Method checks

            ThrowIfDisposed();

            #endregion Method checks

            if (HasStatus(MidiPortStatus.Started))
            {
                Stop();
            }

            if (MidiBufferManager.UsedBufferCount > 0)
            {
                // Reset returns the buffers from the port
                Reset();

                base.Close();

                // wait until all buffers are returned
                bool success = MidiBufferManager.WaitForBuffersReturned(
                    global::System.Threading.Timeout.Infinite);

                // should always work with infinite timeout
                Debug.Assert(success);
            }
        }

        /// <summary>
        /// Resets the Midi In Port returning all buffers to the <see cref="P:BufferManager"/>.
        /// </summary>
        public override void Reset()
        {
            #region Method checks

            ThrowIfDisposed();

            #endregion Method checks

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
            // cannot start the in port before connecting it to a receiver
            if (Next == null)
            {
                throw new MidiInPortException(Properties.Resources.MidiInPort_NoReceiver);
            }
            // NOTE: by accessing the MidiBufferManager we make sure it is created and that the
            // buffers are registered.
            if (!MidiBufferManager.IsInitialized)
            {
                throw new MidiInPortException(Properties.Resources.MidiInPort_BufferManagerNotInitialzed);
            }

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
            #region Method checks

            ThrowIfDisposed();

            #endregion Method checks

            int result = NativeMethods.midiInStop(MidiSafeHandle);

            ThrowIfError(result);

            ModifyStatus(MidiPortStatus.Stopped, MidiPortStatus.Started);
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

            switch ((uint)msg)
            {
                case NativeMethods.MIM_OPEN:
                    ModifyStatus(MidiPortStatus.Open, MidiPortStatus.Pending);
                    MidiBufferManager.RegisterAllBuffers();
                    break;
                case NativeMethods.MIM_CLOSE:
                    MidiSafeHandle = null;
                    ModifyStatus(MidiPortStatus.Closed, MidiPortStatus.Pending);
                    break;
                default:
                    handled = false;
                    break;
            }

            if (Next != null && handled == false)
            {
                handled = true;

                switch ((uint)msg)
                {
                    case NativeMethods.MIM_DATA:
                        Next.ShortData(param1.ToInt32(), param2.ToInt32());
                        break;
                    case NativeMethods.MIM_LONGDATA:
                        MidiHeader header = MemoryUtil.Unpack<MidiHeader>(param1);
                        MidiBufferStream buffer = MidiBufferManager.FindBuffer(ref header);

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

                switch ((uint)msg)
                {
                    case NativeMethods.MIM_ERROR:
                        NextErrorReceiver.ShortError(param1.ToInt32(), param2.ToInt32());
                        break;
                    case NativeMethods.MIM_LONGERROR:
                        MidiHeader error = MemoryUtil.Unpack<MidiHeader>(param1);
                        MidiBufferStream longError = MidiBufferManager.FindBuffer(ref error);

                        if (longError.BytesRecorded > 0)
                        {
                            NextErrorReceiver.LongError(longError, param2.ToInt32());

                            if (AutoReturnBuffers)
                            {
                                MidiBufferManager.Return(longError);
                            }
                        }
                        else
                        {
                            MidiBufferManager.Return(longError);
                        }
                        break;
                    default:
                        handled = false;
                        break;
                }
            }

            if (handled == false)
            {
                switch ((uint)msg)
                {
                    case NativeMethods.MIM_LONGDATA:
                    case NativeMethods.MIM_LONGERROR:
                        // make sure buffers are returned when there's no handler to take care of it.
                        MidiHeader header = MemoryUtil.Unpack<MidiHeader>(param1);
                        MidiBufferStream buffer = MidiBufferManager.FindBuffer(ref header);
                        MidiBufferManager.Return(buffer);
                        handled = true;
                        break;
                }
            }

            return handled;
        }

        #region IChainOf<IMidiReceiver> Members

        private IMidiReceiver _receiver;

        /// <summary>
        /// Gets or sets the next <see cref="IMidiReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        public IMidiReceiver Next
        {
            get { return _receiver; }
            set
            {
                #region Method checks

                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                #endregion Method checks

                _receiver = value;
            }
        }

        #endregion IChainOf<IMidiReceiver> Members

        #region IChainOf<IMidiErrorReceiver> Members

        private IMidiErrorReceiver _errorReceiver;

        /// <summary>
        /// Gets or sets the next <see cref="IMidiErrorReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        IMidiErrorReceiver IChainOf<IMidiErrorReceiver>.Next
        {
            get { return _errorReceiver; }
            set
            {
                #region Method checks

                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                #endregion Method checks

                _errorReceiver = value;
            }
        }

        /// <summary>
        /// Gets or sets the next <see cref="IMidiErrorReceiver"/> implementation.
        /// </summary>
        /// <remarks>The interface will be called directly from the method that handles the
        /// driver callbacks. Calls will be made on a new thread.</remarks>
        public IMidiErrorReceiver NextErrorReceiver
        {
            get { return _errorReceiver; }
            set
            {
                #region Method checks

                ThrowIfDisposed();

                if (HasStatus(MidiPortStatus.Started))
                {
                    throw new MidiInPortException(Properties.Resources.MidiInPort_CannotChangeReceiver);
                }

                #endregion Method checks

                _errorReceiver = value;
            }
        }

        #endregion IChainOf<IMidiErrorReceiver> Members

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">True to also dispose of the managed resources.</param>
        /// <remarks>Closes the Midi In Port. If <paramref name="disposing"/> is true
        /// the <see cref="P:BufferManager"/> and the <see cref="NextReceiver"/> and the
        /// <see cref="NextErrorReceiver"/> are set to null.</remarks>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_bufferManager != null)
                {
                    _bufferManager.Dispose();
                    _bufferManager = null;
                }

                if (!IsDisposed)
                {
                    if (disposing)
                    {
                        _receiver = null;
                        _errorReceiver = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private MidiInBufferManager _bufferManager;

        /// <summary>
        /// Gets the buffer manager for the Midi In Port.
        /// </summary>
        public MidiInBufferManager MidiBufferManager
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