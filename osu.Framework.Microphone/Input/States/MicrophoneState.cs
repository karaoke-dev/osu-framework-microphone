// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Input.States
{
    public class MicrophoneState : IEquatable<MicrophoneState>, ICloneable
    {
        /// <summary>
        /// Detected pitch
        /// </summary>
        public float Pitch { get; set; }

        /// <summary>
        /// Detected loudness
        /// </summary>
        public float Loudness { get; set; }

        /// <summary>
        /// Detected sound or not
        /// </summary>
        public bool HasSound => Pitch != 0;

        public bool Equals(MicrophoneState other)
        {
            return Pitch == other.Pitch && Loudness == other.Loudness;
        }

        public object Clone() => HasSound ? new MicrophoneState { Pitch = Pitch, Loudness = Loudness } : new MicrophoneState();

        public override string ToString()
        {
            return $@"Pitch: {Pitch}, Loudness: {Loudness}";
        }
    }
}
