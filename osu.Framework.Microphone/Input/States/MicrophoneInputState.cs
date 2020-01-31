// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Input.States
{
    public class MicrophoneInputState : InputState
    {
        public readonly MicrophoneState Microphone;

        public MicrophoneInputState(MicrophoneState microphone)
        {
            Microphone = microphone;
        }
    }
}
