// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;

namespace osu.Framework.Input.Events
{
    public abstract class MicrophoneEvent : UIEvent
    {
        public new MicrophoneInputState CurrentState => (MicrophoneInputState)base.CurrentState;

        protected MicrophoneEvent(MicrophoneInputState state)
            : base(state)
        {
        }
    }
}
