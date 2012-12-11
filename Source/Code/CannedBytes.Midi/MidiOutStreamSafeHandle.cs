namespace CannedBytes.Midi
{
    /// <summary>
    /// SafeHandle implementation for a MidiOutStreamPort.
    /// </summary>
    internal class MidiOutStreamSafeHandle : MidiSafeHandle
    {
        /// <summary>
        /// Closes the port handle (with retry).
        /// </summary>
        /// <returns>Returns true when successful.</returns>
        protected override bool ReleaseHandle()
        {
            int result = NativeMethods.midiStreamClose(handle);

            if (result == NativeMethods.MIDIERR_STILLPLAYING)
            {
                NativeMethods.midiOutReset(this);

                result = NativeMethods.midiStreamClose(handle);
            }

            return result == NativeMethods.MMSYSERR_NOERROR;
        }
    }
}