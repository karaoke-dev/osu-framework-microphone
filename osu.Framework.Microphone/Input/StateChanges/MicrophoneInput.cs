// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.StateChanges.Events;
using osu.Framework.Input.States;
using System;

namespace osu.Framework.Input.StateChanges
{
    public class MicrophoneInput : IInput
    {
        public MicrophoneState State;

        public void Apply(InputState state, IInputStateChangeHandler handler)
        {
            if (!(state is IMicrophoneInputState microphoneInputState))
                throw new ArgumentException($"{nameof(state)} should be the type of {nameof(IMicrophoneInputState)}");

            // Become last state
            var lastState = microphoneInputState.Microphone.Clone() as MicrophoneState;

            // Update latest state into input state
            microphoneInputState.Microphone.Pitch = State.Pitch;
            microphoneInputState.Microphone.Volumn = State.Volumn;

            // Trigger change
            handler.HandleInputStateChange(new MicrophoneSoundChangeEvent(state, this, lastState));
        }
    }
}
