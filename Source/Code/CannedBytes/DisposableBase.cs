using System;
using System.Globalization;

namespace CannedBytes
{
    /// <summary>
    /// Implements the Dispose implementation pattern as a base class.
    /// </summary>
    /// <remarks>
    /// see also http://obiwanjacobi.blogspot.nl/2006/12/two-layers-of-disposability.html
    /// </remarks>
    public abstract class DisposableBase : IDisposable
    {
        /// <summary>
        /// Call to dispose of this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Called either from <see cref="M:Dispose"/> or the Finalizer (not in this base class)
        /// to dispose of this instance.
        /// </summary>
        /// <param name="disposing">True when called from <see cref="M:Dispose"/>.
        /// False when called from the Finalizer.</param>
        /// <remarks>Derived classes override to Dispose their members.</remarks>
        protected virtual void Dispose(bool disposing)
        {
            //if (!_disposed)
            //{
            //    if (disposing)
            //    {
            //        // dispose managed resources
            //    }
            //    // dispose unmanaged resources
            //}

            IsDisposed = true;
        }

        /// <summary>
        /// Gets a value indicating if this instance has been disposed.
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Throws an exception if the instance has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the instance is disposed.</exception>
        protected virtual void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(
                    String.Format(CultureInfo.InvariantCulture,
                        Properties.Resources.DisposableBase_ObjectDisposed,
                        GetType().FullName));
            }
        }
    }
}