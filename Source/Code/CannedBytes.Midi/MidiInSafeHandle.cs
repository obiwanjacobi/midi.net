namespace CannedBytes.Midi
{
    /// <summary>
    /// SafeHandle implementation for a MidiInPort.
    /// </summary>
    internal class MidiInSafeHandle : MidiSafeHandle
    {
        /// <summary>
        /// Closes the port handle (with retry).
        /// </summary>
        /// <returns>Returns true when successful.</returns>
        protected override bool ReleaseHandle()
        {
            int result = NativeMethods.midiInClose(handle);

            //if (result == NativeMethods.MIDIERR_STILLPLAYING)
            //{
            //    result = NativeMethods.midiInReset(this);

            //    result = NativeMethods.midiInClose(handle);
            //}

            return result == NativeMethods.MMSYSERR_NOERROR;
        }
    }
}