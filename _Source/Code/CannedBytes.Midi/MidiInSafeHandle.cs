namespace CannedBytes.Midi
{
    /// <summary>
    /// SafeHandle implementation for a MidiInPort.
    /// </summary>
    internal class MidiInSafeHandle : MidiSafeHandle
    {
        /// <summary>
        /// Closes the port handle.
        /// </summary>
        /// <returns>Returns true when successful.</returns>
        protected override bool ReleaseHandle()
        {
            int result = NativeMethods.midiInClose(handle);

            return result == NativeMethods.MMSYSERR_NOERROR;
        }
    }
}