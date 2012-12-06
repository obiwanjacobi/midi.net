using System;

namespace CannedBytes
{
    /// <summary>
    /// Implements the Dispose implementation pattern for classes that own unmanaged resources.
    /// </summary>
    /// <remarks>
    /// see also http://obiwanjacobi.blogspot.nl/2006/12/two-layers-of-disposability.html
    /// </remarks>
    public abstract class UnmanagedDisposableBase : DisposableBase
    {
        /// <summary>
        /// Called when the instance should be disposed.
        /// </summary>
        /// <param name="disposing">True when called from <see cref="Dispose"/>.
        /// False when called from the Finalizer.</param>
        /// <remarks>Finalization of the instance is suppressed when <paramref name="disposing"/> is true.</remarks>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    GC.SuppressFinalize(this);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Destructor called by the GC.
        /// </summary>
        ~UnmanagedDisposableBase()
        {
            Dispose(false);
        }
    }
}