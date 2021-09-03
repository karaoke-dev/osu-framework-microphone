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
using System.Linq;
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

                    if(!isCurrentDeviceValid())
                        return;

                    recordInfo = Bass.RecordingInfo;
                    var frequency = recordInfo.Frequency;
                    var channel = recordInfo.Channels;
                    var period = 10 * channel;

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

            static bool isCurrentDeviceValid()
            {
                var index = Bass.CurrentRecordingDevice;
                var device = index == Bass.DefaultDevice ? default : Bass.RecordGetDeviceInfo(index);

                return device.IsEnabled && device.IsInitialized;
            }
        }

        private float[] unprocessedBuffer = Array.Empty<float>();

        private Voice lastVoice;

        private bool procedure(int handle, IntPtr buffer, int length, IntPtr user)
        {
            // Read and save buffer
            var size = length / 4;
            var localBuffer = new float[size];

            Marshal.Copy(buffer, localBuffer, 0, size);

            unprocessedBuffer = unprocessedBuffer.Concat(localBuffer).ToArray();

            // note : will cause error if buffer is less than 48000 / 40 * 2 = 2400
            // so not need to process buffer if less then 2400
            if (unprocessedBuffer.Length < 2400)
                return true;

            // Process pitch
            var pitch = Pitch.FromYin(unprocessedBuffer, recordInfo.Frequency, low: 40, high: 1000);

            // Process loudness
            var spectrum = new Fft().PowerSpectrum(new DiscreteSignal(recordInfo.Frequency, unprocessedBuffer)).Samples;
            var loudness = Perceptual.Loudness(spectrum);

            // Send new event
            var voice = new Voice(pitch, loudness);
            if (voice != lastVoice)
            {
                dispatchEvent(voice);
                lastVoice = voice;
            }

            // clear the array.
            unprocessedBuffer =  Array.Empty<float>();

            return true;
        }

        private void dispatchEvent(Voice voice)
        {
            PendingInputs.Enqueue(new MicrophoneInput
            {
                Voice = voice
            });
        }
    }
}
