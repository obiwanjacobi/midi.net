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
            Status = MidiPortStatus.Open | MidiPortStatus.Pending;

            MidiOutSafeHandle outHandle;

            int result = NativeMethods.midiOutOpen(out outHandle, (uint)portId,
                MidiProcRef, ToIntPtr(), NativeMethods.CALLBACK_FUNCTION);

            ThrowIfError(result);

            MidiSafeHandle = outHandle;

            base.Open(portId);
        }
    }
}