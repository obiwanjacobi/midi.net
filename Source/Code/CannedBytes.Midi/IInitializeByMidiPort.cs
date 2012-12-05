namespace CannedBytes.Midi
{
    /// <summary>
    /// The IInitializeByMidiPort interface is implemented by midi
    /// chain components that need a reference to a midi port to
    /// initialize their internal state.
    /// </summary>
    /// <remarks>
    /// Depending on the type of chain the component is in the <see cref="IMidiPort"/>
    /// interface will reference to one of the midi port implementations.
    /// </remarks>
    public interface IInitializeByMidiPort
    {
        /// <summary>
        /// Called after construction to initialize the instance.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        void Initialize(IMidiPort port);

        /// <summary>
        /// Called before disposing the instance.
        /// </summary>
        /// <param name="port">Must not be null.</param>
        void Uninitialize(IMidiPort port);
    }
}