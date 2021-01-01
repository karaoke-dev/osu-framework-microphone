// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.States;
using System;

namespace osu.Framework.Input.Events
{
    public abstract class MicrophoneEvent : UIEvent
    {
        public new IMicrophoneInputState CurrentState => (IMicrophoneInputState)base.CurrentState;

        protected MicrophoneEvent(IMicrophoneInputState state)
            : base(state as InputState)
        {
            if (state == null)
                throw new ArgumentException($"{nameof(state)} should be the type of {nameof(IMicrophoneInputState)}");
        }
    }
}
