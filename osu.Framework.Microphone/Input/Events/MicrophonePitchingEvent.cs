// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;

namespace osu.Framework.Input.Events
{
    public class MicrophonePitchingEvent : MicrophoneEvent
    {
        public MicrophonePitchingEvent(IMicrophoneInputState state)
            : base(state)
        {
        }
    }
}
