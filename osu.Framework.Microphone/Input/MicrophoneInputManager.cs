// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.Events;
using osu.Framework.Input.Handlers.Microphone;
using osu.Framework.Input.StateChanges;
using osu.Framework.Input.StateChanges.Events;
using osu.Framework.Input.States;

namespace osu.Framework.Input
{
    public class MicrophoneInputManager : CustomInputManager
    {
        private readonly OsuTKMicrophoneHandler handler;

        protected override InputState CreateInitialState() => new MicrophoneInputState(new MicrophoneState());

        public MicrophoneInputManager(int device = -1)
        {
            AddHandler(handler = new OsuTKMicrophoneHandler(device));
        }

        public override void HandleInputStateChange(InputStateChangeEvent inputStateChange)
        {
            switch (inputStateChange)
            {
                case MicrophoneSoundChangeEvent microphoneSoundChange:
                    HandleMicrophoneStateChange(microphoneSoundChange);
                    break;
            }

            base.HandleInputStateChange(inputStateChange);
        }

        protected virtual void HandleMicrophoneStateChange(MicrophoneSoundChangeEvent microphoneSoundChange)
        {
            // TODO : call microphoneStartSinging, microphoneEndSinging, microphoneSinging at right time
            microphoneSinging(new MicrophoneInputState(microphoneSoundChange.LastState));
        }

        private bool microphoneStartSinging(MicrophoneInputState state) => PropagateBlockableEvent(PositionalInputQueue, new MicrophoneStartSingingEvent(state));

        private bool microphoneEndSinging(MicrophoneInputState state) => PropagateBlockableEvent(PositionalInputQueue, new MicrophoneEndSingingEvent(state));

        private bool microphoneSinging(MicrophoneInputState state) => PropagateBlockableEvent(NonPositionalInputQueue, new MicrophoneSingingEvent(state));
    }
}
