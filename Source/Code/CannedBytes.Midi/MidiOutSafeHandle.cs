namespace CannedBytes.Midi
{
    /// <summary>
    /// SafeHandle implementation for a MidiOutPort.
    /// </summary>
    internal class MidiOutSafeHandle : MidiSafeHandle
    {
        /// <summary>
        /// Closes the port handle (with retry).
        /// </summary>
        /// <returns>Returns true when successful.</returns>
        protected override bool ReleaseHandle()
        {
            int result = NativeMethods.midiOutClose(handle);

            if (result == NativeMethods.MIDIERR_STILLPLAYING)
            {
                result = NativeMethods.midiOutReset(this);

                result = NativeMethods.midiOutClose(handle);
            }

            return result == NativeMethods.MMSYSERR_NOERROR;
        }
    }
}