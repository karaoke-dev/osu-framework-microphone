// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;

namespace osu.Framework.Input.Events;

public class MicrophoneEndPitchingEvent : MicrophoneEvent
{
    public MicrophoneEndPitchingEvent(IMicrophoneInputState state)
        : base(state)
    {
    }
}
