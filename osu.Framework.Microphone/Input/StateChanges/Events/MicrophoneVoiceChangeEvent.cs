// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;
using System;

namespace osu.Framework.Input.StateChanges.Events
{
    public class MicrophoneVoiceChangeEvent : InputStateChangeEvent
    {
        public readonly MicrophoneState LastState;

        public MicrophoneVoiceChangeEvent(InputState state, IInput input, MicrophoneState lastState)
            : base(state, input)
        {
            if (!(state is IMicrophoneInputState))
                throw new NotMicrophoneInputStateException();

            LastState = lastState;
        }
    }
}
