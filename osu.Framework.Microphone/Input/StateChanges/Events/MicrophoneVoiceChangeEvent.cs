// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;
using System;

namespace osu.Framework.Input.StateChanges.Events
{
    public class MicrophoneVoiceChangeEvent : InputStateChangeEvent
    {
        public readonly Voice LastVoice;

        public MicrophoneVoiceChangeEvent(InputState state, IInput input, Voice lastVoice)
            : base(state, input)
        {
            LastVoice = lastVoice;
        }
    }
}
