// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Input.Events;
using osu.Framework.Input.Handlers.Microphone;
using osu.Framework.Input.StateChanges.Events;
using osu.Framework.Input.States;
using System;

namespace osu.Framework.Input
{
    public class MicrophoneInputManager : CustomInputManager
    {
        protected override InputState CreateInitialState() => new MicrophoneInputState(new MicrophoneState());

        private readonly int deviceId;

        public MicrophoneInputManager(int device = -1)
        {
            deviceId = device;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // Use handler like iOS microphone handler if there's exist handler in dependencies.
            if (Host.Dependencies.Get(typeof(MicrophoneHandler)) is MicrophoneHandler handler)
                AddHandler(Activator.CreateInstance(handler.GetType()) as MicrophoneHandler);
            else
                AddHandler(new MicrophoneHandler(deviceId));
        }

        public override void HandleInputStateChange(InputStateChangeEvent inputStateChange)
        {
            switch (inputStateChange)
            {
                case MicrophoneVoiceChangeEvent microphoneVoiceChange:
                    HandleMicrophoneStateChange(microphoneVoiceChange);
                    break;
            }

            base.HandleInputStateChange(inputStateChange);
        }

        protected virtual void HandleMicrophoneStateChange(MicrophoneVoiceChangeEvent microphoneVoiceChange)
        {
            var inputState = microphoneVoiceChange.State as IMicrophoneInputState;
            var lastState = microphoneVoiceChange.LastState;
            var state = inputState.Microphone;

            if (!lastState.HasSound && state.HasSound)
                microphoneStartSinging(inputState);
            else if (lastState.HasSound && !state.HasSound)
                microphoneEndSinging(inputState);
            else
                microphoneSinging(inputState);
        }

        private bool microphoneStartSinging(IMicrophoneInputState state) => PropagateBlockableEvent(NonPositionalInputQueue, new MicrophoneStartPitchingEvent(state));

        private bool microphoneEndSinging(IMicrophoneInputState state) => PropagateBlockableEvent(NonPositionalInputQueue, new MicrophoneEndPitchingEvent(state));

        private bool microphoneSinging(IMicrophoneInputState state) => PropagateBlockableEvent(NonPositionalInputQueue, new MicrophonePitchingEvent(state));
    }
}
