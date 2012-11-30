using System.Collections.ObjectModel;

namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Manages a list (collection) of <see cref="IMidiMessage"/> instances.
    /// </summary>
    public class MidiMessageList : Collection<IMidiMessage>
    {
    }
}