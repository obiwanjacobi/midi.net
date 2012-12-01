namespace CannedBytes
{
    /// <summary>
    /// The IChainOf interface is implemented by chain components
    /// that implement interface T.
    /// </summary>
    /// <typeparam name="T">A chain interface.</typeparam>
    public interface IChainOf<T>
    {
        /// <summary>
        /// Gets or sets the next implementation of interface T this
        /// instance will call.
        /// </summary>
        T Next { get; set; }
    }
}