// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Input.States
{
    public class MicrophoneState : IEquatable<MicrophoneState>, ICloneable
    {
        private static readonly double inverse_log2 = 1.0 / Math.Log10(2.0);

        public const int MIN_MIDI_NOTE_A0 = 21,
                         MAX_MIDI_NOTE_C8 = 108;

        private double pitch;

        /// <summary>
        /// Detected pitch
        /// </summary>
        public double Pitch
        {
            get => pitch;
            set
            {
                if (pitch == value)
                    return;

                pitch = value;

                if (pitch < 20)
                {
                    Note = 0;
                }
                else
                {
                    var fNote = (float)(12 * Math.Log10(pitch / 55) * inverse_log2) + 33;
                    Note = fNote + 0.5f;
                }
            }
        }

        public float Note { get; private set; }

        /// <summary>
        /// Detected loudness
        /// </summary>
        public float Loudness { get; set; }

        /// <summary>
        /// Detected sound or not
        /// </summary>
        public bool HasSound => Pitch != 0;

        public MicrophoneState()
        {
        }

        public MicrophoneState(double pitch, float loudness)
        {
            Pitch = pitch;
            Loudness = loudness;
        }

        public bool Equals(MicrophoneState other)
        {
            return Pitch == other.Pitch && Loudness == other.Loudness;
        }

        public object Clone() => HasSound ? new MicrophoneState(Pitch, Loudness) : new MicrophoneState();
    }
}
