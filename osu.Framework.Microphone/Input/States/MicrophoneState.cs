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
        public double Pitch { get; set; }

        /// <summary>
        /// Detected volumn
        /// </summary>
        public float Volumn { get; set; }

        /// <summary>
        /// Detected sound or not
        /// </summary>
        public bool HasSound => Pitch != 0;

        public MicrophoneState()
        { 
        
        }

        public MicrophoneState(double pitch)
        {
            Pitch = pitch;
        }

        public bool Equals(MicrophoneState other)
        {
            return Pitch == other.Pitch && Volumn == other.Volumn;
        }

        public object Clone()
        {
            if (HasSound)
                return new MicrophoneState(Pitch);

            return new MicrophoneState();
        }
    }
}
