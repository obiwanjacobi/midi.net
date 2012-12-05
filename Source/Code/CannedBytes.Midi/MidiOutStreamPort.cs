using System;

namespace CannedBytes.Midi
{
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
        /// Opens the Midi Out Port identified by the <paramref name="portId"/>.
        /// </summary>
        /// <param name="portId">An index into the available Midi Out Ports.</param>
        /// <remarks>Refer to <see cref="MidiOutPortCapsCollection"/>.</remarks>
        public override void Open(int portId)
        {
            MidiStreamOutSafeHandle streamHandle;

            uint deviceId = (uint)portId;
            int result = NativeMethods.midiStreamOpen(out streamHandle, ref deviceId, 1,
                _midiProc, ToIntPtr(), NativeMethods.CALLBACK_FUNCTION);

            ThrowIfError(result);

            MidiSafeHandle = streamHandle;

            base.Open((int)deviceId);
        }

        /// <summary>
        /// Resumes a paused midi stream.
        /// </summary>
        public void Restart()
        {
            int result = NativeMethods.midiStreamRestart(MidiSafeHandle);

            ThrowIfError(result);

            ModifyStatus(MidiPortStatus.Started, MidiPortStatus.Stopped | MidiPortStatus.Paused);
        }

        /// <summary>
        /// Pauses the midi stream.
        /// </summary>
        public void Pause()
        {
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
        /// Use the <see cref="MidiEventStreamWriter"/> on the <see cref="MidiBufferStream.MidiStream"/>
        /// to create a midi stream.</remarks>
        public override void LongData(MidiBufferStream buffer)
        {
            //Throw.IfArgumentNull(buffer, "buffer");

            if ((buffer.HeaderFlags & NativeMethods.MHDR_PREPARED) == 0)
            {
                throw new InvalidOperationException("LongData cannot be called with a MidiBufferStream that has not been prepared.");
                //MidiBufferManager.Prepare(buffer);
            }

            int result = NativeMethods.midiStreamOut(MidiSafeHandle,
                buffer.ToIntPtr(), (uint)MemoryUtil.SizeOfMidiHeader);

            ThrowIfError(result);
        }

        #endregion IMidiSender Members

        /// <summary>
        /// Gets the buffer manager for the Midi Stream Out Port.
        /// </summary>
        public override MidiOutBufferManager MidiBufferManager
        {
            get
            {
                if (base.MidiBufferManager == null)
                {
                    base.MidiBufferManager = new MidiOutStreamBufferManager(this);
                }

                return base.MidiBufferManager;
            }
        }

        /// <summary>
        /// Retrieves the current time position for playback.
        /// </summary>
        /// <param name="formatType">One of the <see cref="TimeFormatType"/> enumerated values, except Smpte.</param>
        /// <returns>Returns the current time in the requested time <paramref name="formatType"/>.
        /// When the value is negative, the requested time <paramref name="formatType"/> was not supported
        /// and the return value specifies the proposed time format type (as a negative value).</returns>
        public long GetTime(TimeFormatType formatType)
        {
            #region Method Checks

            ThrowIfDisposed();
            if (formatType == TimeFormatType.Smpte)
            {
                throw new InvalidOperationException(
                    Properties.Resources.MidiStreamOutPort_InvalidTimeFormatType);
            }

            #endregion Method Checks

            MmTime time = new MmTime();
            time.wType = (uint)formatType;

            GetTime(ref time);

            if (time.wType == (uint)formatType)
            {
                // for all types (except Smpte) same field is used.
                return time.ticks;
            }
            else
            {
                // return the supported time format as error
                return -time.wType;
            }
        }

        /// <summary>
        /// Returns the current stream position in a Smpte format.
        /// </summary>
        /// <returns>Returns the smpte time.</returns>
        public SmpteTime GetSmpteTime()
        {
            ThrowIfDisposed();

            MmTime time = new MmTime();
            time.wType = (uint)TimeFormatType.Smpte;

            GetTime(ref time);

            return new SmpteTime(
                time.smpteHour, time.smpteMin, time.smpteSec, time.smpteFrame, time.smpteFps);
        }

        /// <summary>
        /// Returns the Time division for this stream, in the format specified
        /// in the Standard MIDI Files 1.0 specification.
        /// </summary>
        public int TimeDivision
        {
            get { return (int)GetProperty(NativeMethods.MIDIPROP_TIMEDIV); }
            set { SetProperty(NativeMethods.MIDIPROP_TIMEDIV, (uint)value); }
        }

        /// <summary>
        /// Returns the Tempo of the stream, in microseconds per quarter note.
        /// </summary>
        /// <remarks>The tempo is honored only if the time division for the stream
        /// is specified in quarter note format.</remarks>
        public long Tempo
        {
            get { return (long)GetProperty(NativeMethods.MIDIPROP_TEMPO); }
            set { SetProperty(NativeMethods.MIDIPROP_TEMPO, (uint)value); }
        }

        private void GetTime(ref MmTime time)
        {
            int result = NativeMethods.midiStreamPosition(MidiSafeHandle,
                ref time, (uint)MemoryUtil.SizeOfMmTime);

            ThrowIfError(result);
        }

        private uint GetProperty(uint flags)
        {
            MidiOutStreamPortProperty prop = new MidiOutStreamPortProperty(0);

            int result = NativeMethods.midiStreamProperty(MidiSafeHandle,
                ref prop, flags | NativeMethods.MIDIPROP_GET);

            ThrowIfError(result);

            return prop.propertyValue;
        }

        private void SetProperty(uint flags, uint value)
        {
            MidiOutStreamPortProperty prop = new MidiOutStreamPortProperty(value);

            int result = NativeMethods.midiStreamProperty(MidiSafeHandle,
                ref prop, flags | NativeMethods.MIDIPROP_SET);

            ThrowIfError(result);
        }
    }
}