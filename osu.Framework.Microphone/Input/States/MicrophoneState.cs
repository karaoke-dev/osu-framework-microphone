// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Framework.Input.States
{
    public class MicrophoneState : IEquatable<MicrophoneState>
    {
        public float Sacle { get; set; }

        public float Volumn { get; set; }

        public bool Equals(MicrophoneState other)
        {
            return Sacle == other.Sacle && Volumn == other.Volumn;
        }
    }
}
