// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input;

namespace osu.Framework.Tests.Visual.Input;

[TestFixture]
public partial class TestSceneGetMicrophoneList : FrameworkTestScene
{
    public TestSceneGetMicrophoneList()
    {
        var manager = new MicrophoneManager();
        Child = new BasicDropdown<string>
        {
            X = 100,
            Y = 100,
            Width = 300,
            Items = manager.MicrophoneDeviceNames
        };
    }
}
