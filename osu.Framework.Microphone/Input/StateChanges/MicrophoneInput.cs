// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.StateChanges.Events;
using osu.Framework.Input.States;

namespace osu.Framework.Input.StateChanges
{
    public class MicrophoneInput : IInput
    {
        public Voice Voice;

        public void Apply(InputState state, IInputStateChangeHandler handler)
        {
            if (!(state is IMicrophoneInputState microphoneInputState))
                throw new NotMicrophoneInputStateException();

            var microphone = microphoneInputState.Microphone;
            if(microphone.Voice == Voice)
                return;

            var lastVoice = microphone.Voice;
            microphone.Voice = Voice;
            handler.HandleInputStateChange(new MicrophoneVoiceChangeEvent(state, this, lastVoice));
        }
    }
}
