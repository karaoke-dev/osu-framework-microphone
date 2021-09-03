// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace osu.Framework.Tests.Visual.Input
{
    [TestFixture]
    public class TestSceneMicrophone : FrameworkTestScene
    {
        public TestSceneMicrophone()
        {
            Child = new MicrophoneInputManager
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new MicrophonePitchVisualization
                    {
                        X = 50
                    },
                    new MicrophoneLoudnessVisualization
                    {
                        X = 200,
                    }
                }
            };
        }

        private class MicrophonePitchVisualization : MicrophoneVisualization
        {
            protected override bool OnMicrophoneStartSinging(MicrophoneStartPitchingEvent e)
            {
                var pitch = e.CurrentState.Microphone.Pitch;
                BoxText.Text = "Pitch start : " + pitch;
                return base.OnMicrophoneStartSinging(e);
            }

            protected override bool OnMicrophoneEndSinging(MicrophoneEndPitchingEvent e)
            {
                var pitch = e.CurrentState.Microphone.Pitch;
                BoxText.Text = "Pitch end : " + pitch;
                return base.OnMicrophoneEndSinging(e);
            }

            protected override bool OnMicrophoneSinging(MicrophonePitchingEvent e)
            {
                var pitch = e.CurrentState.Microphone.Pitch;
                Y = -(pitch - 50);
                BoxText.Text = "Pitching : " + pitch;
                return base.OnMicrophoneSinging(e);
            }
        }

        private class MicrophoneLoudnessVisualization : MicrophoneVisualization
        {
            protected override bool OnMicrophoneStartSinging(MicrophoneStartPitchingEvent e)
            {
                var loudness = e.CurrentState.Microphone.Loudness;
                BoxText.Text = "Loudness start : " + loudness;
                return base.OnMicrophoneStartSinging(e);
            }

            protected override bool OnMicrophoneEndSinging(MicrophoneEndPitchingEvent e)
            {
                var loudness = e.CurrentState.Microphone.Loudness;
                BoxText.Text = "Loudness end : " + loudness;
                return base.OnMicrophoneEndSinging(e);
            }

            protected override bool OnMicrophoneSinging(MicrophonePitchingEvent e)
            {
                var loudness = e.CurrentState.Microphone.Loudness;
                Y = -(loudness - 50) * 5;
                BoxText.Text = "Loudness : " + loudness;
                return base.OnMicrophoneSinging(e);
            }
        }

        private class MicrophoneVisualization : CompositeDrawable
        {
            private readonly Box background;

            protected SpriteText BoxText { get; }

            protected MicrophoneVisualization()
            {
                Width = 100;
                Height = 100;
                Anchor = Anchor.CentreLeft;
                InternalChildren = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Blue
                    },
                    BoxText = new SpriteText
                    {
                        Text = "detecting"
                    }
                };
            }

            protected override bool Handle(UIEvent e) =>
                e switch
                {
                    MicrophoneStartPitchingEvent microphoneStartPitching => OnMicrophoneStartSinging(microphoneStartPitching),
                    MicrophoneEndPitchingEvent microphoneEndPitching => OnMicrophoneEndSinging(microphoneEndPitching),
                    MicrophonePitchingEvent microphonePitching => OnMicrophoneSinging(microphonePitching),
                    _ => base.Handle(e)
                };

            protected virtual bool OnMicrophoneStartSinging(MicrophoneStartPitchingEvent e)
            {
                background.Colour = Color4.Red;
                return false;
            }

            protected virtual bool OnMicrophoneEndSinging(MicrophoneEndPitchingEvent e)
            {
                background.Colour = Color4.Yellow;
                return false;
            }

            protected virtual bool OnMicrophoneSinging(MicrophonePitchingEvent e)
            {
                background.Colour = Color4.Blue;
                return false;
            }
        }
    }
}
