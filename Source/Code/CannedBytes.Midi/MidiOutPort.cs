using System;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutPort class represents an interface to a physical (or virtual, depending on the driver)
    /// Midi Out Port.
    /// </summary>
    /// <remarks>
    /// Midi Ports are sometimes also called Midi Devices.
    /// </remarks>
    public class MidiOutPort : MidiOutPortBase
    {
        /// <summary>
        /// Opens the Midi Out Port identified by the <paramref name="portId"/>.
        /// </summary>
        /// <param name="portId">An index into the available Midi Out Ports.</param>
        /// <remarks>Refer to <see cref="MidiOutPortCapsCollection"/>.</remarks>
        public override void Open(int portId)
        {
            ThrowIfDisposed();

            MidiOutSafeHandle outHandle;

            int result = NativeMethods.midiOutOpen(out outHandle, (uint)portId,
                _midiProc, ToIntPtr(), NativeMethods.CALLBACK_FUNCTION);

            ThrowIfError(result);

            MidiSafeHandle = outHandle;

            base.Open(portId);
        }

        /// <summary>
        /// Midi out device callback handler.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        protected override bool OnMessage(int msg, IntPtr param1, IntPtr param2)
        {
            bool handled = true;

            switch ((uint)msg)
            {
                case NativeMethods.MOM_OPEN:
                    ModifyStatus(MidiPortStatus.Open, MidiPortStatus.Pending);
                    break;
                case NativeMethods.MOM_CLOSE:
                    ModifyStatus(MidiPortStatus.Closed, MidiPortStatus.Pending);
                    MidiSafeHandle = null;
                    break;
                case NativeMethods.MOM_DONE:
                    MidiHeader header = MemoryUtil.Unpack<MidiHeader>(param1);
                    MidiBufferStream buffer = MidiBufferManager.FindBuffer(ref header);
                    MidiBufferManager.Return(buffer);
                    break;
                case NativeMethods.MOM_POSITIONCB:
                    //MidiHeader header = MidiUtil.Unpack<MidiHeader>(param1);
                    // TODO: raise event?
                    break;
                default:
                    handled = false;
                    break;
            }

            return handled;
        }

        private MidiOutBufferManager _bufferManager;

        /// <summary>
        /// Gets the buffer manager for the Midi In Port.
        /// </summary>
        public MidiOutBufferManager MidiBufferManager
        {
            get
            {
                if (_bufferManager == null)
                {
                    _bufferManager = new MidiOutBufferManager(this);
                }

                return _bufferManager;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_bufferManager != null)
                {
                    _bufferManager.Dispose();
                    _bufferManager = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}