// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using ManagedBass;
using NWaves.Features;
using NWaves.Signals;
using NWaves.Transforms;
using osu.Framework.Input.StateChanges;
using osu.Framework.Input.States;
using osu.Framework.Platform;
using System;
using System.Runtime.InteropServices;

namespace osu.Framework.Input.Handlers.Microphone
{
    public class MicrophoneHandler : InputHandler
    {
        public override bool IsActive => Bass.RecordingDeviceCount > 0;

        private readonly int deviceIndex;
        private int stream;

        public MicrophoneHandler(int device)
        {
            deviceIndex = device;
        }

        private RecordInfo recordInfo;

        public override bool Initialize(GameHost host)
        {
            Enabled.BindValueChanged(e =>
            {
                if (e.NewValue)
                {
                    // Open microphone device if available
                    Bass.RecordInit(deviceIndex);

                    recordInfo = Bass.RecordingInfo;
                    var frequency = recordInfo.Frequency;
                    var channel = recordInfo.Channels;
                    var period = 10;

                    stream = Bass.RecordStart(frequency, channel, BassFlags.RecordPause | BassFlags.Float, period, procedure);

                    // Start channel
                    Bass.ChannelPlay(stream);
                }
                else
                {
                    // Pause channel
                    Bass.ChannelPause(stream);

                    // Close microphone
                    Bass.RecordFree();
                }
            }, true);

            return true;
        }

        private float[] buffer;

        private bool procedure(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // Read and save buffer
            if (this.buffer == null || this.buffer.Length < length / 4)
                this.buffer = new float[length / 4];

            Marshal.Copy(buffer, this.buffer, 0, length / 4);

            // Process pitch
            var pitch = Pitch.FromYin(this.buffer, recordInfo.Frequency, low: 40, high: 1000);

            // Process loudness
            var spectrum = new Fft().PowerSpectrum(new DiscreteSignal(recordInfo.Frequency, this.buffer)).Samples;
            var loudness = Perceptual.Loudness(spectrum);

            // Send new event
            onPitchDetected(new MicrophoneState { Pitch = pitch, Loudness = loudness });
            return true;
        }

        private MicrophoneState lastState = new MicrophoneState();

        private void onPitchDetected(MicrophoneState state)
        {
            // do not continuous sending no sound event
            if (!state.HasSound && lastState.HasSound == state.HasSound)
                return;

            // Throw into pending input
            PendingInputs.Enqueue(new MicrophoneInput
            {
                State = state
            });

            lastState = state;
        }
    }
}
