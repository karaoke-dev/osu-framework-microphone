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
            private readonly SpriteText _pitchText;

            public MicrophoneVisualization()
            {
                InternalChildren = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Red
                    },
                    _pitchText = new SpriteText
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
                _pitchText.Text = "Start : " + e.CurrentState.Microphone.Pitch.ToString();
                return true;
            }

            protected virtual bool OnMicrophoneEndSinging(MicrophoneEndSingingEvent e)
            {
                _pitchText.Text = "End : " + e.CurrentState.Microphone.Pitch.ToString();
                return true;
            }

            protected virtual bool OnMicrophoneSinging(MicrophoneSingingEvent e)
            {
                var scale = e.CurrentState.Microphone.Pitch;
                Y = (float)-(scale - 50) * 5;
                _pitchText.Text = "Singing : " + scale;
                return true;
            }
        }
    }
}
