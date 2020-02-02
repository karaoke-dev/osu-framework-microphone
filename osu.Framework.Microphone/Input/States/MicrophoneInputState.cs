// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Input.States
{
    public class MicrophoneInputState : InputState, IMicrophoneInputState
    {
        public MicrophoneState Microphone { get; }

        public MicrophoneInputState(MicrophoneState microphone)
        {
            Microphone = microphone;
        }
    }
}
