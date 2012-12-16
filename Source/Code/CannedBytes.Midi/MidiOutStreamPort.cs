namespace CannedBytes.Midi
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;

    /// <summary>
    /// The MidiOutPort class represents an interface to a physical (or virtual, depending on the driver)
    /// Midi Out Port.
    /// </summary>
    /// <remarks>
    /// Midi Ports are sometimes also called Midi Devices.
    /// </remarks>
    public class MidiOutStreamPort : MidiOutPortBase
    {
        /// <summary>
        /// Opens the Midi Out Port identified by the <paramref name="deviceId"/>.
        /// </summary>
        /// <param name="deviceId">An index into the available Midi Out Ports.</param>
        /// <remarks>Refer to <see cref="MidiOutPortCapsCollection"/>.</remarks>
        public override void Open(int deviceId)
        {
            ThrowIfDisposed();
            Check.IfArgumentOutOfRange(deviceId, 0, NativeMethods.midiOutGetNumDevs() - 1, "deviceId");

            Status = MidiPortStatus.Open | MidiPortStatus.Pending;

            MidiOutStreamSafeHandle streamHandle;

            uint portId = (uint)deviceId;
            int result = NativeMethods.midiStreamOpen(
                         out streamHandle,
                         ref portId,
                         1,
                         MidiProcRef,
                         ToIntPtr(),
                         NativeMethods.CALLBACK_FUNCTION);

            ThrowIfError(result);

            MidiSafeHandle = streamHandle;

            base.Open((int)portId);
        }

        /// <summary>
        /// Closes the Midi Out Stream Port.
        /// </summary>
        /// <remarks>
        /// When the Midi Port is in a <see cref="MidiPortStatus.Started"/> status <see cref="M:Stop"/>
        /// is called. If any buffers are still in use the <see cref="M:Reset"/> method is called to
        /// return all the buffers to the <see cref="P:BufferManager"/>. The method will block until all
        /// buffers are returned.
        /// </remarks>
        public override void Close()
        {
            ThrowIfDisposed();

            if (HasStatus(MidiPortStatus.Started))
            {
                this.Stop();
            }

            base.Close();
        }

        /// <summary>
        /// Resumes a paused midi stream.
        /// </summary>
        public void Restart()
        {
            if (!IsOpen)
            {
                throw new MidiInPortException(Properties.Resources.MidiOutPort_PortNotOpen);
            }

            int result = NativeMethods.midiStreamRestart(MidiSafeHandle);

            ThrowIfError(result);

            ModifyStatus(MidiPortStatus.Started, MidiPortStatus.Stopped | MidiPortStatus.Paused);
        }

        /// <summary>
        /// Pauses the midi stream.
        /// </summary>
        public void Pause()
        {
            if (!IsOpen)
            {
                throw new MidiInPortException(Properties.Resources.MidiOutPort_PortNotOpen);
            }

            int result = NativeMethods.midiStreamPause(MidiSafeHandle);

            ThrowIfError(result);

            ModifyStatus(MidiPortStatus.Paused, MidiPortStatus.Started | MidiPortStatus.Stopped);
        }

        /// <summary>
        /// Turns off playing notes and returns pending <see cref="MidiBufferStream"/>s to the <see cref="P:BufferManager"/>
        /// marked as done.
        /// </summary>
        public void Stop()
        {
            if (!IsOpen)
            {
                throw new MidiInPortException(Properties.Resources.MidiOutPort_PortNotOpen);
            }

            int result = NativeMethods.midiStreamStop(MidiSafeHandle);

            ThrowIfError(result);

            ModifyStatus(MidiPortStatus.Stopped, MidiPortStatus.Started | MidiPortStatus.Paused);
        }

        #region IMidiSender Members

        /// <summary>
        /// Outputs the <paramref name="buffer"/> to the port.
        /// </summary>
        /// <param name="buffer">The midi stream.</param>
        /// <remarks><see cref="Open"/> opens the port in paused mode. So <see cref="Restart"/>
        /// must be called before streams can be output with this method.
        /// Use the <see cref="T:MidiEventStreamWriter"/> on the <see cref="T:MidiBufferStream"/>
        /// to fill a midi stream.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public override void LongData(MidiBufferStream buffer)
        {
            Check.IfArgumentNull(buffer, "buffer");

            ////if ((buffer.HeaderFlags & NativeMethods.MHDR_PREPARED) == 0)
            ////{
            ////    throw new InvalidOperationException("LongData cannot be called with a MidiBufferStream that has not been prepared.");
            ////}

            buffer.BytesRecorded = buffer.Position;

            int result = NativeMethods.midiStreamOut(
                         MidiSafeHandle,
                         buffer.ToIntPtr(),
                         (uint)MemoryUtil.SizeOfMidiHeader);

            ThrowIfError(result);
        }

        #endregion IMidiSender Members

        /// <inheritdocs/>
        protected override bool OnMessage(int msg, IntPtr parameter1, IntPtr parameter2)
        {
            switch ((uint)msg)
            {
                case NativeMethods.MOM_POSITIONCB:
                    var buffer = this.BufferManager.FindBuffer(parameter1);

                    if (buffer != null && NextCallback != null)
                    {
                        NextCallback.LongData(buffer, MidiDataCallbackType.Notification);
                    }
                    return true;
            }
            return base.OnMessage(msg, parameter1, parameter2);
        }

        /// <summary>
        /// Backing field for the <see cref="BufferManager"/> property.
        /// </summary>
        private MidiOutStreamBufferManager bufferManager = null;

        /// <summary>
        /// Gets the buffer manager for the Midi Stream Out Port.
        /// </summary>
        public override MidiOutBufferManager BufferManager
        {
            get
            {
                if (this.bufferManager == null)
                {
                    this.bufferManager = new MidiOutStreamBufferManager(this);
                }

                return this.bufferManager;
            }
        }

        /// <inheritdocs/>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            try
            {
                if (!IsDisposed)
                {
                    if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
                    {
                        if (this.bufferManager != null)
                        {
                            this.bufferManager.Dispose();
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposeKind);
            }
        }

        /// <summary>
        /// Retrieves the current time position for playback.
        /// </summary>
        /// <param name="formatType">One of the <see cref="TimeFormatTypes"/> enumerated values, except Smpte.</param>
        /// <returns>Returns the current time in the requested time <paramref name="formatType"/>.
        /// When the value is negative, the requested time <paramref name="formatType"/> was not supported
        /// and the return value specifies the proposed time format type (as a negative value).</returns>
        public long GetTime(TimeFormatTypes formatType)
        {
            ThrowIfDisposed();
            if (formatType == TimeFormatTypes.Smpte)
            {
                throw new InvalidOperationException(
                    Properties.Resources.MidiStreamOutPort_InvalidTimeFormatType);
            }

            MmTime time = new MmTime();
            time.Type = (uint)formatType;

            this.GetTime(ref time);

            if (time.Type == (uint)formatType)
            {
                // for all types (except Smpte) same field is used.
                return time.Ticks;
            }
            else
            {
                // return the supported time format as error
                return -time.Type;
            }
        }

        /// <summary>
        /// Returns the current stream position in a Smpte format.
        /// </summary>
        /// <returns>Returns the smpte time.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Smpte", Justification = "Known abbreviation.")]
        public SmpteTime SmpteTime
        {
            get
            {
                ThrowIfDisposed();

                MmTime time = new MmTime();
                time.Type = (uint)TimeFormatTypes.Smpte;

                this.GetTime(ref time);

                return new SmpteTime(
                    time.SmpteHour, time.SmpteMin, time.SmpteSec, time.SmpteFrame, time.SmpteFps);
            }
        }

        /// <summary>
        /// Returns the Time division for this stream, in the format specified
        /// in the Standard MIDI Files 1.0 specification.
        /// </summary>
        public int TimeDivision
        {
            get { return (int)this.GetProperty(NativeMethods.MIDIPROP_TIMEDIV); }
            set { this.SetProperty(NativeMethods.MIDIPROP_TIMEDIV, (uint)value); }
        }

        /// <summary>
        /// Returns the Tempo of the stream, in microseconds per quarter note.
        /// </summary>
        /// <remarks>The tempo is honored only if the time division for the stream
        /// is specified in quarter note format.</remarks>
        public long Tempo
        {
            get { return (long)this.GetProperty(NativeMethods.MIDIPROP_TEMPO); }
            set { this.SetProperty(NativeMethods.MIDIPROP_TEMPO, (uint)value); }
        }

        /// <summary>
        /// Gets the current time position from the port.
        /// </summary>
        /// <param name="time">A reference to the time structure that receives the value.</param>
        private void GetTime(ref MmTime time)
        {
            int result = NativeMethods.midiStreamPosition(
                         MidiSafeHandle,
                         ref time,
                         (uint)MemoryUtil.SizeOfMmTime);

            ThrowIfError(result);
        }

        /// <summary>
        /// Returns a property from the port.
        /// </summary>
        /// <param name="flags">Indication of what property to read.</param>
        /// <returns>Returns the property value read.</returns>
        private uint GetProperty(uint flags)
        {
            MidiOutStreamPortProperty prop = new MidiOutStreamPortProperty(0);

            int result = NativeMethods.midiStreamProperty(
                         MidiSafeHandle,
                         ref prop,
                         flags | NativeMethods.MIDIPROP_GET);

            ThrowIfError(result);

            return prop.PropertyValue;
        }

        /// <summary>
        /// Sets a property to the port.
        /// </summary>
        /// <param name="flags">Indication of what property to write.</param>
        /// <param name="value">The value of the property.</param>
        private void SetProperty(uint flags, uint value)
        {
            MidiOutStreamPortProperty prop = new MidiOutStreamPortProperty(value);

            int result = NativeMethods.midiStreamProperty(
                         MidiSafeHandle,
                         ref prop,
                         flags | NativeMethods.MIDIPROP_SET);

            ThrowIfError(result);
        }
    }
}