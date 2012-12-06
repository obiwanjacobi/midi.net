using Microsoft.Win32.SafeHandles;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiSafeHandle represents a handle to a midi device/port.
    /// </summary>
    public abstract class MidiSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Constructs a new instance that owns the handle.
        /// </summary>
        public MidiSafeHandle()
            : base(true)
        { }
    }

    /// <summary>
    /// SafeHandle implementation for a MidiInPort
    /// </summary>
    internal class MidiInSafeHandle : MidiSafeHandle
    {
        protected override bool ReleaseHandle()
        {
            int result = NativeMethods.midiInClose(base.handle);

            if (result == NativeMethods.MIDIERR_STILLPLAYING)
            {
                NativeMethods.midiInReset(this);

                result = NativeMethods.midiInClose(base.handle);
            }

            return (result == NativeMethods.MMSYSERR_NOERROR);
        }
    }

    /// <summary>
    /// SafeHandle implementation for a MidiOutPort
    /// </summary>
    internal class MidiOutSafeHandle : MidiSafeHandle
    {
        protected override bool ReleaseHandle()
        {
            int result = NativeMethods.midiOutClose(base.handle);

            if (result == NativeMethods.MIDIERR_STILLPLAYING)
            {
                NativeMethods.midiOutReset(this);

                result = NativeMethods.midiOutClose(base.handle);
            }

            return (result == NativeMethods.MMSYSERR_NOERROR);
        }
    }

    /// <summary>
    /// SafeHandle implementation for a MidiOutStreamPort
    /// </summary>
    internal class MidiOutStreamSafeHandle : MidiSafeHandle
    {
        protected override bool ReleaseHandle()
        {
            int result = NativeMethods.midiStreamClose(base.handle);

            if (result == NativeMethods.MIDIERR_STILLPLAYING)
            {
                NativeMethods.midiOutReset(this);

                result = NativeMethods.midiStreamClose(base.handle);
            }

            return (result == NativeMethods.MMSYSERR_NOERROR);
        }
    }
}