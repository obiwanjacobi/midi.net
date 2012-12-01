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
            ThrowIfDisposed();
            //Throw.IfArgumentOutOfRange(portId, 0, NativeMethods.midiInGetNumDevs() - 1, "portId");

            base.Open(portId);

            MidiInSafeHandle inHandle;

            int result = NativeMethods.midiInOpen(out inHandle, (uint)portId,
                _midiProc, ToIntPtr(),
                NativeMethods.CALLBACK_FUNCTION | NativeMethods.MIDI_IO_STATUS);

            ThrowIfError(result);

            MidiSafeHandle = inHandle;

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

            if (MidiBufferManager.UsedBufferCount > 0)
            {
                // Reset returns the buffers from the port
                Reset();

                // wait until all buffers are returned
                bool success = MidiBufferManager.WaitForBuffersReturned(
                    global::System.Threading.Timeout.Infinite);

                // should always work with infinite timeout
                Debug.Assert(success);

                MidiBufferManager.UnPrepareAllBuffers();
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
                        Status = MidiPortStatus.Open;
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
            if (!IsDisposed)
            {
                base.Dispose(disposing);

                // we dispose the buffer manager last.
                // base.Dispose can call Close and that needs a working buffer manager.
                if (_bufferManager != null)
                {
                    _bufferManager.Dispose();
                    _bufferManager = null;
                }

                if (disposing)
                {
                    _receiver = null;
                    _errorReceiver = null;
                }
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