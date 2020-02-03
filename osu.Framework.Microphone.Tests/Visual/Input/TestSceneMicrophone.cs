// Copyright (c) andy840119 <andy840119@gmail.com>. Licensed under the GPL Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK.Graphics;

namespace osu.Framework.Tests.Visual.Input
{
    public class TestSceneMicrophone : FrameworkTestScene
    {
        public TestSceneMicrophone()
        {
            Child = new MicrophoneInputManager()
            {
                RelativeSizeAxes = Axes.Both,
                Children = new []
                { 
                    new MicrophoneVisualization
                    {
                        Anchor = Anchor.Centre,
                        Width = 100,
                        Height = 100
                    }
                }
            };
        }

        public class MicrophoneVisualization : CompositeDrawable
        {
            private readonly Box background;
            private readonly SpriteText pitchText;

            public MicrophoneVisualization()
            {
                InternalChildren = new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Blue
                    },
                    pitchText = new SpriteText
                    {
                        Text = "detecting"
                    }
                };
            }

            protected override bool Handle(UIEvent e)
            {
                switch (e)
                {
                    case MicrophoneStartPitchingEvent microphoneStartPitching:
                        return OnMicrophoneStartSinging(microphoneStartPitching);

                    case MicrophoneEndPitchingEvent microphoneEndPitching:
                        return OnMicrophoneEndSinging(microphoneEndPitching);

                    case MicrophonePitchingEvent microphonePitching:
                        return OnMicrophoneSinging(microphonePitching);

                    default:
                        return base.Handle(e);
                }
            }

            protected virtual bool OnMicrophoneStartSinging(MicrophoneStartPitchingEvent e)
            {
                pitchText.Text = "Start : " + e.CurrentState.Microphone.Pitch.ToString();
                background.Colour = Color4.Red;
                return true;
            }

            protected virtual bool OnMicrophoneEndSinging(MicrophoneEndPitchingEvent e)
            {
                pitchText.Text = "End : " + e.CurrentState.Microphone.Pitch.ToString();
                background.Colour = Color4.Yellow;
                return true;
            }

            protected virtual bool OnMicrophoneSinging(MicrophonePitchingEvent e)
            {
                var scale = e.CurrentState.Microphone.Pitch;
                Y = (float)-(scale - 50) * 5;
                pitchText.Text = "Singing : " + scale;
                background.Colour = Color4.Blue;
                return true;
            }
        }
    }
}
