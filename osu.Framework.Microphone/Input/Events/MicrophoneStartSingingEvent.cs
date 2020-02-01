// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;

namespace osu.Framework.Input.Events
{
    public class MicrophoneStartSingingEvent : MicrophoneEvent
    {
        public MicrophoneStartSingingEvent(MicrophoneInputState state)
            : base(state)
        {
        }
    }
}
