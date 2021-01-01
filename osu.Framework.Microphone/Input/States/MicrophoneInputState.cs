// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Input.States
{
    public class MicrophoneInputState : InputState, IMicrophoneInputState
    {
        /// <summary>
        /// The microphone state.
        /// </summary>
        public MicrophoneState Microphone { get; }

        /// <summary>
        /// Creates a new <see cref="InputState"/> using the individual input states from another <see cref="InputState"/>.
        /// </summary>
        /// <param name="other">The <see cref="InputState"/> to take the individual input states from. Note that states are not cloned and will remain as references to the same objects.</param>
        public MicrophoneInputState(MicrophoneInputState other)
            : this(other.Microphone)
        {
        }

        /// <summary>
        /// Creates a new <see cref="InputState"/> using given individual input states.
        /// </summary>
        /// <param name="microphone">The microphone state.</param>
        public MicrophoneInputState(MicrophoneState microphone)
        {
            Microphone = microphone ?? new MicrophoneState();
        }
    }
}
