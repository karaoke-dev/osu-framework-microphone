﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using ManagedBass;
using osu.Framework.Input.Handlers.Microphone.PitchTracker;
using osu.Framework.Input.StateChanges;
using osu.Framework.Input.States;
using osu.Framework.Platform;
using System;
using System.Runtime.InteropServices;

namespace osu.Framework.Input.Handlers.Microphone
{
    internal class OsuTKMicrophoneHandler : InputHandler
    {
        public override bool IsActive => true;
        public override int Priority => 3;

        private readonly PitchTracker.PitchTracker pitchTracker = new PitchTracker.PitchTracker();
        private readonly int deviceIndex;
        private int stream;

        public OsuTKMicrophoneHandler(int device)
        {
            deviceIndex = device;
        }

        public override bool Initialize(GameHost host)
        {
            // Open microphone device if available
            Bass.RecordInit(deviceIndex);
            stream = Bass.RecordStart(44100, 2, BassFlags.RecordPause | BassFlags.Float, 60, Procedure);

            pitchTracker.PitchDetected += onPitchDetected;

            // Start channel
            Bass.ChannelPlay(stream);

            return true;
        }

        private float[] buffer;

        private bool Procedure(int Handle, IntPtr Buffer, int Length, IntPtr User)
        {
            // Read and save buffer
            if (buffer == null || buffer.Length < Length / 4)
                buffer = new float[Length / 4];

            Marshal.Copy(Buffer, buffer, 0, Length / 4);

            // Process buffer
            pitchTracker.ProcessBuffer(buffer);

            return true;
        }

        private MicrophoneState lastState = new MicrophoneState();

        void onPitchDetected(MicrophoneState state)
        {
            // Throw into pending input
            PendingInputs.Enqueue(new MicrophoneInput
            {
                State = lastState
            });

            lastState = state;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            // Pause channel
            Bass.ChannelPause(stream);

            // Close microphone
            Bass.RecordFree();
        }
    }
}
