using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;

namespace CannedBytes.Midi
{
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

        public FileAccess StreamAccess { get; private set; }

        public int BufferSize { get; private set; }

        public int BufferCount { get; private set; }

        /// <summary>
        /// Gets the number of buffers currently in use.
        /// </summary>
        public int UsedBufferCount
        {
            get { ThrowIfDisposed(); return this.usedBuffers.Count; }
        }

        /// <summary>
        /// Gets the number of unused buffers.
        /// </summary>
        public int UnusedBufferCount
        {
            get { ThrowIfDisposed(); return this.unusedBuffers.Count; }
        }

        /// <summary>
        /// Indicates if the buffer manager has been initialized.
        /// </summary>
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

        public virtual void Initialize(int bufferCount, int bufferSize)
        {
            ThrowIfDisposed();
            Contract.Requires<ArgumentOutOfRangeException>(bufferCount >= 0);
            Contract.Requires<ArgumentOutOfRangeException>(bufferSize > 0 && bufferSize < 64 * 1024);

            if (IsInitialized)
            {
                throw new InvalidOperationException("The MidiBufferManager is already initialized.");
            }

            BufferCount = bufferCount;
            BufferSize = bufferSize;

            AllocateBuffers();
        }

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

        public virtual void Return(MidiBufferStream buffer)
        {
            if (!this.mapBuffers.ContainsKey(buffer.BufferMemory))
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
            if ((buffer.HeaderFlags & NativeMethods.MHDR_DONE) == 0)
            {
                throw new InvalidOperationException(Properties.Resources.MidiBufferManager_BufferNotDone);
            }

            lock (this.locker)
            {
                this.usedBuffers.Remove(buffer);
                this.unusedBuffers.Enqueue(buffer);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (this.usedBuffers.Count > 0)
            {
                throw new InvalidOperationException("Cannot call Dispose when there are still buffers in use.");
            }

            try
            {
                FreeBuffers();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected abstract void OnPrepareBuffer(MidiBufferStream buffer);

        protected abstract void OnUnprepareBuffer(MidiBufferStream buffer);

        internal MidiBufferStream FindBuffer(ref MidiHeader header)
        {
            ThrowIfDisposed();

            return this.mapBuffers[header.data];
        }

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
                this.mapBuffers.Add(pBuffer, buffer);

                OnPrepareBuffer(buffer);

                pHeader = IntPtr.Add(pHeader, MemoryUtil.SizeOfMidiHeader);
                pBuffer = IntPtr.Add(pBuffer, BufferSize);
            }
        }

        private void FreeBuffers()
        {
            for (int i = 0; i < this.unusedBuffers.Count; i++)
            {
                OnUnprepareBuffer(this.unusedBuffers.Dequeue());
            }

            MemoryUtil.Free(this.memHeaders);
            this.memHeaders = IntPtr.Zero;

            MemoryUtil.Free(this.memBuffers);
            this.memBuffers = IntPtr.Zero;

            GC.RemoveMemoryPressure((MemoryUtil.SizeOfMidiHeader + BufferSize) * BufferCount);
        }
    }
}