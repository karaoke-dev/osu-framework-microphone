// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
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
                    case MicrophoneStartSingingEvent microphoneStartSinging:
                        return OnMicrophoneStartSinging(microphoneStartSinging);

                    case MicrophoneEndSingingEvent microphoneEndSinging:
                        return OnMicrophoneEndSinging(microphoneEndSinging);

                    case MicrophoneSingingEvent microphoneSinging:
                        return OnMicrophoneSinging(microphoneSinging);

                    default:
                        return base.Handle(e);
                }
            }

            protected virtual bool OnMicrophoneStartSinging(MicrophoneStartSingingEvent e)
            {
                pitchText.Text = "Start : " + e.CurrentState.Microphone.Pitch.ToString();
                background.Colour = Color4.Red;
                return true;
            }

            protected virtual bool OnMicrophoneEndSinging(MicrophoneEndSingingEvent e)
            {
                pitchText.Text = "End : " + e.CurrentState.Microphone.Pitch.ToString();
                background.Colour = Color4.Yellow;
                return true;
            }

            protected virtual bool OnMicrophoneSinging(MicrophoneSingingEvent e)
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
