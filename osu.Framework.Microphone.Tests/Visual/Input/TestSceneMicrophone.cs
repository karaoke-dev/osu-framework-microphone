﻿// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Input.Handlers.Microphone;
using osuTK.Graphics;

namespace osu.Framework.Tests.Visual.Input;

[TestFixture]
public partial class TestSceneMicrophone : FrameworkTestScene
{
    public TestSceneMicrophone()
    {
        var manager = new TestMicrophoneInputManager
        {
            RelativeSizeAxes = Axes.Both,
            Children = new Drawable[]
            {
                new MicrophonePitchVisualization
                {
                    X = 50
                },
                new MicrophoneDecibelVisualization
                {
                    X = 200,
                }
            }
        };
        AddSliderStep("sensitive", 0f, 100, 10, blur =>
        {
            var sensitive = manager.MicrophoneHandler.Sensitivity;
            sensitive.Value = blur;
        });

        Child = manager;
    }

    private partial class TestMicrophoneInputManager : MicrophoneInputManager
    {
        public MicrophoneHandler MicrophoneHandler => InputHandlers.OfType<MicrophoneHandler>().First();
    }

    private partial class MicrophonePitchVisualization : MicrophoneVisualization
    {
        protected override bool OnMicrophoneStartSinging(MicrophoneStartPitchingEvent e)
        {
            float pitch = e.CurrentState.Microphone.Voice.Pitch;
            BoxText.Text = "Pitch start : " + pitch;
            return base.OnMicrophoneStartSinging(e);
        }

        protected override bool OnMicrophoneEndSinging(MicrophoneEndPitchingEvent e)
        {
            float pitch = e.CurrentState.Microphone.Voice.Pitch;
            BoxText.Text = "Pitch end : " + pitch;
            return base.OnMicrophoneEndSinging(e);
        }

        protected override bool OnMicrophoneSinging(MicrophonePitchingEvent e)
        {
            float pitch = e.CurrentState.Microphone.Voice.Pitch;
            Y = -(pitch - 50);
            BoxText.Text = "Pitching : " + pitch;
            return base.OnMicrophoneSinging(e);
        }
    }

    private partial class MicrophoneDecibelVisualization : MicrophoneVisualization
    {
        protected override bool OnMicrophoneStartSinging(MicrophoneStartPitchingEvent e)
        {
            float decibel = e.CurrentState.Microphone.Voice.Decibel;
            BoxText.Text = "Decibel start : " + decibel;
            return base.OnMicrophoneStartSinging(e);
        }

        protected override bool OnMicrophoneEndSinging(MicrophoneEndPitchingEvent e)
        {
            float decibel = e.CurrentState.Microphone.Voice.Decibel;
            BoxText.Text = "Decibel end : " + decibel;
            return base.OnMicrophoneEndSinging(e);
        }

        protected override bool OnMicrophoneSinging(MicrophonePitchingEvent e)
        {
            float decibel = e.CurrentState.Microphone.Voice.Decibel;
            Y = -(decibel - 50) * 5;
            BoxText.Text = "Decibel : " + decibel;
            return base.OnMicrophoneSinging(e);
        }
    }

    private partial class MicrophoneVisualization : CompositeDrawable
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
