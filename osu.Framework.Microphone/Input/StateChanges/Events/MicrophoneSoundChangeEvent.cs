// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;
using System;

namespace osu.Framework.Input.StateChanges.Events
{
    public class MicrophoneSoundChangeEvent : InputStateChangeEvent
    {
        public readonly MicrophoneState LastState;

        public MicrophoneSoundChangeEvent(InputState state, IInput input, MicrophoneState lastState)
            : base(state, input)
        {
            if (!(state is IMicrophoneInputState microphoneInputState))
                throw new ArgumentException($"{nameof(state)} should be the type of {nameof(MicrophoneInputState)}");

            LastState = lastState;
        }
    }
}
