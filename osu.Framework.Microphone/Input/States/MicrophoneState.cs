// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Framework.Input.States;

public class MicrophoneState
{
    /// <summary>
    /// Detected pitch
    /// </summary>
    public Voice Voice { get; set; }

    public override string ToString() => $@"Pitch: {Voice.Pitch}, Decibel: {Voice.Decibel}";
}
