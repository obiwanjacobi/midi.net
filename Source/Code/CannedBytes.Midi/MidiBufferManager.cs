namespace CannedBytes.Midi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    /// <summary>
    /// The MidiBufferManager base class implements unmanaged memory management
    /// for <see cref="MidiBufferStream"/>s that are used by the <see cref="MidiPort"/>s
    /// for sending and receiving sysex message or sending streams of midi events.
    /// </summary>
    public abstract class MidiBufferManager : UnmanagedDisposableBase
    {
        /// <summary>Unmanaged pointer to the header.</summary>
        private IntPtr _memHeaders = IntPtr.Zero;

        /// <summary>Unmanaged pointer to the buffer.</summary>
        private IntPtr _memBuffers = IntPtr.Zero;

        /// <summary>Locking object.</summary>
        private readonly object _locker = new object();

        /// <summary>List of used buffers.</summary>
        private readonly List<MidiBufferStream> _usedBuffers = new List<MidiBufferStream>();

        /// <summary>A list of used buffers.</summary>
        private readonly Queue<MidiBufferStream> _unusedBuffers = new Queue<MidiBufferStream>();

        /// <summary>A map of all buffers.</summary>
        private readonly Dictionary<IntPtr, MidiBufferStream> _mapBuffers = new Dictionary<IntPtr, MidiBufferStream>();

        /// <summary>An threading event to signal all buffers were returned.</summary>
        private readonly AutoResetEvent _buffersReturnedEvent = new AutoResetEvent(false);

        /// <summary>
        /// For derived classes only.
        /// </summary>
        /// <param name="port">A reference to the midi port this buffer manager serves.</param>
        /// <param name="access">The type of access the stream provides to the underlying buffer.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="port"/> is null.</exception>
        protected MidiBufferManager(MidiPort port, FileAccess access)
        {
            Check.IfArgumentNull(port, nameof(port));

            MidiPort = port;
            StreamAccess = access;
        }

        /// <summary>
        /// Backing field for the <see cref="MidiPort"/> property.
        /// </summary>
        private MidiPort _midiPort;

        /// <summary>
        /// Gets the MidiPort this buffer manager serves.
        /// </summary>
        protected MidiPort MidiPort
        {
            get
            {
                return _midiPort;
            }

            set
            {
                Check.IfArgumentNull(value, "MidiPort");
                _midiPort = value;
            }
        }

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
            get { return _usedBuffers.Count; }
        }

        /// <summary>
        /// Gets the number of buffers that are currently not in use by the <see cref="MidiPort"/>.
        /// </summary>
        public int UnusedBufferCount
        {
            get { return _unusedBuffers.Count; }
        }

        /// <summary>
        /// Indicates if the buffer manager has been initialized.
        /// </summary>
        /// <remarks>Call the <see cref="M:Initialze"/> method to initialize this instance.</remarks>
        public bool IsInitialized
        {
            get { return (_memHeaders != IntPtr.Zero) || (_memBuffers != IntPtr.Zero); }
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

            return _buffersReturnedEvent.WaitOne(millisecondTimeout, false);
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
            Check.IfArgumentOutOfRange(bufferCount, 0, int.MaxValue, nameof(bufferCount));
            Check.IfArgumentOutOfRange(bufferSize, 0, 64 * 1024, nameof(bufferSize));
            ThrowIfDisposed();
            if (IsInitialized)
            {
                throw new InvalidOperationException("The Midi Buffer Manager is already initialized.");
            }

            BufferCount = bufferCount;
            BufferSize = bufferSize;

            if (BufferCount > 0)
            {
                AllocateBuffers();
            }
        }

        /// <summary>
        /// Retrieves a fresh (unused) buffer for the application to use.
        /// </summary>
        /// <returns>Returns null when no more buffers are unused.</returns>
        /// <remarks>This method is only called by the application logic for an <see cref="MidiOutPort"/>
        /// or a <see cref="MidiOutStreamPort"/>. The <see cref="MidiInPort"/> registers its own buffers.</remarks>
        public virtual MidiBufferStream RetrieveBuffer()
        {
            MidiBufferStream buffer = null;

            if (_unusedBuffers.Count > 0)
            {
                lock (_locker)
                {
                    if (_unusedBuffers.Count > 0)
                    {
                        buffer = _unusedBuffers.Dequeue();
                        _usedBuffers.Add(buffer);
                    }
                }
            }

            return buffer;
        }

        /// <summary>
        /// Returns a buffer to the manager for reuse.
        /// </summary>
        /// <param name="buffer">Must not be null.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the buffer does not belong to this manager or when the buffer is not ready to be returned.
        /// </exception>
        public virtual void ReturnBuffer(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, nameof(buffer));
            ThrowIfDisposed();
            if (!_mapBuffers.ContainsKey(buffer.HeaderMemory))
            {
                throw new InvalidOperationException("Specified buffer is not owned by this instance.");
            }

            if (!_usedBuffers.Contains(buffer))
            {
                throw new InvalidOperationException("Specified buffer was not in the used buffer list of this instance.");
            }

            if ((buffer.HeaderFlags & NativeMethods.MHDR_INQUEUE) > 0)
            {
                throw new InvalidOperationException(Properties.Resources.MidiBufferManager_BufferStillInQueue);
            }

            //// could be an error
            ////if ((buffer.HeaderFlags & NativeMethods.MHDR_DONE) == 0)
            ////{
            ////    throw new InvalidOperationException(Properties.Resources.MidiBufferManager_BufferNotDone);
            ////}

            lock (_locker)
            {
                _usedBuffers.Remove(buffer);
                _unusedBuffers.Enqueue(buffer);
            }

            buffer.Position = 0;
        }

        /// <summary>
        /// Called when the instance is disposed.
        /// </summary>
        /// <param name="disposeKind">The type of resources to dispose.</param>
        /// <exception cref="InvalidOperationException">Thrown when not all buffers have been
        /// returned to the buffer manager.</exception>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            // I know you're not supposed to throw exceptions in Dispose.
            // But the alternative is yanking the unmanaged memory from under the buffers
            // that are still being used. That would certainly crash even the most robust
            // applications. So view this as an early warning system - as a developer head's up.
            if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources &&
                _usedBuffers.Count > 0)
            {
                throw new InvalidOperationException("Cannot call Dispose when there are still buffers in use.");
            }

            FreeBuffers();

            if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
            {
                _unusedBuffers.Clear();
                _usedBuffers.Clear();
                _mapBuffers.Clear();

                _buffersReturnedEvent.Close();
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
        /// <param name="headerMemory">A pointer to the midi header structure.</param>
        /// <returns>Returns null if the buffer was not found.</returns>
        internal MidiBufferStream FindBuffer(IntPtr headerMemory)
        {
            ThrowIfDisposed();

            return _mapBuffers[headerMemory];
        }

        /// <summary>
        /// Allocates the unmanaged memory for the midi headers and buffers.
        /// </summary>
        private void AllocateBuffers()
        {
            if (BufferSize > 0 && BufferCount > 0)
            {
                _memHeaders = MemoryUtil.Alloc(MemoryUtil.SizeOfMidiHeader * BufferCount);
                _memBuffers = MemoryUtil.Alloc(BufferSize * BufferCount);
                GC.AddMemoryPressure((MemoryUtil.SizeOfMidiHeader + BufferSize) * BufferCount);

                IntPtr headerMem = IntPtr.Add(_memHeaders, 0);
                IntPtr bufferMem = IntPtr.Add(_memBuffers, 0);

                for (int i = 0; i < BufferCount; i++)
                {
                    var buffer = new MidiBufferStream(headerMem, bufferMem, BufferSize, StreamAccess);

                    try
                    {
                        _unusedBuffers.Enqueue(buffer);
                        _mapBuffers.Add(headerMem, buffer);

                        headerMem = IntPtr.Add(headerMem, MemoryUtil.SizeOfMidiHeader);
                        bufferMem = IntPtr.Add(bufferMem, BufferSize);
                    }
                    catch
                    {
                        buffer.Dispose();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Frees the allocated memory.
        /// </summary>
        private void FreeBuffers()
        {
            if (_memHeaders != IntPtr.Zero)
            {
                MemoryUtil.Free(_memHeaders);
                _memHeaders = IntPtr.Zero;
            }

            if (_memBuffers != IntPtr.Zero)
            {
                MemoryUtil.Free(_memBuffers);
                _memBuffers = IntPtr.Zero;
            }

            var totalLength = (MemoryUtil.SizeOfMidiHeader + BufferSize) * BufferCount;

            if (totalLength > 0)
            {
                GC.RemoveMemoryPressure(totalLength);
            }
        }

        /// <summary>
        /// Loops through all the buffers and calls the <see cref="M:OnPrepareBuffer"/> for each one.
        /// </summary>
        protected internal virtual void PrepareAllBuffers()
        {
            ThrowIfDisposed();

            foreach (var buffer in _mapBuffers.Values)
            {
                if (buffer != null)
                {
                    OnPrepareBuffer(buffer);
                }
            }
        }

        /// <summary>
        /// Loops through all the buffers and calls the <see cref="M:OnUnprepareBuffer"/> for each one.
        /// </summary>
        protected internal virtual void UnprepareAllBuffers()
        {
            ThrowIfDisposed();

            foreach (var buffer in _mapBuffers.Values)
            {
                if (buffer != null)
                {
                    OnUnprepareBuffer(buffer);
                }
            }
        }
    }
}