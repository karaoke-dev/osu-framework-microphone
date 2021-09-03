// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Utils;

namespace osu.Framework.Input
{
    public readonly struct Voice : IEquatable<Voice>
    {
        /// <summary>
        /// Detected pitch
        /// </summary>
        public readonly float Pitch;

        /// <summary>
        /// Detected loudness
        /// </summary>
        public readonly float Loudness;

        /// <summary>
        /// Detected voice or not
        /// </summary>
        public bool HasVoice => Pitch != 0;

        public Voice(float pitch, float loudness)
        {
            Pitch = pitch;
            Loudness = loudness;
        }

        /// <summary>
        /// Indicates whether the <see cref="Pitch"/> of this voice is equal to <see cref="Pitch"/> of the other voice.
        /// Notice that we didn't care about loudness because it's almost not possible be same even with no noise.
        /// </summary>
        /// <param name="other">The other voice.</param>
        public bool Equals(Voice other) => Precision.AlmostEquals(Pitch, other.Pitch);

        public static bool operator ==(Voice left, Voice right) => left.Equals(right);
        public static bool operator !=(Voice left, Voice right) => !(left == right);

        public override bool Equals(object obj) => obj is Voice other && Equals(other);

        public override int GetHashCode() => Pitch.GetHashCode();
    }
}
