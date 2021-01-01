// Copyright (c) karaoke.dev <contact@karaoke.dev>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Foundation;
using osu.Framework.Input.Handlers.Microphone;
using osu.Framework.iOS;
using osu.Framework.iOS.Input;

namespace osu.Framework.Tests
{
    [Register("AppDelegate")]
    public class AppDelegate : GameAppDelegate
    {
        protected override Game CreateGame() => new TestingVisualTestGame();

        internal class TestingVisualTestGame : VisualTestGame
        {
            protected override void LoadComplete()
            {
                base.LoadComplete();

                // Need to cache IOSMicrophoneHandler in here to let MicrophoneInputManager knows.
                Host.Dependencies.CacheAs(typeof(OsuTKMicrophoneHandler), new IOSMicrophoneHandler());
            }
        }
    }
}
