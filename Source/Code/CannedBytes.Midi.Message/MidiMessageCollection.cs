namespace CannedBytes.Midi.Message
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Manages a collection of <see cref="IMidiMessage"/> instances.
    /// </summary>
    public class MidiMessageCollection : Collection<IMidiMessage>
    {
    }
}