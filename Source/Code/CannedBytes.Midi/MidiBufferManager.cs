using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiBufferManager base class implements unmanaged memory management
    /// for <see cref="MidiBufferStream"/>s that are used by the <see cref="MidiPort"/>s
    /// for sending and receiving sysex message or sending streams of midi events.
    /// </summary>
    public abstract class MidiBufferManager : UnmanagedDisposableBase
    {
        private IntPtr memHeaders = IntPtr.Zero;
        private IntPtr memBuffers = IntPtr.Zero;
        private object locker = new object();
        private List<MidiBufferStream> usedBuffers = new List<MidiBufferStream>();
        private Queue<MidiBufferStream> unusedBuffers = new Queue<MidiBufferStream>();
        private Dictionary<IntPtr, MidiBufferStream> mapBuffers = new Dictionary<IntPtr, MidiBufferStream>();
        private AutoResetEvent buffersReturnedEvent = new AutoResetEvent(false);

        /// <summary>
        /// For derived classes only.
        /// </summary>
        /// <param name="port">A reference to the midi port this buffer manager serves.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="port"/> is null.</exception>
        protected MidiBufferManager(IMidiPort port, FileAccess access)
        {
            Contract.Requires<ArgumentNullException>(port != null);

            MidiPort = port;
            StreamAccess = access;
        }

        /// <summary>
        /// Gets the MidiPort this buffer manager serves.
        /// </summary>
        public IMidiPort MidiPort { get; private set; }

        /// <summary>
        /// Gets the type of access the stream provides to the underlying data.
        /// </summary>
        public FileAccess StreamAccess { get; private set; }

        /// <summary>
        /// Gets the size (in bytes) of each buffer.
        /// </summary>
        public int BufferSize { get; private set; }

        /// <summary>
        /// Gets the number of buffers that will be available.
        /// </summary>
        public int BufferCount { get; private set; }

        /// <summary>
        /// Gets the number of buffers currently in use by the <see cref="MidiPort"/>.
        /// </summary>
        public int UsedBufferCount
        {
            get { ThrowIfDisposed(); return this.usedBuffers.Count; }
        }

        /// <summary>
        /// Gets the number of buffers that are currently not in use by the <see cref="MidiPort"/>.
        /// </summary>
        public int UnusedBufferCount
        {
            get { ThrowIfDisposed(); return this.unusedBuffers.Count; }
        }

        /// <summary>
        /// Indicates if the buffer manager has been initialized.
        /// </summary>
        /// <remarks>Call the <see cref="M:Initialze"/> method to initialize this instance.</remarks>
        public bool IsInitialized
        {
            get { return ((this.memHeaders != IntPtr.Zero || this.memBuffers != IntPtr.Zero)); }
        }

        /// <summary>
        /// Blocks the calling thread until all buffers are returned
        /// to the manager or the specified timeout period has elapsed.
        /// </summary>
        /// <param name="millisecondTimeout">A timeout in milliseconds.</param>
        /// <returns>Returns true if all buffers were returned or false when the timeout expired.</returns>
        public bool WaitForBuffersReturned(int millisecondTimeout)
        {
            ThrowIfDisposed();

            if (UsedBufferCount == 0)
            {
                return true;
            }

            return this.buffersReturnedEvent.WaitOne(millisecondTimeout, false);
        }

        /// <summary>
        /// Initializes the instance for use.
        /// </summary>
        /// <param name="bufferCount">The number of buffers that will be available.</param>
        /// <param name="bufferSize">The size (in bytes) of each buffer.</param>
        /// <remarks>Two blocks of continuous unmanaged memory will be allocated.
        /// Call the <see cref="M:Dispose"/> method to free that memory.
        /// Buffer manager instances that are owned by <see cref="MidiPort"/>s will
        /// be disposed when the port is disposed.</remarks>
        public virtual void Initialize(int bufferCount, int bufferSize)
        {
            Contract.Requires<ArgumentOutOfRangeException>(bufferCount >= 0);
            //Contract.Requires<ArgumentOutOfRangeException>(bufferSize > 0 && bufferSize < 64 * 1024);
            ThrowIfDisposed();

            if (IsInitialized)
            {
                throw new InvalidOperationException("The MidiBufferManager is already initialized.");
            }

            BufferCount = bufferCount;
            BufferSize = bufferSize;

            AllocateBuffers();
        }

        /// <summary>
        /// Retrieves a fresh (unused) buffer for the application to use.
        /// </summary>
        /// <returns>Returns null when no more buffers are unused.</returns>
        /// <remarks>This method is only called by the application logic for an <see cref="MidiOutPort"/>
        /// or a <see cref="MidiStreamOutPort"/>. The <see cref="MidiInPort"/> registers its own buffers.</remarks>
        public virtual MidiBufferStream Retrieve()
        {
            MidiBufferStream buffer = null;

            if (this.unusedBuffers.Count > 0)
            {
                lock (this.locker)
                {
                    if (this.unusedBuffers.Count > 0)
                    {
                        buffer = this.unusedBuffers.Dequeue();
                        this.usedBuffers.Add(buffer);
                    }
                }
            }

            return buffer;
        }

        /// <summary>
        /// Returns a buffer to the manager for reuse.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <exception cref="InvalidOpertationException">
        /// Thrown when the buffer does not belong to this manager or when the buffer is not ready to be returned.
        /// </exception>
        public virtual void Return(MidiBufferStream buffer)
        {
            if (!this.mapBuffers.ContainsKey(buffer.HeaderMemory))
            {
                throw new InvalidOperationException("Specified buffer is not owned by this MidiBufferManager.");
            }
            if (!this.usedBuffers.Contains(buffer))
            {
                throw new InvalidOperationException("Specified buffer was not in the used buffer list of this MidiBufferManager.");
            }

            if ((buffer.HeaderFlags & NativeMethods.MHDR_INQUEUE) > 0)
            {
                throw new InvalidOperationException(Properties.Resources.MidiBufferManager_BufferStillInQueue);
            }

            // could be an error
            //if ((buffer.HeaderFlags & NativeMethods.MHDR_DONE) == 0)
            //{
            //    throw new InvalidOperationException(Properties.Resources.MidiBufferManager_BufferNotDone);
            //}

            lock (this.locker)
            {
                this.usedBuffers.Remove(buffer);
                this.unusedBuffers.Enqueue(buffer);
            }
        }

        /// <summary>
        /// Called when the instance is disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.usedBuffers.Count > 0)
            {
                throw new InvalidOperationException("Cannot call Dispose when there are still buffers in use.");
            }

            try
            {
                FreeBuffers();

                if (disposing)
                {
                    this.unusedBuffers.Clear();
                    this.usedBuffers.Clear();
                    this.mapBuffers.Clear();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        /// Called when a buffer needs to be prepared for use.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        protected abstract void OnPrepareBuffer(MidiBufferStream buffer);

        /// <summary>
        /// Called when a buffer needs to be un-prepared after use.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        protected abstract void OnUnprepareBuffer(MidiBufferStream buffer);

        /// <summary>
        /// Finds a buffer instance based on the <see cref="MidiHeader"/> that is used by the <see cref="MidiPort"/> implementations.
        /// </summary>
        /// <param name="header">A reference to the midi header structure.</param>
        /// <returns>Returns null if the buffer was not found.</returns>
        internal MidiBufferStream FindBuffer(IntPtr headerMemory)
        {
            ThrowIfDisposed();

            return this.mapBuffers[headerMemory];
        }

        /// <summary>
        /// Allocates the unmanaged memory for the midi headers and buffers.
        /// </summary>
        private void AllocateBuffers()
        {
            memHeaders = MemoryUtil.Alloc(MemoryUtil.SizeOfMidiHeader * BufferCount);
            memBuffers = MemoryUtil.Alloc(BufferSize * BufferCount);
            GC.AddMemoryPressure((MemoryUtil.SizeOfMidiHeader + BufferSize) * BufferCount);

            IntPtr pHeader = IntPtr.Add(memHeaders, 0);
            IntPtr pBuffer = IntPtr.Add(memBuffers, 0);

            for (int i = 0; i < BufferCount; i++)
            {
                var buffer = new MidiBufferStream(pHeader, pBuffer, BufferSize, StreamAccess);
                this.unusedBuffers.Enqueue(buffer);
                this.mapBuffers.Add(pHeader, buffer);

                pHeader = IntPtr.Add(pHeader, MemoryUtil.SizeOfMidiHeader);
                pBuffer = IntPtr.Add(pBuffer, BufferSize);
            }
        }

        /// <summary>
        /// Frees the allocated memory.
        /// </summary>
        private void FreeBuffers()
        {
            if (this.memHeaders != IntPtr.Zero)
            {
                MemoryUtil.Free(this.memHeaders);
                this.memHeaders = IntPtr.Zero;
            }

            if (this.memBuffers != IntPtr.Zero)
            {
                MemoryUtil.Free(this.memBuffers);
                this.memBuffers = IntPtr.Zero;
            }

            var totalLength = (MemoryUtil.SizeOfMidiHeader + BufferSize) * BufferCount;

            if (totalLength > 0)
            {
                GC.RemoveMemoryPressure(totalLength);
            }
        }

        protected internal virtual void UnPrepareAllBuffers()
        {
            foreach (var buffer in mapBuffers.Values)
            {
                OnUnprepareBuffer(buffer);
            }
        }
    }
}