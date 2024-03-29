﻿using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using System;
using System.Collections;
using System.IO;

namespace Traffix.Providers.PcapFile
{
    public class SharpPcapReader : ICaptureFileReader
    {
        ICaptureDevice _device;
        int _frameNumber;
        long _frameOffset;
        RawCapture _current;
        ReadingState _state;

        public SharpPcapReader(string captureFile)
        {
            // Get an offline device
            _device = new CaptureFileReaderDevice(captureFile);
            // Open the device
            _device.Open();
            _state = ReadingState.NotStarted;
        }

        /// <inheritdoc/>
        public LinkLayers LinkLayer => _device.LinkType;


        /// <inheritdoc/>
        public RawCapture Current
        {
            get
            {
                switch(_state)
                {
                    case ReadingState.NotStarted: throw new InvalidOperationException("Call MoveNext first.");
                    case ReadingState.Finished: throw new InvalidOperationException("Already finished.");
                    case ReadingState.Success: return _current;
                    default: throw new InvalidOperationException("Reader is not open."); 
                }
            }
        }

        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public ReadingState State => _state;

        public Stream Stream => throw new NotSupportedException();

        public bool CanAccessStream => false;

        /// <inheritdoc/>
        public void Close()
        {
            _device.Close();
            _state = ReadingState.Closed;
        }

        /// <inheritdoc/>
        public bool GetNextFrame(out RawCapture frame, bool readData = true)
        {
            var ok = GetNextFrameInternal();
            frame = _current;
            return ok;
        }
        private bool GetNextFrameInternal()
        {
            if (_state == ReadingState.Closed) throw new InvalidOperationException("Reader is not open.");
            if (_state == ReadingState.Finished) return false;
            _current = _device.GetNextPacket();
           
            if (_current != null)
            {
                _state = ReadingState.Success;
                return true;
            }
            else
            {
                _current = default;
                _state = ReadingState.Finished;
                return false;
            }
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _device.Close();
                    _device = null;
                }
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

        internal static long GetTicksFromPosixTimeval(PosixTimeval timeval)
        {
            return timeval.Date.Ticks;
        }

        public bool MoveNext()
        {
            return GetNextFrameInternal();
        }

        public void Reset()
        {
            _device.Close();
            _device.Open();
            _state = ReadingState.NotStarted;
        }

        public static IObservable<RawCapture> CreateObservable(string captureFile)
        {
            var reader = new SharpPcapReader(captureFile);
            return reader.CreateObservable();
        }
    }
}
