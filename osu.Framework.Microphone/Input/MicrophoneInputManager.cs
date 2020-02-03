// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.Events;
using osu.Framework.Input.Handlers.Microphone;
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

        public float DetectLevelThreshold
        {
            get => handler.DetectLevelThreshold;
            set => handler.DetectLevelThreshold = value;
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
            var inputState = microphoneSoundChange.State as IMicrophoneInputState;
            var lastState = microphoneSoundChange.LastState;
            var state = inputState.Microphone;

            if (!lastState.HasSound && state.HasSound)
                microphoneStartSinging(inputState);
            else if(lastState.HasSound && !state.HasSound)
                microphoneEndSinging(inputState);
            else
                microphoneSinging(inputState);
        }

        private bool microphoneStartSinging(IMicrophoneInputState state) => PropagateBlockableEvent(NonPositionalInputQueue, new MicrophoneStartPitchingEvent(state));

        private bool microphoneEndSinging(IMicrophoneInputState state) => PropagateBlockableEvent(NonPositionalInputQueue, new MicrophoneEndPitchingEvent(state));

        private bool microphoneSinging(IMicrophoneInputState state) => PropagateBlockableEvent(NonPositionalInputQueue, new MicrophonePitchingEvent(state));
    }
}
