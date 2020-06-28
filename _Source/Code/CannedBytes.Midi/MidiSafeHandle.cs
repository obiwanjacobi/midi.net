namespace CannedBytes.Midi
{
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// The MidiSafeHandle represents a handle to a midi device/port.
    /// </summary>
    public abstract class MidiSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Constructs a new instance that owns the handle.
        /// </summary>
        protected MidiSafeHandle()
            : base(true)
        {
        }
    }
}